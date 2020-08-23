using Newtonsoft.Json;

namespace Selva.Core
{
    /// <summary>
    /// Wrapper class to provide <see cref="IWebAgent"/> to children.
    /// </summary>
    public abstract class RedditObject
    {
        /// <summary>
        /// WebAgent for requests
        /// </summary>
        [JsonIgnore]
        public IWebAgent WebAgent { get; }

        /// <summary>
        /// Assign <see cref="WebAgent"/>
        /// </summary>
        /// <param name="agent"></param>
        public RedditObject(IWebAgent agent)
        {
            WebAgent = agent ?? new WebAgent();
        }

    }

}