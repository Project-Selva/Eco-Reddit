using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Eco_Reddit.Core.Models
{
    public class Utils
    {
        public static string UnixTimeStampToLocalTime(DateTime timeStamp, string format = "")
        {
            return timeStamp.ToString(format == null || format == string.Empty
                ? "dddd, dd MMMM yyyy HH:mm:ss"
                : format);
        }

        public static Visibility IsImageVisible(string url)
        {
            return url != null || url != "" ? Visibility.Visible : Visibility.Collapsed;
        }

        public static GridLength GetColWidth(int originalWidth, string imgUrl)
        {
            if (imgUrl == "self" || imgUrl == "default" || imgUrl == "nsfw" || imgUrl == "spoiler" || imgUrl == null ||
                imgUrl == "") return new GridLength(0);
            return new GridLength(originalWidth);
        }

        public static ImageSource CheckImageIsNullOrEmpty(string imgUrl)
        {
            if (imgUrl == null || imgUrl == "")
                return new BitmapImage(new Uri("ms-appx:///Assets/RedditLogo.png"));
            return new BitmapImage(new Uri(imgUrl));
        }

        public static Thickness GenerateCommentLeftPadding(uint commentDepth, int top, int right, int bottom)
        {
            return new Thickness(10 + commentDepth * 20, top, right, bottom);
        }
    }
}
