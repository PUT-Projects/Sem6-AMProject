using Chatter.Models;
using Chatter.Models.Dashboard;
using Chatter.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chatter.Selectors
{
    public class MessageDataTemplateSelector : DataTemplateSelector
    {
        private readonly IApiService _apiService;

        public MessageDataTemplateSelector()
        {
            _apiService = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetRequiredService<IApiService>();
        }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var msg = item as Message;
            if (msg == null) {
                throw new ArgumentException("Item is not a Message");
            }

            return msg.Sender == _apiService.Username ? GetMyTemplate(msg) : GetFriendTemplate(msg);
        }

        private DataTemplate GetMyTemplate(Message message)
        {
            if (ContainsOnlyEmojis(message.Content)) {
                Application.Current!.Resources.TryGetValue("MyMessageOnlyEmojiStyle", out var template);
                return (DataTemplate)template;
            }
            else {
                Application.Current!.Resources.TryGetValue("MyMessageStyle", out var template);
                return (DataTemplate)template;
            }
        }

        private DataTemplate GetFriendTemplate(Message message)
        {
            if (ContainsOnlyEmojis(message.Content)) {
                Application.Current!.Resources.TryGetValue("FriendMessageOnlyEmojiStyle", out var template);
                return (DataTemplate)template;
            }
            else {
                Application.Current!.Resources.TryGetValue("FriendMessageStyle", out var template);
                return (DataTemplate)template;
            }
        }


        private const string _pattern = @"[\u2600-\u26FF\u2700-\u27BF\uD83C\uD83D\uD83E][\uDC00-\uDFFF]?|\uD83C[\uDDE6-\uDDFF]{1,2}|[\uD83D\uD83E][\uDC00-\uDFFF]";
        private static Regex _regex = new(_pattern, RegexOptions.Compiled);
        private static bool ContainsOnlyEmojis(string input)
        {
            return _regex.IsMatch(input);
        }
    }
}
