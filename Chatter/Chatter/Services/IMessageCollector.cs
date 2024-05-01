using Chatter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Services;

public interface IMessageCollector
{
    void AddObserver(Func<IEnumerable<DecryptedMessage>, Task> observer);
    void RemoveObserver(Func<IEnumerable<DecryptedMessage>, Task> observer);
    void StartCollectingMessages();
    void MuteNotificationsFrom(string username);
    void UnMute();
    void Stop();
}
