using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Lumina
{
    public class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate UserTemplate { get; set; }
        public DataTemplate AITemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            // Cast to your ChatMessage class
            var message = item as ChatMessage;
            if (message == null)
                return base.SelectTemplateCore(item, container);

            // Choose template based on Role (case-insensitive)
            return message.Role.Equals("user", StringComparison.OrdinalIgnoreCase)
                ? UserTemplate
                : AITemplate;
        }
    }
}
