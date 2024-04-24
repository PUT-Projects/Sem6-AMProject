using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Entities;

[Table("Notifications")]
public class Notifications
{
    public string From { get; set; }
}
