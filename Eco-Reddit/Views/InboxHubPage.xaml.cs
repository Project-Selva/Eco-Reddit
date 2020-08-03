using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Reddit;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Eco_Reddit.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InboxHubPage : Page
    {
        public string appId = "mp8hDB_HfbctBg";
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";

        public InboxHubPage()
        {
            InitializeComponent();
            var refreshToken = localSettings.Values["refresh_token"].ToString();
            // Gets items from the collection according to pageIndex and pageSize parameters.
            var reddit = new RedditClient(appId, refreshToken, secret);
            if (reddit.Account.Me.InboxCount > 0)
            {
                nvInbox.SelectedItem = Unread;
                contentFrame.Navigate(typeof(PrivateMessagesUnread));
            }
            else
            {
                nvInbox.SelectedItem = All;
                contentFrame.Navigate(typeof(InboxPage));
            }
        }

        private void NvInbox_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var navitem = nvInbox.SelectedItem as NavigationViewItem;
            var content = navitem.Content.ToString();

            switch (content)
            {
                case "All":
                    //    contentFrame.Navigate(typeof(InboxPage));
                    break;
                case "Unread":
                    contentFrame.Navigate(typeof(PrivateMessagesUnread));
                    break;

                case "Mod news letter":
                    contentFrame.Navigate(typeof(ModNewsLetter));
                    break;

                case "User mentions":
                    contentFrame.Navigate(typeof(UserMentions));
                    break;
                case "Sent messages":
                    contentFrame.Navigate(typeof(SentMessages));
                    break;

                case "Post replies":
                    contentFrame.Navigate(typeof(PostReplies));
                    break;
                case "Messages":
                    contentFrame.Navigate(typeof(Recievedmessages));
                    break;
                case "Comment replies":
                    contentFrame.Navigate(typeof(CommentReplies));
                    break;
            }
        }
    }
}
