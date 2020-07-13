using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using RedditSharp.Things;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Eco_Reddit.Helpers;
using Windows.UI.Popups;
using Windows.System;
using Microsoft.Toolkit.Uwp.UI.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Eco_Reddit.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PostCommentPage : Page
    {
        private readonly ObservableCollection<Thing> commentTree = null;
        private Post selectedPost = null;
        private readonly LoginHelper loginHelper = new LoginHelper("mp8hDB_HfbctBg", "UCIGqKPDABnjb0XtMh0Q_LhrNks");
        public PostCommentPage()
        {
            this.InitializeComponent();

            commentTree = new ObservableCollection<Thing>();
        }
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string accessToken = localSettings.Values["access_token"].ToString();
            var result = await loginHelper.Login_Refresh((string)localSettings.Values["refresh_token"]);
         //   localSettings.Values["refresh_token"] = result.RefreshToken;
            localSettings.Values["access_token"] = result.AccessToken;
            Eco_Reddit.Models.TokenSharpData.Reddit = new RedditSharp.Reddit(result.AccessToken);
            await Eco_Reddit.Models.TokenSharpData.Reddit.InitOrUpdateUserAsync();
            RedditSharp.Listing<Comment> s = Eco_Reddit.Models.TokenSharpData.Reddit.User.GetUsernameMentions();
            selectedPost = await Eco_Reddit.Models.TokenSharpData.Reddit.GetPostAsync(new Uri("https://www.reddit.com/r/AskReddit/comments/hpc07p/the_last_thing_you_googled_is_how_youre_goona_get/"));
            List <Thing> commentsWithMores = null;
            try
            {
                 commentsWithMores = await selectedPost.GetCommentsWithMoresAsync(100);
            }
            catch
            {

            }
            foreach (var comment in commentsWithMores)
            {
                if (comment is Comment) //Flatten tree for list
                {
                    Stack<Thing> flatTree = new Stack<Thing>();
                    DepthFirstTraversal(comment as Comment, flatTree);
                    flatTree.Reverse().ToList().ForEach((x) => commentTree.Add(x));
                }
                else if (comment is More)
                {
                    commentTree.Add(comment);
                }
            }
        }

        public void DepthFirstTraversal(Comment root, Stack<Thing> stack)
        {
            stack.Push(root);
            foreach (Thing thing in root.Comments)
            {
                if (thing is Comment)
                {
                    DepthFirstTraversal(thing as Comment, stack);
                }
                else if (thing is More)
                {
                    stack.Push(thing);
                }
            }
        }

        public static string GetLoadMoreCommentsString(string[] children)
        {
            return "Load " + children.Length + " more comments";
        }
        private async void MarkdownText_ImageClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out Uri link))
            {
                await Launcher.LaunchUriAsync(link);
            }
        }
        private async void MarkdownText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out Uri link))
            {
                await Launcher.LaunchUriAsync(link);
            }
        }
        private async void Comments_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is More)
            {
                var clickedItem = e.ClickedItem as More;
                var Id = clickedItem.Id;
                var index = commentTree.IndexOf(clickedItem);

                //Load all child comments
                var comments = await Task.Run(() => clickedItem.GetThingsAsync());
                List<Thing> tempList = new List<Thing>();

                foreach (var comment in comments)
                {
                    if (comment is Comment) //Flatten tree for listview
                    {
                        Stack<Thing> flatTree = new Stack<Thing>();
                        DepthFirstTraversal(comment as Comment, flatTree);
                        flatTree.Reverse().ToList().ForEach((x) => tempList.Add(x));
                    }
                    else if (comment is More && (comment as More).Children.Count() > 0)
                    {
                        tempList.Add(comment);
                    }
                }

                if (tempList.Count == 0)
                {
                    //Remove the item and return
                    commentTree.RemoveAt(index);
                    return;
                }

                //Replace more item at index
                commentTree[index] = tempList[0];
                tempList.RemoveAt(0); index++;

                //Insert each item of templist at specific index
                tempList.ForEach((x) =>
                {
                    commentTree.Insert(index, x);
                    index++;
                });
            }
        }
    }
}
