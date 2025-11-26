using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;

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
    public class EventServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly EventService _sut;

        public EventServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            _sut = new EventService(_uowMock.Object, _mapperMock.Object);
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ReturnsAllEvents()
        {
            var events = new List<Event>
            {
                new Event { Id = Guid.NewGuid(), Name = "Event 1" },
                new Event { Id = Guid.NewGuid(), Name = "Event 2" }
            };

            _uowMock.Setup(u => u.Events.GetAllAsync()).ReturnsAsync(events);
            _mapperMock.Setup(m => m.Map<IEnumerable<EventDto>>(events))
                .Returns(events.Select(e => new EventDto(e.Id, e.Name)));

            var result = await _sut.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, e => e.Name == "Event 1");
            Assert.Contains(result, e => e.Name == "Event 2");
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WhenEventExists_ReturnsEvent()
        {
            var id = Guid.NewGuid();
            var ev = new Event { Id = id, Name = "Test Event" };

            _uowMock.Setup(u => u.Events.GetByIdAsync(id)).ReturnsAsync(ev);
            _mapperMock.Setup(m => m.Map<EventDto>(ev))
                .Returns(new EventDto(ev.Id, ev.Name));

            var result = await _sut.GetByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal("Test Event", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WhenEventDoesNotExist_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();
            _uowMock.Setup(u => u.Events.GetByIdAsync(id)).ReturnsAsync((Event)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetByIdAsync(id));
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithUniqueName_CreatesEvent()
        {
            var name = "New Event";
            var entity = new Event { Id = Guid.NewGuid(), Name = name };

            _uowMock.Setup(u => u.Events.ExistsWithNameAsync(It.IsAny<string>(), It.IsAny<Guid?>()))
                    .ReturnsAsync(false); _uowMock.Setup(u => u.Events.AddAsync(It.IsAny<Event>()))
                .Callback<Event>(e => entity = e)
                .Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mapperMock.Setup(m => m.Map<EventDto>(It.IsAny<Event>()))
                .Returns((Event e) => new EventDto(e.Id, e.Name));

            var result = await _sut.CreateAsync(name);

            Assert.NotNull(result);
            Assert.Equal(name, result.Name);
            _uowMock.Verify(u => u.Events.AddAsync(It.IsAny<Event>()), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenNameExists_ThrowsAlreadyExistsException()
        {
            // Arrange
            var name = "Existing Event";

            // Настраиваем мок, чтобы ExistsWithNameAsync возвращал true
            _uowMock.Setup(u => u.Events.ExistsWithNameAsync(name, null))
                    .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AlreadyExistsException>(() =>
                _sut.CreateAsync(name)
            );

            Assert.Contains(name, exception.Message);

            // Проверяем, что AddAsync не вызывался
            _uowMock.Verify(u => u.Events.AddAsync(It.IsAny<Domain.Entities.Event>()), Times.Never);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WhenEventExists_UpdatesEvent()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var existingEvent = new Event { Id = eventId, Name = "Old Name" };

            _uowMock.Setup(u => u.Events.GetByIdAsync(eventId))
                    .ReturnsAsync(existingEvent);

            _uowMock.Setup(u => u.Events.ExistsWithNameAsync("Updated Name", eventId))
                    .ReturnsAsync(false);

            _uowMock.Setup(u => u.Events.Update(It.IsAny<Event>()));
            _uowMock.Setup(u => u.SaveChangesAsync())
                    .ReturnsAsync(1);

            _mapperMock.Setup(m => m.Map<EventDto>(It.IsAny<Event>()))
                       .Returns((Event ev) => new EventDto(ev.Id, ev.Name));

            // Act
            var result = await _sut.UpdateAsync(eventId, "Updated Name");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.Name);
            _uowMock.Verify(u => u.Events.Update(existingEvent), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenEventDoesNotExist_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();
            var newName = "Test Name";

            _uowMock.Setup(u => u.Events.GetByIdAsync(id)).ReturnsAsync((Event)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(id, newName));
        }

        [Fact]
        public async Task UpdateAsync_WhenNameAlreadyExists_ThrowsAlreadyExistsException()
        {
            var id = Guid.NewGuid();
            var ev = new Event { Id = id, Name = "Old Name" };
            var newName = "Duplicate Name";

            _uowMock.Setup(u => u.Events.GetByIdAsync(id)).ReturnsAsync(ev);
            _uowMock.Setup(u => u.Events.ExistsWithNameAsync(newName, id)).ReturnsAsync(true);

            await Assert.ThrowsAsync<AlreadyExistsException>(() => _sut.UpdateAsync(id, newName));
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WhenEventExists_DeletesEvent()
        {
            var id = Guid.NewGuid();
            var ev = new Event { Id = id, Name = "Test Event" };

            _uowMock.Setup(u => u.Events.GetByIdAsync(id)).ReturnsAsync(ev);
            _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            await _sut.DeleteAsync(id);

            _uowMock.Verify(u => u.Events.Delete(ev), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenEventDoesNotExist_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();
            _uowMock.Setup(u => u.Events.GetByIdAsync(id)).ReturnsAsync((Event)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(id));
        }

        #endregion
    }
}
