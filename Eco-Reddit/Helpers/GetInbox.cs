using Eco_Reddit.Models;
using Eco_Reddit.Views;
using Microsoft.Toolkit.Collections;
using Reddit;
using Reddit.Controllers;
using Reddit.Inputs;
using Reddit.Things;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using static Eco_Reddit.Views.HomePage;

namespace Eco_Reddit.Helpers
{
    public class GetInboxClass : IIncrementalSource<Inbox>
    {
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public static int limit = 10;
        public static int skipInt = 0;
        public string appId = "mp8hDB_HfbctBg";
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        List<Inbox> MessageCollection;
        private IEnumerable<Message> Messages;
        public async Task<IEnumerable<Inbox>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Run(async () =>
            {

                string refreshToken = localSettings.Values["refresh_token"].ToString();
                MessageCollection = new List<Inbox>();
                var reddit = new RedditClient(appId, refreshToken, secret);
                Messages = reddit.Account.Messages.GetMessagesInbox();
                await Task.Run(() =>
                {
                    foreach (Message message in Messages)
                    {
                        MessageCollection.Add(new Inbox()
                        {
                            InboxSelf = message
                        });
                    }
                });
                // Simulates a longer request...
            });
            return MessageCollection;
        }
    }
}
