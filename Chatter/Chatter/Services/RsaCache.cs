using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Services;

public class RsaCache
{
    private ConcurrentDictionary<string, string> _rsaKeys = new ();

    public void AddKey(string username, string publicKey)
    {
        _rsaKeys.TryAdd(username, publicKey);
    }

    public string? GetKey(string username)
    {
        return _rsaKeys.TryGetValue(username, out string? publicKey) ? publicKey : null;
    }

    public bool ContainsKey(string username)
    {
        return _rsaKeys.ContainsKey(username);
    }
}
