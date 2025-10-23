using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SocNetwork.Models.Db;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public DbSet<FriendShip> FriendShips { get; set; }

    // ✅ Добавляем сущность сообщений
    public DbSet<Message> Messages { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // --- Настройка Friendship ---
        builder.Entity<FriendShip>()
            .HasOne(f => f.Requester)
            .WithMany()
            .HasForeignKey(f => f.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FriendShip>()
            .HasOne(f => f.Addressee)
            .WithMany()
            .HasForeignKey(f => f.AddresseeId)
            .OnDelete(DeleteBehavior.Restrict);

        // --- Настройка Message ---
        builder.Entity<Message>(entity =>
        {
            entity.HasKey(m => m.Id);

            entity.Property(m => m.Text)
                  .IsRequired()
                  .HasMaxLength(2000);

            entity.Property(m => m.SentAt)
                  .IsRequired();

            entity.HasOne(m => m.Sender)
                  .WithMany()
                  .HasForeignKey(m => m.SenderId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Receiver)
                  .WithMany()
                  .HasForeignKey(m => m.ReceiverId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(m => new { m.SenderId, m.ReceiverId, m.SentAt });
        });

        // --- Конвертеры для PostgreSQL ---
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                // Для DateTime
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(
                        new ValueConverter<DateTime, DateTime>(
                            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                }
                // Для DateTimeOffset
                else if (property.ClrType == typeof(DateTimeOffset) || property.ClrType == typeof(DateTimeOffset?))
                {
                    property.SetValueConverter(
                        new ValueConverter<DateTimeOffset, DateTimeOffset>(
                            v => v.ToUniversalTime(),
                            v => v));
                }
            }
        }
    }
}
