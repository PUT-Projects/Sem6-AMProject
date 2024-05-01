using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Entities;

[Table("UserData")]
public class UserData
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [NotNull, Unique]
    public string Username { get; set; }
    [NotNull]
    public string XmlKeys { get; set; }

}
