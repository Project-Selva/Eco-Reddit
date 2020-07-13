using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Reddit;
using Reddit.Controllers;
using Reddit.Inputs.LinksAndComments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Eco_Reddit.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateNewPost : Page
    {
        private bool IsSubVisible = false;
        private bool IsOtherSubVisible = false;
        public string appId = "mp8hDB_HfbctBg";
        string sub;
        Windows.Storage.StorageFile file = null;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Subreddit CurrentSub;
        public static string currentsubSTATIC;
        string currentsub;
        public CreateNewPost()
        {
            this.InitializeComponent();
            currentsub = currentsubSTATIC;
            if (currentsub == "HOME")
            {
                IsSubVisible = false;
                IsOtherSubVisible = true;
                OtherSubreddit.IsChecked = true;
                sub = OtherText.Text;
            }
            else
            {
              IsSubVisible = true;
                CurrentSubreddit.IsChecked = true;
                CurrentSubreddit.Content = "r/" + currentsub;
                sub = CurrentSub.Name;
            }
        }
        private void EditZone_TextChanged(object sender, RoutedEventArgs e)
        {
            String RichText;
            EditZone.TextDocument.GetText(Windows.UI.Text.TextGetOptions.None, out RichText);
            MarkDownBlock.Text = RichText;
        }
        private async void MarkdownText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out Uri link))
            {
                await Launcher.LaunchUriAsync(link);
            }
        }
        private async void MarkdownText_ImageClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out Uri link))
            {
                await Launcher.LaunchUriAsync(link);
            }
        }

  
        

        private void CurrentSubreddit_Checked(object sender, RoutedEventArgs e)
        {
            sub = CurrentSub.Name;
        }

  

        private void OtherSubreddit_Checked_1(object sender, RoutedEventArgs e)
        {
            sub = OtherText.Text;
        }

   

        private async void NewPostButton_Click(object sender, RoutedEventArgs e)
        {
            string refreshToken = localSettings.Values["refresh_token"].ToString();
            var reddit = new RedditClient(appId, refreshToken, secret);
            PivotItem pivotitem = (PivotItem)CreatePostPivot.SelectedItem;
            String RichText;
            EditZone.TextDocument.GetText(Windows.UI.Text.TextGetOptions.None, out RichText);
            if (pivotitem.Header.ToString() == "Text")
            {
                if (string.IsNullOrEmpty(OtherText.Text) == false && string.IsNullOrEmpty(TitleBox.Text) == false && string.IsNullOrEmpty(MarkDownBlock.Text) == false)
                {
                    if (OtherSubreddit.IsChecked == true)
                    {
                        sub = OtherText.Text;
                    }
                    reddit.Models.LinksAndComments.Submit(new LinksAndCommentsSubmitInput(title: TitleBox.Text, kind: "self", text: MarkDownBlock.Text, sr: sub, spoiler: Spoil.IsEnabled, nsfw: NSFW.IsEnabled));
                    statusInAppNotification.Show("Posted", 3000);
                }
                else
                {
                    statusInAppNotification.Show("Failed", 3000);
                }
            }
            else if ((pivotitem.Header.ToString() == "Link"))
            {
                if (string.IsNullOrEmpty(OtherText.Text) == false && string.IsNullOrEmpty(TitleBox.Text) == false && string.IsNullOrEmpty(Link.Text) == false && Link.Text.StartsWith("http") == true)
                {
                    if (OtherSubreddit.IsChecked == true)
                    {
                        sub = OtherText.Text;
                    }
                    reddit.Models.LinksAndComments.Submit(new LinksAndCommentsSubmitInput(title: TitleBox.Text, url: Link.Text, sr: sub, spoiler: Spoil.IsEnabled, nsfw: NSFW.IsEnabled));
                statusInAppNotification.Show("Posted", 3000);
                }
                else
                {
                    statusInAppNotification.Show("Failed", 3000);
                }
            }
            else if ((pivotitem.Header.ToString() == "Image"))
            {
                if (string.IsNullOrEmpty(OtherText.Text) == false && string.IsNullOrEmpty(TitleBox.Text) == false && file != null)
                {
                    if (OtherSubreddit.IsChecked == true)
                    {
                        sub = OtherText.Text;
                    }
                    IImage image = null;
                  //  try
                  //  {
                        var client = new ImgurClient("62976f1fad07416", "481ef93212ba3d3f0bb9bf2e152fc36815fb21d3");
                        var endpoint = new ImageEndpoint(client);
                  
                    var fStream = await file.OpenAsync(FileAccessMode.Read);

                    var reader = new DataReader(fStream.GetInputStreamAt(0));
                    var bytes = new byte[fStream.Size];
                    await reader.LoadAsync((uint)fStream.Size);
                    reader.ReadBytes(bytes);
                        var stream = new MemoryStream(bytes);
                    image = await endpoint.UploadImageStreamAsync(stream);
                    
                   /* }
                    catch (ImgurException imgurEx)
                    {
                        statusInAppNotification.Show("Upload failed", 3000);
                    }*/
        
                    reddit.Models.LinksAndComments.Submit(new LinksAndCommentsSubmitInput(title: TitleBox.Text, url: image.Link, sr: sub, spoiler: Spoil.IsEnabled, nsfw: NSFW.IsEnabled));
                    statusInAppNotification.Show("Posted", 3000);
                }
                else
                {
                    statusInAppNotification.Show("Failed", 3000);
                }
            }
        }

        private void OtherText_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            sub = OtherText.Text;
        }

        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".png");

                file = await picker.PickSingleFileAsync();
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap
                    BitmapImage bitmapImage = new BitmapImage();
                    // Decode pixel sizes are optional
                    // It's generally a good optimisation to decode to match the size you'll display
                    await bitmapImage.SetSourceAsync(fileStream);
                    pRVEIEWiMAGE.Source = bitmapImage;
                }
                statusInAppNotification.Show("Image selected", 3000);
            }
            catch
            {
                file = null;
                statusInAppNotification.Show("Failed", 3000);
            }
        }
    }

}
