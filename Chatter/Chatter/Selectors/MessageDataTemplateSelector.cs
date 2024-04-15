using Chatter.Models;
using Chatter.Models.Dashboard;
using Chatter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            return msg.Sender == _apiService.Username ? GetMyTemplate() : GetFriendTemplate();
        }

        private DataTemplate GetMyTemplate()
        {
            Application.Current!.Resources.TryGetValue("MyMessageStyle", out var template);
            return (DataTemplate)template;
        }

        private DataTemplate GetFriendTemplate()
        {
            Application.Current!.Resources.TryGetValue("FriendMessageStyle", out var template);
            return (DataTemplate)template;
        }

    }
}
