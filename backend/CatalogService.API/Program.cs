
using CatalogService.DAL.Context;
using CatalogService.DAL.Repositories.Implementations;
using CatalogService.DAL.Repositories.Interfaces;
using CatalogService.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IFlowerRepository, FlowerRepository>();
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<ISizeRepository, SizeRepository>();
            builder.Services.AddScoped<IRecipientRepository, RecipientRepository>();
            builder.Services.AddScoped<IGiftRepository, GiftRepository>();
            builder.Services.AddScoped<IBouquetRepository, BouquetRepository>();

            // Ðåºñòðóºìî UnitOfWork
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
