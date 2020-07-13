using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Reddit.Things;
using Reddit;
using Windows.UI.Core;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Toolkit.Uwp;
using Eco_Reddit.Helpers;
using Eco_Reddit.Models;
using System.Text.RegularExpressions;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Eco_Reddit.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserMentions : Page
    {
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public string appId = "mp8hDB_HfbctBg";
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public UserMentions()
        {
            this.InitializeComponent();
            GetUniversalMessagesClass.limit = 50;
            GetUniversalMessagesClass.skipInt = 0;
            GetUniversalMessagesClass.Type = "mention";
            string refreshToken = localSettings.Values["refresh_token"].ToString();
            var PrivateMessages = new IncrementalLoadingCollection<GetUniversalMessagesClass, PrivateMessage>();
            InboxList.ItemsSource = PrivateMessages;
            var reddit = new RedditClient(appId, refreshToken, secret);
        }
        private void InboxList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
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

            PrivateMessage SenderMessage = args.Item as PrivateMessage;
            Message Message = SenderMessage.MessageSelf;
            var templateRoot = args.ItemContainer.ContentTemplateRoot as RelativePanel;
            var textBlock = templateRoot.Children[3] as HyperlinkButton;
            var AuthorBlock = templateRoot.Children[2] as HyperlinkButton;
            var TextDateBlock = templateRoot.Children[1] as TextBlock;
            var TextTitleBlock = templateRoot.Children[0] as MarkdownTextBlock;
          ///  var converter = new ReverseMarkdown.Converter();
          //  TextTitleBlock.Text = converter.Convert(Message.BodyHTML.ToString());

                TextTitleBlock.Text = Regex.Replace(Message.BodyHTML.ToString(), @"<[^>]*>", "");
            
            textBlock.Content = Message.Subject;
            AuthorBlock.Content = Message.Author;
            TextDateBlock.Text = "Created: " + Message.CreatedUTC;
        }
        private async void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                Frame Frames = (Frame)sender;
                Message pp = (Frames).Tag as Message;
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                Reddit.Controllers.User PostUser = reddit.User(pp.Author);
                UserTemporaryInfo.PostUser = PostUser;
                Frame f = sender as Frame;
                f.Navigate(typeof(UserTemporaryInfo));
            });
        }

        private void HyperlinkButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private async void Frame_LoadedSubreddit(object sender, RoutedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                Frame Frames = (Frame)sender;
                Message pp = (Frames).Tag as Message;
                string refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                SubredditTemporaryInfo.InfoSubReddit = reddit.Subreddit(pp.Subreddit);
                Frame f = sender as Frame;
                f.Navigate(typeof(SubredditTemporaryInfo));
            });
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            string refreshToken = localSettings.Values["refresh_token"].ToString();
            var reddit = new RedditClient(appId, refreshToken, secret);
            reddit.Account.Messages.MarkAllReadAsync();
        }
    }
}
