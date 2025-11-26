using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;
using MassTransit;

using CatalogService.BLL.Services.Interfaces;
using CatalogService.BLL.Services.Implementations;
using AutoMapper;
using CatalogService.Domain.Entities;
using CatalogService.BLL.DTO;
using CatalogService.BLL.Exceptions;
using CatalogService.DAL.Repositories.Interfaces;
using CatalogService.DAL.UnitOfWork;
using CatalogService.Domain.QueryParametrs;
using CatalogService.DAL.Helpers;
using FlowerLab.Shared.Events;
using Microsoft.AspNetCore.Http;

namespace CatalogService.Tests.Services
{
    public class RecipientServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRecipientRepository> _recipientRepoMock;
        private readonly RecipientService _sut;

        public RecipientServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _recipientRepoMock = new Mock<IRecipientRepository>();

            _uowMock.Setup(u => u.Recipients).Returns(_recipientRepoMock.Object);
            _sut = new RecipientService(_uowMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsMappedRecipients()
        {
            var recipients = new List<Recipient>
            {
                new Recipient { Id = Guid.NewGuid(), Name = "Alice" },
                new Recipient { Id = Guid.NewGuid(), Name = "Bob" }
            };

            _recipientRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(recipients);
            _mapperMock.Setup(m => m.Map<IEnumerable<RecipientDto>>(recipients))
                       .Returns(recipients.Select(r => new RecipientDto(r.Id, r.Name)));

            var result = await _sut.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.Name == "Alice");
            Assert.Contains(result, r => r.Name == "Bob");
        }

        [Fact]
        public async Task GetByIdAsync_WhenRecipientExists_ReturnsRecipient()
        {
            var id = Guid.NewGuid();
            var recipient = new Recipient { Id = id, Name = "Alice" };
            var recipientDto = new RecipientDto(id, "Alice");

            _recipientRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(recipient);
            _mapperMock.Setup(m => m.Map<RecipientDto>(recipient)).Returns(recipientDto);

            var result = await _sut.GetByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Alice", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WhenRecipientDoesNotExist_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();
            _recipientRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Recipient)null);

            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetByIdAsync(id));

            Assert.Contains(id.ToString(), ex.Message);
        }

        [Fact]
        public async Task CreateAsync_WithValidName_CreatesRecipient()
        {
            var name = "Alice";

            _recipientRepoMock.Setup(r => r.ExistsWithNameAsync(name, null)).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<RecipientDto>(It.IsAny<Recipient>()))
                       .Returns((Recipient r) => new RecipientDto(r.Id, r.Name));

            var result = await _sut.CreateAsync(name);

            Assert.NotNull(result);
            Assert.Equal(name, result.Name);
            _recipientRepoMock.Verify(r => r.AddAsync(It.IsAny<Recipient>()), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenNameExists_ThrowsAlreadyExistsException()
        {
            var name = "Alice";
            _recipientRepoMock.Setup(r => r.ExistsWithNameAsync(name, null)).ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<AlreadyExistsException>(() => _sut.CreateAsync(name));

            Assert.Contains(name, ex.Message);
            _recipientRepoMock.Verify(r => r.AddAsync(It.IsAny<Recipient>()), Times.Never);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenRecipientExists_UpdatesRecipient()
        {
            var id = Guid.NewGuid();
            var recipient = new Recipient { Id = id, Name = "Alice" };
            var newName = "Bob";

            _recipientRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(recipient);
            _recipientRepoMock.Setup(r => r.ExistsWithNameAsync(newName, id)).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<RecipientDto>(recipient)).Returns(new RecipientDto(id, newName));

            var result = await _sut.UpdateAsync(id, newName);

            Assert.Equal(newName, result.Name);
            _recipientRepoMock.Verify(r => r.Update(recipient), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenRecipientDoesNotExist_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();
            _recipientRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Recipient)null);

            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(id, "Bob"));

            Assert.Contains(id.ToString(), ex.Message);
        }

        [Fact]
        public async Task UpdateAsync_WhenNameExists_ThrowsAlreadyExistsException()
        {
            var id = Guid.NewGuid();
            var recipient = new Recipient { Id = id, Name = "Alice" };
            _recipientRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(recipient);
            _recipientRepoMock.Setup(r => r.ExistsWithNameAsync("Bob", id)).ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<AlreadyExistsException>(() => _sut.UpdateAsync(id, "Bob"));

            Assert.Contains("Bob", ex.Message);
        }

        [Fact]
        public async Task DeleteAsync_WhenRecipientExists_DeletesRecipient()
        {
            var id = Guid.NewGuid();
            var recipient = new Recipient { Id = id, Name = "Alice" };
            _recipientRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(recipient);

            await _sut.DeleteAsync(id);

            _recipientRepoMock.Verify(r => r.Delete(recipient), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenRecipientDoesNotExist_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();
            _recipientRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Recipient)null);

            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(id));

            Assert.Contains(id.ToString(), ex.Message);
        }
    }
}
