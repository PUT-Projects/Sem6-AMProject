using Chatter.Entities;
using Chatter.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Repositories;

public class UserDataRepository : IDisposable
{
    private readonly IApiService _apiService;
    private readonly DbSettings _dbSettings;

    private ISQLiteConnection? _connection = null;

    public UserDataRepository(DbSettings dbSettings, IApiService apiService)
    {
        _dbSettings = dbSettings;
        _apiService = apiService;
        
        UpdateConnection();
    }

    public void UpdateConnection()
    {
        _connection?.Close();

        _connection = new SQLiteConnection(_dbSettings.UserDataDbPath, DbSettings.Flags);
        _connection.CreateTable<UserData>();
    }

    public void AddUserData(string xml)
    {
         var x = _connection!.Insert(new UserData { Username = _apiService.Username, XmlKeys = xml });
    }

    public bool UserDataExists(string username)
    {
        return _connection!.Table<UserData>()
            .Any(u => u.Username == username);
    }

    public UserData? GetCurrentUserData()
    {
        if (string.IsNullOrEmpty(_apiService.Username)) {
            //throw new InvalidOperationException("Users encryption data doesn't exist!");
        }

        return _connection!.Table<UserData>()
            .Where(u => u.Username == _apiService.Username)
            .FirstOrDefault();
    }


    #region IDisposable
    ~UserDataRepository()
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
            _connection.Close();

            _disposed = true;
        }
    }
    #endregion
}
