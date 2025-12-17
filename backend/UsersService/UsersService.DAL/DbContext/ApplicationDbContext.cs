namespace UsersService.DAL.DbContext;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using shared.events.EventService;
using UsersService.Domain.Entities; 

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    public virtual DbSet<UserAddresses> Addresses { get; set; }
    public virtual DbSet<ProcessedEvent> ProcessedEvent { get; set; }


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        foreach (var foreignKey in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }

        builder.Entity<RefreshToken>()
            .HasIndex(t => t.Token)
            .IsUnique();

        builder.Entity<ProcessedEvent>(entity =>
        {
            entity.HasKey(p => p.Id);
        });
            

        builder.Entity<UserAddresses>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Address).IsRequired();

            entity.HasOne(a => a.User)
                  .WithMany(u => u.Addresses)
                  .HasForeignKey(a => a.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}