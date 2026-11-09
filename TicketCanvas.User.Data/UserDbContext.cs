using Microsoft.EntityFrameworkCore;
using TicketCanvas.User.Data.Models;
using UserModel = TicketCanvas.User.Data.Models.User;

namespace TicketCanvas.User.Data;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<UserModel> Users => Set<UserModel>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserModel>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        
        configurationBuilder
            .Properties<Enum>()
            .HaveConversion<string>();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<Entity>();
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    break;
                case EntityState.Modified:
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    entry.Entity.UpdatedAt = now;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
