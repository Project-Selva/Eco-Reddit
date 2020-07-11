using System;
using System.Collections.Generic;
using System.Linq;
using RedditSharp.Things;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Eco_Reddit.Templates
{
    public class CommentsTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CommentDataTemplate { get; set; }

        public DataTemplate MoreDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is Comment)
            {
                return CommentDataTemplate;
            }
            else
            {
                return MoreDataTemplate;
            }
        }
    }
}
