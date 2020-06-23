using Reddit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Eco_Reddit.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InboxHubPage : Page
    {
        public string appId = "mp8hDB_HfbctBg";
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public InboxHubPage()
        {
            this.InitializeComponent();
            string refreshToken = localSettings.Values["refresh_token"].ToString();
            // Gets items from the collection according to pageIndex and pageSize parameters.
            var reddit = new RedditClient(appId, refreshToken, secret);
            if(reddit.Account.Me.InboxCount > 0 )
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
            NavigationViewItem navitem = nvInbox.SelectedItem as NavigationViewItem;
                string content = navitem.Content.ToString();

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
