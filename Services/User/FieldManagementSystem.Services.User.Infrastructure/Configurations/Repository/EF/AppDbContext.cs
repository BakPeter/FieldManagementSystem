using FieldManagementSystem.Services.User.Core.Types;
using Microsoft.EntityFrameworkCore;

namespace FieldManagementSystem.Services.User.Infrastructure.Configurations.Repository.EF;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<UserTypeRow> UserTypes => Set<UserTypeRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserTypeRow>(b =>
        {
            b.ToTable("user_types");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
            b.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<UserEntity>(b =>
        {
            b.ToTable("users");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            // Map FK int column (users.user_type)
            b.Property(x => x.UserTypeId)
                .HasColumnName("user_type")
                .IsRequired();

            // UserType enum is derived from UserTypeId -> do not map it
            b.Ignore(x => x.UserType);

            // FK: users.user_type -> user_types.id
            b.HasOne<UserTypeRow>()
                .WithMany()
                .HasForeignKey(x => x.UserTypeId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);

            b.Property(x => x.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            b.HasIndex(x => x.Email).IsUnique();

            b.Property(x => x.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
            b.Property(x => x.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();

            b.Property(x => x.CreatedDate).HasColumnName("created_date");
            b.Property(x => x.ModifiedDate).HasColumnName("modified_date");
        });
    }
}


// Lookup table row
public sealed class UserTypeRow
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}