using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using RedditSharp.Things;

namespace Eco_Reddit.Templates
{
    public class CommentsTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CommentDataTemplate { get; set; }

        public DataTemplate MoreDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is Comment)
                return CommentDataTemplate;
            return MoreDataTemplate;
        }
    }
}
