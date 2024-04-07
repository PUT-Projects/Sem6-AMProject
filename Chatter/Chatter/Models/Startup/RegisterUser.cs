using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Models.Startup;

public class RegisterUser : User
{
    public string ConfirmPassword { get; set; } = string.Empty;
}
