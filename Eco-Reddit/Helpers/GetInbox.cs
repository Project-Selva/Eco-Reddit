using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Selva.Core.Models;
using Microsoft.Toolkit.Collections;
using Reddit;
using Reddit.Things;

namespace Selva.Helpers
{
    public class GetInboxClass : IIncrementalSource<Inbox>
    {
        public static int limit = 10;
        public static int skipInt;
        public string appId = "mp8hDB_HfbctBg";
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private List<Inbox> MessageCollection;
        private IEnumerable<Message> Messages;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public string thing;

        public async Task<IEnumerable<Inbox>> GetPagedItemsAsync(int pageIndex, int pageSize,
            CancellationToken cancellationToken = default)
        {
            await Task.Run(async () =>
            {
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                MessageCollection = new List<Inbox>();
                var reddit = new RedditClient(appId, refreshToken, secret);
                Messages = reddit.Account.Messages.GetMessagesInbox(limit: limit).Skip(skipInt);
                await Task.Run(() =>
                {
                    foreach (var message in Messages)
                    {
                        MessageCollection.Add(new Inbox
                        {
                            InboxSelf = message
                        });
                        thing = message.Fullname;
                    }
                });
                // Simulates a longer request...
                skipInt = skipInt + 10;
                limit = limit + 10;
            });
            return MessageCollection;
        }
    }
}
