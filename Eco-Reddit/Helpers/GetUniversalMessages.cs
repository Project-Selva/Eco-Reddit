using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Eco_Reddit.Core.Models;
using Microsoft.Toolkit.Collections;
using Reddit;
using Reddit.Things;

namespace Eco_Reddit.Helpers
{
    public class GetUniversalMessagesClass : IIncrementalSource<PrivateMessage>
    {
        public static int limit = 10;
        public static int skipInt;
        public static string Type;
        public string appId = "mp8hDB_HfbctBg";
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private List<PrivateMessage> MessageCollection;
        private IEnumerable<Message> MessagesInbox;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public string thing;
        public string type;

        public async Task<IEnumerable<PrivateMessage>> GetPagedItemsAsync(int pageIndex, int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(type)) type = Type;
            await Task.Run(async () =>
            {
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                // Gets items from the collection according to pageIndex and pageSize parameters.
                MessageCollection = new List<PrivateMessage>();
                var reddit = new RedditClient(appId, refreshToken, secret);
                switch (type)
                {
                    case "Sent":
                        try
                        {
                            MessagesInbox = reddit.Account.Messages.GetMessagesSent(limit: limit).Skip(skipInt);


                            await Task.Run(() =>
                            {
                                foreach (var message in MessagesInbox)
                                {
                                    MessageCollection.Add(new PrivateMessage
                                    {
                                        MessageSelf = message
                                    });
                                    thing = message.Fullname;
                                }
                            });
                            skipInt = skipInt + 10;
                            limit = limit + 10;
                        }
                        catch
                        {
                        }

                        break;
                    case "Messages":
                        MessagesInbox = reddit.Account.Messages.GetMessagesInbox(limit: limit).Skip(skipInt);


                        await Task.Run(() =>
                        {
                            foreach (var message in MessagesInbox)
                            {
                                if (message.Subject.Contains("Mod Newsletter") == false &&
                                    message.Subject != "post reply" && message.Subject != "comment reply" &&
                                    message.Subject != "username mention")
                                    MessageCollection.Add(new PrivateMessage
                                    {
                                        MessageSelf = message
                                    });
                                thing = message.Fullname;
                            }
                        });
                        skipInt = skipInt + 10;
                        limit = limit + 10;
                        break;
                    case "Mod":
                        MessagesInbox = reddit.Account.Messages.GetMessagesInbox(limit: limit).Skip(skipInt);


                        await Task.Run(() =>
                        {
                            foreach (var message in MessagesInbox)
                                if (message.Subject.Contains("Mod Newsletter"))
                                {
                                    MessageCollection.Add(new PrivateMessage
                                    {
                                        MessageSelf = message
                                    });
                                    thing = message.Fullname;
                                }
                        });
                        skipInt = skipInt + 50;
                        limit = limit + 50;
                        break;
                    case "PostReplies":
                        MessagesInbox = reddit.Account.Messages.GetMessagesInbox(limit: limit).Skip(skipInt);

                        await Task.Run(() =>
                        {
                            foreach (var message in MessagesInbox)
                                if (message.Subject == "post reply")
                                {
                                    MessageCollection.Add(new PrivateMessage
                                    {
                                        MessageSelf = message
                                    });
                                    thing = message.Fullname;
                                }
                        });
                        skipInt = skipInt + 10;
                        limit = limit + 10;
                        break;
                    case "CommentReplies":
                        MessagesInbox = reddit.Account.Messages.GetMessagesInbox(limit: limit).Skip(skipInt);

                        await Task.Run(() =>
                        {
                            foreach (var message in MessagesInbox)
                                if (message.Subject == "comment reply")
                                {
                                    MessageCollection.Add(new PrivateMessage
                                    {
                                        MessageSelf = message
                                    });
                                    thing = message.Fullname;
                                }
                        });
                        skipInt = skipInt + 10;
                        limit = limit + 10;
                        break;
                    case "mention":
                        MessagesInbox = reddit.Account.Messages.GetMessagesInbox(limit: limit).Skip(skipInt);

                        await Task.Run(() =>
                        {
                            foreach (var message in MessagesInbox)
                                if (message.Subject.Contains("username mention"))
                                {
                                    MessageCollection.Add(new PrivateMessage
                                    {
                                        MessageSelf = message
                                    });
                                    thing = message.Fullname;
                                }
                        });
                        skipInt = skipInt + 50;
                        limit = limit + 50;
                        break;
                    // Simulates a longer request...
                }
            });

            return MessageCollection;
        }
    }
}
