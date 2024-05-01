using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Models;

public class EncryptedMessage
{
    public string Content { get; set; }
    public string Key { get; set; }
    public string IV { get; set; }
}
