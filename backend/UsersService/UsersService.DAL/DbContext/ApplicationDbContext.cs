namespace UsersService.DAL.DbContext;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UsersService.Domain.Entities; // Припускаючи, що ви використали такий namespace

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<UserAddress> Addresses { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Налаштування для уникнення каскадних видалень, особливо важливо
        // коли ви видаляєте користувача, щоб не видалити інші сутності
        foreach (var foreignKey in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // Приклад налаштування ключа для RefreshToken
        builder.Entity<RefreshToken>()
            .HasIndex(t => t.Token)
            .IsUnique();
    }
}