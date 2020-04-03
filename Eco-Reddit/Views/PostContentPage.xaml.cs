using Eco_Reddit.Helpers;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Reddit;
using Reddit.Controllers;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.System;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace Eco_Reddit.Views
{
    public sealed partial class PostContentPage : Page, INotifyPropertyChanged
    {
        public string appId = "mp8hDB_HfbctBg";
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public static Post Post { get; set; }
        public Post PostLocal;
        public static PostContentPage SingletonReference { get; set; }
        public PostContentPage()
        {
            InitializeComponent();
            PostLocal = Post;
            SingletonReference = this;
        }
        public async void StartUp(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                var Client = new RedditClient(appId, refreshToken, secret);
                Title.Text = PostLocal.Title;
                Date.Text = "Created: " + PostLocal.Created;
                User.Content = "u/" + PostLocal.Author;
                Comment.Label = "Comments: " + PostLocal.Comments.GetComments("new").Count.ToString();
                Subreddit.Content = "r/" + PostLocal.Subreddit;
                UpvoteButton.Label = PostLocal.UpVotes.ToString();
                UpvoteButton.IsChecked = PostLocal.IsUpvoted;
                DownVoteButton.IsChecked = PostLocal.IsDownvoted;
                Total.Text = "Total Awards: " + PostLocal.Awards.Count.ToString();
                Gold.Text = "Platinum Awards: " + PostLocal.Awards.Platinum.ToString();
                Silver.Text = "Gold Awards: " + PostLocal.Awards.Gold.ToString();
                Bronze.Text = "Silver Awards: " + PostLocal.Awards.Silver.ToString();

                if(PostLocal.NSFW == true)
                {
                    NSFW.Text = "NSFW";
                }
                else
                {
                    NSFW.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
               try
                {
                    PostText.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    var p = PostLocal as LinkPost;
                if (p.URL.ToString().Contains("i.redd.it") == true || p.URL.ToString().Contains("i.imgur") == true)
                {
                    BitmapImage img = new BitmapImage();
                    img.UriSource = new Uri(p.URL);
                    Image.Source = img;
                    Link.NavigateUri = new Uri(p.URL);
                    Image.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    LinkView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                else if (p.URL.ToString().Contains("v.redd.it") == true )
                {
                    
                      MediaRedditPlayer.Visibility = Windows.UI.Xaml.Visibility.Visible;
                      MediaRedditPlayer.Source = MediaSource.CreateFromUri(new Uri(p.URL + "/DASHPlaylist.mpd"));
                      Image.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                      LinkView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                else if(p.URL.ToString().Contains("gfycat.com") == true)
                {
                    Image.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    LinkView.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    MediaRedditPlayer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    LinkView.Navigate(new Uri("ms-appx-web:///WebFiles/FramePlayer.html"));
                    ///current string of p.url is example https://gfycat.com/imaginaryfreeeuropeanfiresalamander but we need https://gfycat.com/ifr/imaginaryfreeeuropeanfiresalamander
                    string gfystringID = p.URL.ToString().Remove(0, 19);
                    string GfyCatVideo = "https://gfycat.com/ifr/" + gfystringID;

                    await Task.Delay(300);
                    await LinkView.InvokeScriptAsync("eval", new string[] { "document.getElementById('IframePlayer').src = '" + GfyCatVideo + "';" });
                    //   string functionString = "document.getElementById('IframePlayer').src = '" + p.URL + "';";
                    //  await LinkView.InvokeScriptAsync("eval", new string[] { functionString };
                }
                else
                {
                    LinkView.Source = new Uri(p.URL);
                    Image.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    LinkView.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    MediaRedditPlayer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
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
                     PostText.Visibility = Windows.UI.Xaml.Visibility.Visible;
                     LinkView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                       MediaRedditPlayer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    Image.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    Link.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    try
                     {
                         Flair.Text = PostLocal.Listing.LinkFlairText;
                     }
                     catch
                     {
                         Flair.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                     }
                 }
                LoadingControl.Visibility = Visibility.Collapsed;
               GetComments.SortOrder = "Top";
                GetComments.limit = 10;
                GetComments.skipInt = 0;
                GetComments.PostToGetCommentsFrom = PostLocal;
                var CommentsCollection = new IncrementalLoadingCollection<GetComments, Eco_Reddit.Models.Comments>();

                CommentList.ItemsSource = CommentsCollection;
            });
        }
        private void CommentList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {

            if (args.Phase != 0)
            {
                throw new System.Exception("We should be in phase 0, but we are not.");
            }

            // It's phase 0, so this item's title will already be bound and displayed.

            args.RegisterUpdateCallback(this.ShowPhase1);

            args.Handled = true;
        }

        private async void ShowPhase1(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase != 1)
            {
                throw new System.Exception("We should be in phase 1, but we are not.");
            }

            Eco_Reddit.Models.Comments SenderComment = args.Item as Eco_Reddit.Models.Comments;
            Reddit.Controllers.Comment comment = SenderComment.CommentSelf;
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
            try
            {
                textBlock.Text = comment.Body;
            }
            catch
            { }
        }
        private async void HideButton_Click(object sender, RoutedEventArgs e)
        {
            Reddit.Controllers.Post post = PostLocal;
            await post.HideAsync();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            Reddit.Controllers.Post post = PostLocal;
            await post.SaveAsync("");
        }
        private async void OpenPostInWebButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://www.reddit.com/r/" + PostLocal.Subreddit + "/comments/" + PostLocal.Id));
        }
        private async void Frame_LoadedSubreddit(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                SubredditTemporaryInfo.InfoSubReddit = reddit.Subreddit(PostLocal.Subreddit);
                Frame f = sender as Frame;
                f.Navigate(typeof(SubredditTemporaryInfo));
            });
        }
        private async void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                Reddit.Controllers.User PostUser = reddit.User(PostLocal.Author);
                UserTemporaryInfo.PostUser = PostUser;
                Frame f = sender as Frame;
                f.Navigate(typeof(UserTemporaryInfo));
            });
        }
        private void Subreddit_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
        private void HyperlinkButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
        private void NewTabButton_Click(object sender, RoutedEventArgs e)
        {

            /* var SenderFramework = (FrameworkElement)e.OriginalSource;
             var DataContext = SenderFramework.DataContext;
         var Post = DataContext as Posts;
         /*  if (Post == null)*/
            /* {
                 return;
             }
             else
             {*/
            // Reddit.Controllers.Post post = Post.PostSelf;
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Document };
            newTab.Header = PostLocal.Title;
            Frame frame = new Frame();
            newTab.Content = frame;
            frame.Navigate(typeof(PostContentPage));
            PostContentPage.Post = PostLocal;
            //  PostContentPage.SingletonReference.StartUp();
            HomePage.MainTab.TabItems.Add(newTab);
            // }
        }
        private async void MarkdownText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out Uri link))
            {
                await Launcher.LaunchUriAsync(link);
            }
        }
        private async void MarkdownText_ImageClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out Uri link))
            {
                await Launcher.LaunchUriAsync(link);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
