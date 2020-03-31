using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Eco_Reddit.Models
{
    public class SubredditList
    {
        public string TitleSubreddit { get; set; }
        public string SubredditIcon { get; set; }
        public Subreddit SubredditSelf { get; set; }
        public Visibility IsNSFW { get; set; }
    }
}
