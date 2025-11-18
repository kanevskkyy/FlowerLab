
using CatalogService.API.Middleware;
using CatalogService.BLL.Automapper;
using CatalogService.BLL.DTO;
using CatalogService.BLL.Services.Implementations;
using CatalogService.BLL.Services.Interfaces;
using CatalogService.BLL.Validators;
using CatalogService.DAL.Context;
using CatalogService.DAL.Repositories.Implementations;
using CatalogService.DAL.Repositories.Interfaces;
using CatalogService.DAL.UnitOfWork;
using CloudinaryDotNet;
using DotNetEnv;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CatalogService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            /////////////////////////////////
            builder.Services.Configure<CloudSettings>(
                builder.Configuration.GetSection("Cloudinary")
            );
            ///////////////////////////////////

            // Add services to the container.
            builder.AddNpgsqlDbContext<CatalogDbContext>("FlowerLabCatalog");

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IFlowerRepository, FlowerRepository>();
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<ISizeRepository, SizeRepository>();
            builder.Services.AddScoped<IRecipientRepository, RecipientRepository>();
            builder.Services.AddScoped<IBouquetRepository, BouquetRepository>();

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<ISizeService, SizeService>();
            builder.Services.AddScoped<IRecipientService, RecipientService>();
            builder.Services.AddScoped<IFlowerService, FlowerService>();
            builder.Services.AddScoped<IBouquetService, BouquetService>();

            // Ðåºñòðóºìî UnitOfWork
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IImageService, CloudinaryImageService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseMiddleware<RequestLoggingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            using (var scope = app.Services.CreateAsyncScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
                db.Database.Migrate();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
