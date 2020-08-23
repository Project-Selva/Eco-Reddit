using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Selva.Helpers;
using Selva.Core.Models;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Selva.Core.Things;
using Selva.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Selva.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PostCommentPage : Page
    {
        private readonly ObservableCollection<Thing> commentTree;
        private readonly Selva.Core.Helpers.LoginHelpers.LoginHelper loginHelper = new Selva.Core.Helpers.LoginHelpers.LoginHelper("mp8hDB_HfbctBg", "UCIGqKPDABnjb0XtMh0Q_LhrNks");
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private Post selectedPost;

        public PostCommentPage()
        {
            InitializeComponent();

            commentTree = new ObservableCollection<Thing>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var accessToken = localSettings.Values["access_token"].ToString();
            var result = await loginHelper.Login_Refresh((string) localSettings.Values["refresh_token"]);
            //   localSettings.Values["refresh_token"] = result.RefreshToken;
            localSettings.Values["access_token"] = result.AccessToken;
            TokenSharpData.Reddit = new Selva.Core.Reddit(result.AccessToken);
            await TokenSharpData.Reddit.InitOrUpdateUserAsync();
            var s = TokenSharpData.Reddit.User.GetUsernameMentions();
            selectedPost = await TokenSharpData.Reddit.GetPostAsync(new Uri(e.Parameter.ToString()));
            List<Thing> commentsWithMores = null;
            try
            {
                commentsWithMores = await selectedPost.GetCommentsWithMoresAsync(100);
            }
            catch
            {
            }

            foreach (var comment in commentsWithMores)
                if (comment is Comment) //Flatten tree for list
                {
                    var flatTree = new Stack<Thing>();
                    DepthFirstTraversal(comment as Comment, flatTree);
                    flatTree.Reverse().ToList().ForEach(x => commentTree.Add(x));
                }
                else if (comment is More)
                {
                    commentTree.Add(comment);
                }

            /*  Parallel.ForEach(commentsWithMores, comment =>
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
              }*/
            LoadingControl.IsLoading = false;
        }

        public void DepthFirstTraversal(Comment root, Stack<Thing> stack)
        {
            stack.Push(root);
            /*   foreach (Thing thing in root.Comments)
               {
                   if (thing is Comment)
                   {
                       DepthFirstTraversal(thing as Comment, stack);
                   }
                   else if (thing is More)
                   {
                       stack.Push(thing);
                   }
               }*/
            Parallel.ForEach(root.Comments, thing =>
            {
                if (thing is Comment)
                    DepthFirstTraversal(thing as Comment, stack);
                else if (thing is More) stack.Push(thing);
            });
        }

        public static string GetLoadMoreCommentsString(string[] children)
        {
            return "Load " + children.Length + " more comments";
        }

        private async void MarkdownText_ImageClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out var link)) await Launcher.LaunchUriAsync(link);
        }

        private async void MarkdownText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out var link)) await Launcher.LaunchUriAsync(link);
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
                var tempList = new List<Thing>();

                /*  foreach (var comment in comments)
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
                   }*/
                Parallel.ForEach(comments, comment =>
                {
                    if (comment is Comment) //Flatten tree for listview
                    {
                        var flatTree = new Stack<Thing>();
                        DepthFirstTraversal(comment as Comment, flatTree);
                        flatTree.Reverse().ToList().ForEach(x => tempList.Add(x));
                    }
                    else if (comment is More && (comment as More).Children.Count() > 0)
                    {
                        tempList.Add(comment);
                    }
                });
                if (tempList.Count == 0)
                {
                    //Remove the item and return
                    commentTree.RemoveAt(index);
                    return;
                }

                //Replace more item at index
                commentTree[index] = tempList[0];
                tempList.RemoveAt(0);
                index++;

                //Insert each item of templist at specific index
                tempList.ForEach(x =>
                {
                    commentTree.Insert(index, x);
                    index++;
                });
                /* Parallel.ForEach(tempList, x =>
                  {
                      commentTree.Insert(index, x);
                      index++;
                  }); */
            }
        }

        private async void ReplyBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var Comment = DataContext as Comment;
            await Comment.ReplyAsync(sender.Text);
        }

        private async void DistinguishEditButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var Comment = DataContext as Comment;
            try
            {
                await Comment.DistinguishAsync(ModeratableThing.DistinguishType.Moderator);
            }
            catch
            {
            }
        }

        private async void saveEditButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var Comment = DataContext as Comment;
            try
            {
                await Comment.SaveAsync();
            }
            catch
            {
            }
        }

        private async void UnsaveEditButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var Comment = DataContext as Comment;
            try
            {
                await Comment.UnsaveAsync();
            }
            catch
            {
            }
        }

        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var Comment = DataContext as Comment;
            try
            {
                await Comment.RemoveAsync();
            }
            catch
            {
            }
        }

        private async void RemoveAsSpamButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var Comment = DataContext as Comment;
            try
            {
                await Comment.RemoveSpamAsync();
            }
            catch
            {
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var Comment = DataContext as Comment;
            try
            {
                await Comment.DelAsync();
            }
            catch
            {
            }
        }

        private async void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            var SenderFramework = (FrameworkElement) sender;
            var DataContext = SenderFramework.DataContext;
            var Comment = DataContext as Comment;
            try
            {
                await Comment.ApproveAsync();
            }
            catch
            {
            }
        }
    }
}
