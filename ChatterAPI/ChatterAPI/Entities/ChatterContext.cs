using Microsoft.EntityFrameworkCore;

namespace ChatterAPI.Entities;

public class ChatterContext : DbContext
{
    public ChatterContext(DbContextOptions<ChatterContext> options)
        : base(options)
    { }
    public DbSet<User> Users { get; set; }
    public DbSet<Message> AwaitingMessages { get; set; }
    public DbSet<FriendPair> FriendPairs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<User>();
        user.HasKey(u => u.Id);
        user.Property(u => u.Username).IsRequired().HasMaxLength(20);
        user.Property(u => u.PasswordHash).IsRequired();
        user.HasMany(u => u.AwaitingMessages).WithOne().HasForeignKey(m => m.ReceiverId);
        user.Property(u => u.PublicKey).IsRequired();

        var message = modelBuilder.Entity<Message>();
        message.HasKey(m => m.Id);
        message.Property(m => m.SenderId).IsRequired();
        message.Property(m => m.ReceiverId).IsRequired();
        message.Property(m => m.Content).IsRequired();
        message.Property(m => m.Key).IsRequired().HasMaxLength(384);
        message.Property(m => m.IV).IsRequired().HasMaxLength(384);
        message.Property(m => m.Type).IsRequired();
        message.Property(m => m.TimeStamp).IsRequired();

        var friendPair = modelBuilder.Entity<FriendPair>();
        friendPair.HasKey(f => new { f.UserId, f.FriendId });
        friendPair.Property(f => f.UserId).IsRequired();
        friendPair.Property(f => f.FriendId).IsRequired();
        friendPair.Property(f => f.FriendshipStatus).IsRequired();
        friendPair.Property(f => f.TimeStamp).IsRequired();
    }
}
