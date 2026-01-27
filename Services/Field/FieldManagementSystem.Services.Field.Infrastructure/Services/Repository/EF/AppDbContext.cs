using FieldManagementSystem.Services.Field.Core.Types;
using Microsoft.EntityFrameworkCore;

namespace FieldManagementSystem.Services.Field.Infrastructure.Services.Repository.EF;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<FieldEntity> Fields => Set<FieldEntity>();
    public DbSet<FieldAuthorizedUserRow> FieldAuthorizedUsers => Set<FieldAuthorizedUserRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureField(modelBuilder);
        ConfigureFieldAuthorizedUsers(modelBuilder);
    }

    private static void ConfigureField(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FieldEntity>(b =>
        {
            b.ToTable("fields");

            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            b.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(200)
                .IsRequired();

            b.Property(x => x.Description)
                .HasColumnName("description")
                .IsRequired();

            b.Property(x => x.CreatedDate)
                .HasColumnName("created_date")
                .HasDefaultValueSql("NOW()");

            b.Property(x => x.ModifiedDate)
                .HasColumnName("modified_date")
                .HasDefaultValueSql("NOW()");

            // Computed property → not mapped
            b.Ignore(x => x.AuthorizedUsers);

            // One field → many authorized users (join rows)
            b.HasMany(x => x.AuthorizedUserLinks)
                .WithOne()
                .HasForeignKey(x => x.FieldId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => x.Name).IsUnique();
        });
    }
    private static void ConfigureFieldAuthorizedUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FieldAuthorizedUserRow>(b =>
        {
            b.ToTable("field_authorized_users");
            
            b.HasKey(x => new { x.FieldId, x.UserId });

            b.Property(x => x.FieldId)
                .HasColumnName("field_id");

            b.Property(x => x.UserId)
                .HasColumnName("user_id");
        });
    }

}