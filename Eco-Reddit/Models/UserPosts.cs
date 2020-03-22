using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Eco_Reddit.Models
{
    public class UserPosts
    {
        public string TitleText { get; set; }
        public string PostDate { get; set; }
        public Post PostSelf { get; set; }
        public string PostUpvotes { get; set; }
        public bool PostDownVoted { get; set; }
        public bool PostUpvoted { get; set; }
        public string PostSubreddit { get; set; }
        public string PostCommentCount { get; set; }
        public string PostFlair { get; set; }
        public string LinkSource { get; set; }
        public Visibility PostType { get; set; }
        public string PostFlairColor { get; set; }
    }
}
