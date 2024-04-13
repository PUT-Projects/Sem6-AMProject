using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Models.Dashboard
{
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public bool IsOnline { get; set; } = false;
        public string ImageUrl => ProfilePictures.GetPictureFromUsername(Username);
    }
}
