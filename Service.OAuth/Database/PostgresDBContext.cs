using MaClasse.Shared;
using MaClasse.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Service.OAuth.Database;

public class PostgresDbContext : DbContext
{
    public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Email).IsRequired();
            entity.Property(a => a.Name).IsRequired();
            entity.Property(a => a.GivenName).IsRequired();
            entity.Property(a => a.FamilyName).IsRequired();
            entity.Property(a => a.Picture).IsRequired();
            entity.Property(a => a.CreatedAt);
            entity.Property(a => a.UpdatedAt);
        });
    }
}