using System;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Eco_Reddit.Helpers;
using Eco_Reddit.Core.Models;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Reddit;
using Reddit.Things;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Eco_Reddit.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InboxPage : Page
    {
        public string appId = "mp8hDB_HfbctBg";
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";

        public InboxPage()
        {
            InitializeComponent();
            GetInboxClass.skipInt = 0;
            GetInboxClass.limit = 10;
            var inbox = new IncrementalLoadingCollection<GetInboxClass, Inbox>();
            InboxList.ItemsSource = inbox;
        }

        private void InboxList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase != 0) throw new Exception("We should be in phase 0, but we are not.");

            // It's phase 0, so this item's title will already be bound and displayed.

            args.RegisterUpdateCallback(ShowPhase1);

            args.Handled = true;
        }

        private async void ShowPhase1(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase != 1) throw new Exception("We should be in phase 1, but we are not.");
            LoadingControl.IsLoading = false;
            var SenderMessage = args.Item as Inbox;
            var Message = SenderMessage.InboxSelf;
            var templateRoot = args.ItemContainer.ContentTemplateRoot as RelativePanel;
            var textBlock = templateRoot.Children[3] as HyperlinkButton;
            var AuthorBlock = templateRoot.Children[2] as HyperlinkButton;
            var TextDateBlock = templateRoot.Children[1] as TextBlock;
            var TextTitleBlock = templateRoot.Children[0] as MarkdownTextBlock;
            TextTitleBlock.Text = Message.Body;
            textBlock.Content = Message.Subreddit;
            AuthorBlock.Content = Message.Author;
            TextDateBlock.Text = "Created: " + Message.CreatedUTC;
        }

        private async void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var Frames = (Frame) sender;
                var pp = Frames.Tag as Message;
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                var PostUser = reddit.User(pp.Author);
                UserTemporaryInfo.PostUser = PostUser;
                var f = sender as Frame;
                f.Navigate(typeof(UserTemporaryInfo));
            });
        }

        private void HyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement) sender);
        }

        private async void Frame_LoadedSubreddit(object sender, RoutedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var Frames = (Frame) sender;
                var pp = Frames.Tag as Message;
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                SubredditTemporaryInfo.InfoSubReddit = reddit.Subreddit(pp.Subreddit);
                var f = sender as Frame;
                f.Navigate(typeof(SubredditTemporaryInfo));
            });
        }
    }
}
