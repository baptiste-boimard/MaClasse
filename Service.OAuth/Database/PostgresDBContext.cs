using MaClasse.Shared;
using MaClasse.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Service.OAuth.Database;

public class PostgresDbContext : DbContext
{
    public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<SessionData> SessionDatas { get; set; }
    public DbSet<Rattachment> Rattachments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.IdRole).IsRequired();
            entity.Property(a => a.Email).IsRequired();
            entity.Property(a => a.Name).IsRequired();
            entity.Property(a => a.Role).IsRequired();
            entity.Property(a => a.Zone).IsRequired();
            entity.Property(a => a.GivenName).IsRequired();
            entity.Property(a => a.FamilyName).IsRequired();
            entity.Property(a => a.Picture).IsRequired();
            entity.Property(a => a.CreatedAt);
            entity.Property(a => a.UpdatedAt);
        });

        modelBuilder.Entity<SessionData>(entity =>
        {
            entity.HasKey(a => a.Token);
            entity.Property(a => a.UserId).IsRequired();
            entity.Property(a => a.Role);
            entity.Property(a => a.Expiration);
        });

        modelBuilder.Entity<Rattachment>(entity =>
        {
            entity.HasKey(a => a.IdGuid);
            entity.Property(a => a.IdDirecteur).IsRequired();
            entity.Property(a => a.IdProfesseur).IsRequired();
        });
    }
}