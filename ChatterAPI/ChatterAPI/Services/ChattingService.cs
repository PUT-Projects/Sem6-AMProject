using ChatterAPI.Entities;
using ChatterAPI.Exceptions;
using ChatterAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatterAPI.Services;

public class ChattingService
{
    private readonly ChatterContext _context;

    public ChattingService(ChatterContext context)
    {
        _context = context;
    }

    public async Task SendMessage(Guid senderId, string sender, MessageDto message)
    {
        var receiver = await _context.Users.FirstOrDefaultAsync(u => u.Username == message.Receiver);

        if (receiver is null) {
            throw new BadRequestException("Receiver not found!");
        }

        if (receiver.Id == senderId) {
            throw new BadRequestException("You cannot send messages to yourself!");
        }

        var newMessage = new Message {
            SenderId = senderId,
            Sender = sender,
            ReceiverId = receiver.Id,
            Content = message.Content,
            TimeStamp = message.TimeStamp,
            Key = message.Key,
            IV = message.IV,
        };

        receiver.AwaitingMessages.Add(newMessage);

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<GetMessageDto>> GetAwaitingMessages(Guid userId)
    {
        var user = await _context.Users.Include(u => u.AwaitingMessages).FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null) {
            throw new BadRequestException("User not found!");
        }

        var messages = user.AwaitingMessages
            .Select(msg => new GetMessageDto {
                Sender = msg.Sender,
                Content = msg.Content,
                TimeStamp = msg.TimeStamp,
                Key = msg.Key,
                IV = msg.IV,
            }).ToList();

        user.AwaitingMessages.Clear();

        await _context.SaveChangesAsync();

        return messages;
    }


}
