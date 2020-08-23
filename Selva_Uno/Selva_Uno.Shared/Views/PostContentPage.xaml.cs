using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Media.Core;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Reddit;
using Reddit.Controllers;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace Selva.Views
{
    public sealed partial class PostContentPage : Page, INotifyPropertyChanged
    {
        public static Post Post;
        public static PostContentPage SingletonReference;
        public string appId = "mp8hDB_HfbctBg";
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public Post PostLocal;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";

        public PostContentPage()
        {
            InitializeComponent();
            PostLocal = Post;
            var dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            SingletonReference = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void MarkdownText_ImageClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out var link)) await Launcher.LaunchUriAsync(link);
        }

        private async void MarkdownText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out var link)) await Launcher.LaunchUriAsync(link);
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;
            request.Data.SetText("https://www.reddit.com/r/" + PostLocal.Subreddit + "/comments/" + PostLocal.Id);
            request.Data.Properties.Title = PostLocal.Title;
        }

        public async void StartUp(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    if ((bool) localSettings.Values["AdEnabled"] && (bool) localSettings.Values["PostAdEnabled"])
                    {
                        var r = new Random();
                        if (r.Next(100) < (double) localSettings.Values["PostAdFrequency"])
                        {
                        }
                        else
                        {
                            vungleBannerControl.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        vungleBannerControl.Visibility = Visibility.Collapsed;
                    }

                    var refreshToken = localSettings.Values["refresh_token"].ToString();
                    var Client = new RedditClient(appId, refreshToken, secret);
                    Title.Text = PostLocal.Title;
                    Date.Text = "Created: " + PostLocal.Created;
                    User.Content = "u/" + PostLocal.Author;
                    Commentcount.Label = PostLocal.Comments.GetComments().Count + " Comments";
                    Subreddit.Content = "r/" + PostLocal.Subreddit;
                    UpvoteButton.Label = PostLocal.UpVotes.ToString();
                    UpvoteButton.IsChecked = PostLocal.IsUpvoted;
                    DownVoteButton.IsChecked = PostLocal.IsDownvoted;
                    Total.Text = "Total Awards: " + PostLocal.Awards.Count;
                    Gold.Text = "Platinum Awards: " + PostLocal.Awards.Platinum;
                    Silver.Text = "Gold Awards: " + PostLocal.Awards.Gold;
                    Bronze.Text = "Silver Awards: " + PostLocal.Awards.Silver;

                    if (PostLocal.NSFW)
                        NSFW.Text = "NSFW";
                    else
                        NSFW.Visibility = Visibility.Collapsed;
                    try
                    {
                        PostText.Visibility = Visibility.Collapsed;
                        var p = PostLocal as LinkPost;
                        if (p.URL.Contains("i.redd.it") || p.URL.Contains("i.imgur"))
                        {
                            var img = new BitmapImage();
                            img.UriSource = new Uri(p.URL);
                            Image.Source = img;
                            Link.NavigateUri = new Uri(p.URL);
                            Image.Visibility = Visibility.Visible;
                            LinkView.Visibility = Visibility.Collapsed;
                        }
                        else if (p.URL.Contains("v.redd.it"))
                        {
                            MediaRedditPlayer.Visibility = Visibility.Visible;
                            MediaRedditPlayer.Source = MediaSource.CreateFromUri(new Uri(p.URL + "/DASHPlaylist.mpd"));
                            Image.Visibility = Visibility.Collapsed;
                            LinkView.Visibility = Visibility.Collapsed;
                        }
                        else if (p.URL.Contains("gfycat.com"))
                        {
                            Image.Visibility = Visibility.Collapsed;
                            LinkView.Visibility = Visibility.Visible;
                            MediaRedditPlayer.Visibility = Visibility.Collapsed;
                            LinkView.Navigate(new Uri("ms-appx-web:////Core/WebFiles/FramePlayer.html"));
                            ///current string of p.url is example https://gfycat.com/imaginaryfreeeuropeanfiresalamander but we need https://gfycat.com/ifr/imaginaryfreeeuropeanfiresalamander
                            var gfystringID = p.URL.Remove(0, 19);
                            var GfyCatVideo = "https://gfycat.com/ifr/" + gfystringID;

                            await Task.Delay(300);
                            await LinkView.InvokeScriptAsync("eval",
                                new[] {"document.getElementById('IframePlayer').src = '" + GfyCatVideo + "';"});
                            //   string functionString = "document.getElementById('IframePlayer').src = '" + p.URL + "';";
                            //  await LinkView.InvokeScriptAsync("eval", new string[] { functionString };
                        }
                        else
                        {
                            LinkView.Source = new Uri(p.URL);
                            Image.Visibility = Visibility.Collapsed;
                            LinkView.Visibility = Visibility.Visible;
                            MediaRedditPlayer.Visibility = Visibility.Collapsed;
                        }

                        try
                        {
                            Flair.Text = PostLocal.Listing.LinkFlairText;
                        }
                        catch
                        {
                            UnloadObject(Flair);
                        }
                    }
                    catch
                    {
                        var p = PostLocal as SelfPost;
                        PostText.Text = p.SelfText;
                        PostText.Visibility = Visibility.Visible;
                        LinkView.Visibility = Visibility.Collapsed;
                        MediaRedditPlayer.Visibility = Visibility.Collapsed;
                        Image.Visibility = Visibility.Collapsed;
                        Link.Visibility = Visibility.Collapsed;
                        try
                        {
                            Flair.Text = PostLocal.Listing.LinkFlairText;
                        }
                        catch
                        {
                            Flair.Visibility = Visibility.Collapsed;
                        }
                    }

                    LoadingControl.Visibility = Visibility.Collapsed;
                    await Task.Delay(100);
                    CommentCount.Text = PostLocal.Comments.GetComments().Count + " Comments";
                    //    string rep = Post.Title;
                    //     string dash = rep.Replace("", "_");
                    CommentFrame.Navigate(typeof(PostCommentPage),
                        "https://www.reddit.com/r/" + Post.Subreddit + "/comments/" + Post.Id + "/");
                });
            });
        }

        private async void EnableRelpyEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var CommentLocal = AppBarButtonObject.Tag as Comment;
            await CommentLocal.EnableSendRepliesAsync();
        }

        private async void disableRelpyEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var CommentLocal = AppBarButtonObject.Tag as Comment;
            await CommentLocal.DisableSendRepliesAsync();
        }

        private async void RemoveComEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var CommentLocal = AppBarButtonObject.Tag as Comment;
            await CommentLocal.RemoveAsync();
        }

        private async void DeleteCommentButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var CommentLocal = AppBarButtonObject.Tag as Comment;
            await CommentLocal.DeleteAsync();
        }

        private async void DistinguishEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var CommentLocal = AppBarButtonObject.Tag as Comment;
            await CommentLocal.DistinguishAsync("yes");
        }

        private async void saveEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var CommentLocal = AppBarButtonObject.Tag as Comment;
            await CommentLocal.SaveAsync("");
        }

        private async void UnsaveEditButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var CommentLocal = AppBarButtonObject.Tag as Comment;
            await CommentLocal.UnsaveAsync();
        }

        private async void PermaLinkCommentButton_Click(object sender, RoutedEventArgs e)
        {
            var AppBarButtonObject = (AppBarButton) sender;
            var CommentLocal = AppBarButtonObject.Tag as Comment;
            var pl = CommentLocal.Permalink;
            var dataPackage = new DataPackage();
            dataPackage.SetText("https://www.reddit.com" + pl);
            Clipboard.SetContent(dataPackage);
        }

        private void CommentList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase != 0) throw new Exception("We should be in phase 0, but we are not.");

            // It's phase 0, so this item's title will already be bound and displayed.

            args.RegisterUpdateCallback(ShowPhase1);

            args.Handled = true;
        }

        private async void ShowPhase1(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase != 1) throw new Exception("We should be in phase 1, but we are not.");
            try
            {
                var SenderComment = args.Item as Selva.Core.Models.Comments;
                var comment = SenderComment.CommentSelf;
                var templateRoot = args.ItemContainer.ContentTemplateRoot as RelativePanel;
                var textBlock = templateRoot.Children[2] as MarkdownTextBlock;
                var DatetextBlock = templateRoot.Children[3] as TextBlock;
                DatetextBlock.Text = comment.Created.ToString();
                var AuthortextBlock = templateRoot.Children[4] as HyperlinkButton;
                AuthortextBlock.Content = comment.Author;
                var Upvoted = templateRoot.Children[0] as AppBarToggleButton;
                var Downvoted = templateRoot.Children[1] as AppBarToggleButton;
                Upvoted.Label = comment.UpVotes.ToString();
                Upvoted.IsChecked = comment.IsUpvoted;
                Downvoted.IsChecked = comment.IsDownvoted;
                var t = new Thickness();
                t.Left = comment.Depth * 100;
                templateRoot.Margin = t;
                try
                {
                    textBlock.Text = comment.Body;
                }
                catch
                {
                }
            }
            catch
            {
                var SenderComment = args.Item as Selva.Core.Models.Comments;
                var comment = SenderComment.CommentSelf;
                var templateRoot = args.ItemContainer.ContentTemplateRoot as RelativePanel;
                var textBlock = templateRoot.Children[2] as MarkdownTextBlock;
                var DatetextBlock = templateRoot.Children[3] as TextBlock;
                DatetextBlock.Text = comment.Created.ToString();
                var AuthortextBlock = templateRoot.Children[4] as HyperlinkButton;
                AuthortextBlock.Content = comment.Author;
                var Upvoted = templateRoot.Children[0] as AppBarToggleButton;
                var Downvoted = templateRoot.Children[1] as AppBarToggleButton;
                Upvoted.Label = comment.Listing.Likes.ToString();
                //    Upvoted.IsChecked = comment.Score.ToString();
                // Downvoted.IsChecked = comment.IsDownvoted;
                var t = new Thickness();
                t.Left = comment.Depth * 100;
                templateRoot.Margin = t;
                try
                {
                    textBlock.Text = comment.Body;
                }
                catch
                {
                }
            }
        }

        private async void HideButton_Click(object sender, RoutedEventArgs e)
        {
            var Comment = PostLocal;
            await Comment.HideAsync();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var Comment = PostLocal;
            await Comment.SaveAsync("");
        }

        private async void OpenPostInWebButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://www.reddit.com/r/" + PostLocal.Subreddit + "/comments/" +
                                                  PostLocal.Id));
        }

        private async void Frame_LoadedSubreddit(object sender, RoutedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                SubredditTemporaryInfo.InfoSubReddit = reddit.Subreddit(PostLocal.Subreddit);
                var f = sender as Frame;
                f.Navigate(typeof(SubredditTemporaryInfo));
            });
        }

        private async void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                var PostUser = reddit.User(PostLocal.Author);
                UserTemporaryInfo.PostUser = PostUser;
                var f = sender as Frame;
                f.Navigate(typeof(UserTemporaryInfo));
            });
        }

        private void Subreddit_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement) sender);
        }

        private void HyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement) sender);
        }

        private void NewTabButton_Click(object sender, RoutedEventArgs e)
        {
            /* var SenderFramework = (FrameworkElement)e.OriginalSource;
             var DataContext = SenderFramework.DataContext;
         var Comment = DataContext as Posts;
         /*  if (Comment == null)*/
            /* {
                 return;
             }
             else
             {*/
            // Reddit.Controllers.Comment Comment = Post.PostSelf;
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource {Symbol = Symbol.Document};
            newTab.Header = PostLocal.Title;
            var frame = new Frame();
            newTab.Content = frame;
            frame.Navigate(typeof(PostContentPage));
            Post = PostLocal;
            //  PostContentPage.SingletonReference.StartUp();
            HomePage.MainTab.TabItems.Add(newTab);
            // }
        }

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return;

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private async void CrosspostButton_Click(object sender, RoutedEventArgs e)
        {
            // try
            // {
            if (PostLocal.Listing.IsSelf)
            {
                var newSelfComment = (PostLocal as SelfPost).About().XPostToAsync(CrosspsotText.Text);
            }
            else
            {
                var newSelfComment = (PostLocal as LinkPost).About().XPostToAsync(CrosspsotText.Text);
            }

            /* }
             catch
             {
                 return;
             }*/
        }

        private async void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            await PostLocal.ReportAsync(violatorUsername: PostLocal.Author, reason: Reason.Text,
                ruleReason: RuleReason.Text, banEvadingAccountsNames: PostLocal.Author, siteReason: SiteReason.Text,
                additionalInfo: AdditionalInfo.Text, customText: Reason.Text, otherReason: OtherInfo.Text,
                fromHelpCenter: false);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            //  PostLocal.set
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            await PostLocal.DeleteAsync();
        }

        private async void DistinguishButton_Click(object sender, RoutedEventArgs e)
        {
            await PostLocal.DistinguishAsync("yes");
        }

        private async void StickyButton_Click(object sender, RoutedEventArgs e)
        {
            await PostLocal.SetSubredditStickyAsync(1, false);
        }

        private async void UnHideEditButton_Click(object sender, RoutedEventArgs e)
        {
            await PostLocal.UnhideAsync();
        }

        private async void UnSaveditButton_Click(object sender, RoutedEventArgs e)
        {
            await PostLocal.UnsaveAsync();
        }

        private async void RemoveEditButton_Click(object sender, RoutedEventArgs e)
        {
            await PostLocal.RemoveAsync();
        }

        private async void UnStickyEditButton_Click(object sender, RoutedEventArgs e)
        {
            await PostLocal.UnsetSubredditStickyAsync(1, false);
        }

        private async void SpoilerEditButton_Click(object sender, RoutedEventArgs e)
        {
            await PostLocal.SpoilerAsync();
        }

        private async void UnSpoilerEditButton_Click(object sender, RoutedEventArgs e)
        {
            await PostLocal.UnspoilerAsync();
        }

        private async void NSFWEditButton_Click(object sender, RoutedEventArgs e)
        {
            await PostLocal.MarkNSFWAsync();
        }

        private async void UNNSFWEditButton_Click(object sender, RoutedEventArgs e)
        {
            await PostLocal.UnmarkNSFWAsync();
        }

        private async void LockEditButton_Click(object sender, RoutedEventArgs e)
        {
            await PostLocal.LockAsync();
        }

        private async void UnlockEditButton_Click(object sender, RoutedEventArgs e)
        {
            await PostLocal.UnlockAsync();
        }

        private async void PermaLinkButton_Click(object sender, RoutedEventArgs e)
        {
            var pl = PostLocal.Permalink;
            var dataPackage = new DataPackage();
            dataPackage.SetText("https://www.reddit.com" + pl);
            Clipboard.SetContent(dataPackage);
        }

        private void CommentBox_QuerySubmitted(object sender, RoutedEventArgs e)
        {
            string RichText;
            CommentZone.TextDocument.GetText(TextGetOptions.None, out RichText);
            PostLocal.ReplyAsync(RichText);
        }

        private async void SortMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            /*  MenuFlyoutItem Sort = sender as MenuFlyoutItem;
              GetComments.SortOrder = Sort.Text;
              SortOrderCommentButton.Label = Sort.Text;
              GetComments.limit = 100;
              GetComments.skipInt = 0;
              GetComments.PostToGetCommentsFrom = PostLocal;
             // var CommentsCollection = new IncrementalLoadingCollection<GetComments, Selva.Models.Comments>();

            //  CommentList.ItemsSource = CommentsCollection;
              await Task.Delay(500);
              CommentCount.Text = "Comments: " + PostLocal.Comments.GetComments("new").Count.ToString();*/
        }

        private void ReplyBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var CommentLocal = sender.Tag as Comment;
            CommentLocal.ReplyAsync(sender.Text);
        }

        /* private async void LOADCOM_Click(object sender, RoutedEventArgs e)
         {

                 GetComments.SortOrder = "Top";
                 GetComments.limit = PostLocal.Comments.GetComments("Top").Count;
                 GetComments.skipInt = 0;
                 lOADCOM.Visibility = Visibility.Collapsed;
                 GetComments.PostToGetCommentsFrom = PostLocal;
                 var CommentsCollection = new IncrementalLoadingCollection<GetComments, Selva.Models.Comments>();
                 await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                 {
                     CommentList.ItemsSource = CommentsCollection;
                     SortOrderCommentButton.Visibility = Visibility.Visible;
                 });

         }*/


        private void CommentZone_TextChanged(object sender, RoutedEventArgs e)
        {
            string RichText;
            CommentZone.TextDocument.GetText(TextGetOptions.None, out RichText);
            MarkDownBlock.Text = RichText;
        }
    }
}
