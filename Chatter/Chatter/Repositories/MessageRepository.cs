using Chatter.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Repositories;

public class MessageRepository : IDisposable
{
    private readonly ISQLiteAsyncConnection _connection;
    public MessageRepository()
    {
        _connection = new SQLiteAsyncConnection(DbSettings.FullPath, DbSettings.Flags);

        _connection.CreateTableAsync<Message>().Wait();
    }

    public async Task AddMessage(Message message)
    {
        await _connection.InsertAsync(message);
    }

    public async Task<IEnumerable<Message>> GetMessagesAsync(string sender, int count = 50)
    {
        return await _connection
            .Table<Message>()
            .Where(m => m.Sender == sender)
            .OrderBy(m => m.TimeStamp)
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
            _connection.CloseAsync().Wait();

            _disposed = true;
        }
    }
    #endregion
}
