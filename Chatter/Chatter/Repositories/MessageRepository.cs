using Chatter.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Repositories;

public class MessageRepository : IDisposable
{
    private readonly DbSettings _dbSettings;
    private ISQLiteAsyncConnection? _connection;
    public MessageRepository(DbSettings dbSettings)
    {
        _dbSettings = dbSettings;

        UpdateConnection();
    }

    public void UpdateConnection()
    {
        _connection?.CloseAsync().Wait();
        _connection = new SQLiteAsyncConnection(_dbSettings.MessagesDbPath, DbSettings.Flags);
        _connection.CreateTableAsync<Message>().Wait();
    }

    public async Task AddMessage(Message message)
    {
        await _connection!.InsertAsync(message);
    }
    public async Task AddMessages(IEnumerable<Message> messages)
    {
        var res = await _connection!.InsertAllAsync(messages);
    }

    public async Task<List<Message>> GetMessagesAsync(string sender, int count = 2)
    {
        return await _connection!.Table<Message>()
            .Where(m => m.Sender == sender || m.Receiver == sender)
            .OrderByDescending(m => m.TimeStamp)
            .Take(count)
            .ToListAsync();
    }


    #region IDisposable
    ~MessageRepository()
    {
        Dispose(false);
    }
    private bool _disposed = false;
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing) {
            _connection?.CloseAsync().Wait();

            _disposed = true;
        }
    }
    #endregion
}
