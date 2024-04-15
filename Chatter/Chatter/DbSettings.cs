using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter;

public class DbSettings
{
    public const SQLiteOpenFlags Flags = 
        SQLiteOpenFlags.ReadWrite |
        SQLiteOpenFlags.Create |
        SQLiteOpenFlags.SharedCache;

    public static string FullPath => Path.Combine(FileSystem.AppDataDirectory, _filename);

    private const string _filename = "chatter.db3";
}
