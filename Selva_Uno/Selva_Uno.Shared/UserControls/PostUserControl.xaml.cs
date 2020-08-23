using Selva.Views;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Reddit;
using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WinUI = Microsoft.UI.Xaml.Controls;
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Selva.UserControls
{
    public sealed partial class PostUserControl : UserControl
    {
        public static readonly DependencyProperty PostItemProperty =
   DependencyProperty.Register(
   "PostItem",
   typeof(Post),
   typeof(PostUserControl), null
   );
        public Post PostItem
        {
            get { return (Post)GetValue(PostItemProperty); }
            set { SetValue(PostItemProperty, (Post)value); }
        }
        public string appId = "mp8hDB_HfbctBg";
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";

        public PostUserControl()
        {
            this.InitializeComponent();

        }
        public async void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            await PostItem.ReportAsync(violatorUsername: PostItem.Author, reason: Reason.Text, ruleReason: RuleReason.Text, banEvadingAccountsNames: PostItem.Author, siteReason: SiteReason.Text, additionalInfo: AdditionalInfo.Text, customText: Reason.Text, otherReason: OtherInfo.Text, fromHelpCenter: false);
        }
        public async void CrosspostButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (PostItem.Listing.IsSelf == true)
                {
                    var newSelfPost = (PostItem as SelfPost).About().XPostToAsync(CrossPostText.Text);
                }
                else
                {
                    var newSelfPost = (PostItem as LinkPost).About().XPostToAsync(CrossPostText.Text);
                }
            }
            catch
            {
                return;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            var templateRoot = rp as RelativePanel;
            var img = templateRoot.Children[11] as Image;
            var Text = templateRoot.Children[9] as MarkdownTextBlock;

            //Downvoted.IsChecked = PostItem.IsDownvoted;
            try
            {
                var s = (SelfPost)PostItem;
                if (s.SelfText.Length < 800)
                    Text.Text = s.SelfText;
                else
                    Text.Text = s.SelfText.Substring(0, 800) + "...";
            }
            catch
            {
            }

            //Downvoted.IsChecked = PostItem.IsDownvoted;
            try
            {
                var p = PostItem as LinkPost;
                var bit = new BitmapImage();
                bit.UriSource = new Uri(p.URL);
                img.Source = bit;
                img.Visibility = Visibility.Visible;
            }
            catch
            {
                img.Visibility = Visibility.Collapsed;
            }
            var dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
        }

        public async void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(500);
            DataTransferManager.ShowShareUI();
        }
        public async void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                var PostUser = reddit.User(PostItem.Author);
                UserTemporaryInfo.PostUser = PostUser;
                var f = sender as Frame;
                f.Navigate(typeof(UserTemporaryInfo));
            });
        }
        public async void Frame_LoadedSubreddit(object sender, RoutedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                SubredditTemporaryInfo.InfoSubReddit = reddit.Subreddit(PostItem.Subreddit);
                var f = sender as Frame;
                f.Navigate(typeof(SubredditTemporaryInfo));
            });
        }
        public async void OpenPostInWebButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://www.reddit.com/r/" + PostItem.Subreddit + "/comments/" + PostItem.Id));
        }
        public void NewTabButton_Click(object sender, RoutedEventArgs e)
        {
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource { Symbol = Symbol.Document };
            newTab.Header = PostItem.Title;
            var frame = new Frame();
            newTab.Content = frame;
            PostContentPage.Post = PostItem;
            frame.Navigate(typeof(PostContentPage));
            //  PostContentPage.SingletonReference.StartUp();
            HomePage.MainTab.TabItems.Add(newTab);
            // }
        }

        public void HyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var h = sender as HyperlinkButton;
            var s = h.Content.ToString().Replace("u/", "");
            var newTab = new WinUI.TabViewItem();
            var refreshToken = localSettings.Values["refresh_token"].ToString();
            newTab.IconSource = new WinUI.SymbolIconSource { Symbol = Symbol.Document };
            var reddit = new RedditClient(appId, refreshToken, secret);
            var u = reddit.User(s);
            newTab.Header = "u/" + s;
            var frame = new Frame();
            newTab.Content = frame;
            UserHomePage.PostUser = reddit.User(s);
            frame.Navigate(typeof(UserHomePage));
            HomePage.MainTab.TabItems.Add(newTab);
            HomePage.MainTab.SelectedItem = newTab;
        }

        public void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;
            request.Data.SetText("https://www.reddit.com/r/" + PostItem.Subreddit + "/comments/" + PostItem.Id);
            request.Data.Properties.Title = PostItem.Title;
        }


        public void Award_Loaded(object sender, RoutedEventArgs e)
        {
            var AwardFrame = sender as Frame;
            AwardsFlyoutFrame.post = PostItem;
            AwardFrame.Navigate(typeof(AwardsFlyoutFrame));

        }
        public void Subreddit_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var h = sender as HyperlinkButton;
            var s = h.Content.ToString().Replace("r/", "");
            HomePage.SingletonReference.NavigateJumper(s);
        }


        public async void HideButton_Click(object sender, RoutedEventArgs e)
        {

            await PostItem.HideAsync();
        }



        public async void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            await PostItem.SaveAsync("");
        }


        public async void MarkdownText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out var link)) await Launcher.LaunchUriAsync(link);
        }


        public async void MarkdownText_ImageClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out var link)) await Launcher.LaunchUriAsync(link);
        }

        public void EditButton_Click(object sender, RoutedEventArgs e)
        {

        }

        public async void DeleteButtonPost_Click(object sender, RoutedEventArgs e)
        {

            await PostItem.DeleteAsync();
        }

        public async void DistinguishButton_Click(object sender, RoutedEventArgs e)
        {

            await PostItem.DistinguishAsync("yes");
        }


        public async void StickyButton_Click(object sender, RoutedEventArgs e)
        {

            await PostItem.SetSubredditStickyAsync(1, false);
        }

        public async void UnHideEditButton_Click(object sender, RoutedEventArgs e)
        {

            await PostItem.UnhideAsync();
        }

        public async void UnSaveditButton_Click(object sender, RoutedEventArgs e)
        {

            await PostItem.UnsaveAsync();
        }



        public async void RemoveEditButton_Click(object sender, RoutedEventArgs e)
        {
            await PostItem.RemoveAsync();
        }

        public async void UnStickyEditButton_Click(object sender, RoutedEventArgs e)
        {
            await PostItem.UnsetSubredditStickyAsync(1, false);
        }

        public async void SpoilerEditButton_Click(object sender, RoutedEventArgs e)
        {
            await PostItem.SpoilerAsync();
        }

        public async void UnSpoilerEditButton_Click(object sender, RoutedEventArgs e)
        {
            await PostItem.UnspoilerAsync();
        }

        public async void NSFWEditButton_Click(object sender, RoutedEventArgs e)
        {
            await PostItem.MarkNSFWAsync();
        }

        public async void UNNSFWEditButton_Click(object sender, RoutedEventArgs e)
        {

            await PostItem.UnmarkNSFWAsync();
        }

        public async void LockEditButton_Click(object sender, RoutedEventArgs e)
        {


            await PostItem.LockAsync();
        }

        public async void UnlockEditButton_Click(object sender, RoutedEventArgs e)
        {


            await PostItem.UnlockAsync();
        }

        public async void PermaLinkButton_Click(object sender, RoutedEventArgs e)
        {


            var pl = PostItem.Permalink;
            var dataPackage = new DataPackage();
            dataPackage.SetText("https://www.reddit.com" + pl);
            Clipboard.SetContent(dataPackage);
        }

    }
}
