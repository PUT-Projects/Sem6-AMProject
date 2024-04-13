using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Chatter;

public static class ProfilePictures
{
    private static readonly List<string> _images;
    static ProfilePictures()
    {
        _images = new List<string>();
        for (int i = 1; i <= 52; ++i) {
            _images.Add($"ProfilePictures/profile{i}.png");
        }
    }
    public static string GetPictureFromUsername(string username)
    {
        int hash = username.GetDeterministicHashCode();
        int index = Math.Abs(hash) % _images.Count;
        return _images[index];
    }

}
