using Chatter.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Selectors
{
    public class UserDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var user = item as SearchItem;
            if (user == null) {
                throw new ArgumentException("Item is not a SearchItem");
            }
            if (user.IsLabel) {
                return GetLabelTemplate();
            }
            return user.IsFriend ? GetFriendTemplate() : GetUserTemplate();
        }

        private DataTemplate GetUserTemplate()
        {
            Application.Current!.Resources.TryGetValue("SearchUserStyle", out var template);
            return (DataTemplate)template;
        }

        private DataTemplate GetFriendTemplate()
        {
            Application.Current!.Resources.TryGetValue("SearchFriendStyle", out var template);
            return (DataTemplate)template;
        }
        private DataTemplate GetLabelTemplate()
        {
            Application.Current!.Resources.TryGetValue("SearchLabelStyle", out var template);
            return (DataTemplate)template;
        }
    }
}
