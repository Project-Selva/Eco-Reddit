namespace Eco_Reddit.Core
{
    internal class TextData : SubmitData
    {
        /// <summary>
        /// Markdown text of the self post.
        /// </summary>
        [RedditAPIName("text")]
        internal string Text { get; set; }

        internal TextData()
        {
            Kind = "self";
        }
    }
}
