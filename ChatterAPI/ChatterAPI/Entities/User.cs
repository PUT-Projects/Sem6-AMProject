using System.ComponentModel.DataAnnotations;

namespace ChatterAPI.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public List<Message> AwaitingMessages { get; set; } = new();
}
