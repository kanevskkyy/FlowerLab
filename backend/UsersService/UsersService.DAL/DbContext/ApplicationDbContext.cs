namespace UsersService.DAL.DbContext;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UsersService.Domain.Entities; // Припускаючи, що ви використали такий namespace

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    public virtual DbSet<Address> Addresses { get; set; }


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

        builder.Entity<Address>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Country).IsRequired();
            entity.Property(a => a.City).IsRequired();
            entity.Property(a => a.Street).IsRequired();
            entity.Property(a => a.PostalCode).IsRequired();

            entity.HasOne(a => a.User)
                  .WithMany(u => u.Addresses)
                  .HasForeignKey(a => a.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}