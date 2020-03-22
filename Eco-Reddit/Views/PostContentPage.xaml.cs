using Microsoft.Toolkit.Uwp.UI.Controls;
using Reddit;
using Reddit.Controllers;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Eco_Reddit.Views
{
    public sealed partial class PostContentPage : Page, INotifyPropertyChanged
    {
        public string appId = "mp8hDB_HfbctBg";
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public static Post Post { get; set; }
        public static PostContentPage SingletonReference { get; set; }
        public PostContentPage()
        {
            InitializeComponent();
            StartUp();
            SingletonReference = this;
        }
        public async void StartUp()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await Task.Delay(500);
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                var Client = new RedditClient(appId, refreshToken, secret);
                Title.Text = Post.Title;
                Date.Text = "Created: " + Post.Created;
                PostAuthor.Content = "u/" + Post.Author;
                Comment.Label = "Comments: " + Post.Comments.GetComments("new").Count.ToString();
                PostSub.Content = "r/" + Post.Subreddit;
                UpvoteButton.Label = Post.UpVotes.ToString();
                UpvoteButton.IsChecked = Post.IsUpvoted;
                DownVoteButton.IsChecked = Post.IsDownvoted;
                Total.Text = "Total Awards: " + Post.Awards.Count.ToString();
                Gold.Text = "Platinum Awards: " + Post.Awards.Platinum.ToString();
                Silver.Text = "Gold Awards: " + Post.Awards.Gold.ToString();
                Bronze.Text = "Silver Awards: " + Post.Awards.Silver.ToString();
                if(Post.NSFW == true)
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
                    var p = Post as LinkPost;
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
                        Flair.Text = Post.Listing.LinkFlairText;
                    }
                    catch
                    {
                        UnloadObject(Flair);
                    }
                 }
                 catch
                 {

                     var p = Post as SelfPost;
                     PostText.Text = p.SelfText;
                     PostText.Visibility = Windows.UI.Xaml.Visibility.Visible;
                     LinkView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                       MediaRedditPlayer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    Image.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    Link.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    try
                     {
                         Flair.Text = Post.Listing.LinkFlairText;
                     }
                     catch
                     {
                         Flair.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                     }
                 }
                Post = null;
            });
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
