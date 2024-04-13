using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Models.Dashboard;

public class AcceptUser
{
    public string Username { get; set; }
    public string ImageUrl => ProfilePictures.GetPictureFromUsername(Username);
}
