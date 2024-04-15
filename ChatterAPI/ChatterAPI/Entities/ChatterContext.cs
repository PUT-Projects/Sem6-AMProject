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
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<User>().Property(u => u.Username).IsRequired();
        modelBuilder.Entity<User>().Property(u => u.PasswordHash).IsRequired();
        modelBuilder.Entity<User>().HasMany(u => u.AwaitingMessages).WithOne().HasForeignKey(m => m.ReceiverId);

        modelBuilder.Entity<Message>().HasKey(m => m.Id);
        modelBuilder.Entity<Message>().Property(m => m.SenderId).IsRequired();
        modelBuilder.Entity<Message>().Property(m => m.ReceiverId).IsRequired();
        modelBuilder.Entity<Message>().Property(m => m.Content).IsRequired();
        modelBuilder.Entity<Message>().Property(m => m.Type).IsRequired();
        modelBuilder.Entity<Message>().Property(m => m.TimeStamp).IsRequired();

        modelBuilder.Entity<FriendPair>().HasKey(f => new { f.UserId, f.FriendId });
        modelBuilder.Entity<FriendPair>().Property(f => f.UserId).IsRequired();
        modelBuilder.Entity<FriendPair>().Property(f => f.FriendId).IsRequired();
        modelBuilder.Entity<FriendPair>().Property(f => f.FriendshipStatus).IsRequired();
        modelBuilder.Entity<FriendPair>().Property(f => f.TimeStamp).IsRequired();
    }
}
