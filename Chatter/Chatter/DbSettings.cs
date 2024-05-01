using Chatter.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter;

public class DbSettings
{
    private readonly IApiService _apiService;
    public DbSettings(IApiService apiService)
    {
        _apiService = apiService;
    }

    public const SQLiteOpenFlags Flags = 
        SQLiteOpenFlags.ReadWrite |
        SQLiteOpenFlags.Create |
        SQLiteOpenFlags.SharedCache;

    public string MessagesDbPath 
        => Path.Combine(
                FileSystem.AppDataDirectory, 
                $"chatter{_apiService.Username.GetDeterministicHashCode()}.db3"
            );

    public string UserDataDbPath => 
        Path.Combine(FileSystem.AppDataDirectory, "chatter-userdata.db3");
}
