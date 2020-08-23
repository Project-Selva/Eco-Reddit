using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Reddit;
using Reddit.Inputs.LinksAndComments;
using Reddit.Things;
using ImgurSharp;
using Subreddit = Reddit.Controllers.Subreddit;
using System.Threading.Tasks;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Selva.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateNewPost : Page
    {
        public static string currentsubSTATIC;
        private bool albumexisting;
        public string appId = "mp8hDB_HfbctBg";
        private readonly string currentsub;
        Flair selectedtoken;
        private readonly Subreddit CurrentSub;
        private readonly List<Flair> FI = new List<Flair>();
        private StorageFile file;
        private bool il;
        private List<string> imageidlist = new List<string>();
        private List<Images> imagelist = new List<Images>();
        private readonly bool IsOtherSubVisible;
        private readonly bool IsSubVisible;
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        private string sub;

        public CreateNewPost()
        {
            InitializeComponent();
            currentsub = currentsubSTATIC;

            var refreshToken = localSettings.Values["refresh_token"].ToString();
            var reddit = new RedditClient(appId, refreshToken, secret);
            CurrentSub = reddit.Subreddit(currentsub);
            //       CarouselControl.ItemsSource = imagelist;
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
                OtherText.IsEnabled = false;
                var Flairs = CurrentSub.Flairs;
                //  var m = new MessageDialog(CurrentSub.Flairs.GetFlairList().Count.ToString());
                //  m.ShowAsync();
                //  try
                //  {

                var f = Flairs.GetLinkFlair();
                foreach (var flair in f)
                    FI.Add(flair);
                Flairlist.SuggestedItemsSource = FI;
                /*  }
                  catch
                  {
                      Flairlist.IsEnabled = false;
                  }*/
            }
        }

        private void EditZone_TextChanged(object sender, RoutedEventArgs e)
        {
            string RichText;
            EditZone.TextDocument.GetText(TextGetOptions.None, out RichText);
            MarkDownBlock.Text = RichText;
        }

        private async void MarkdownText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out var link)) await Launcher.LaunchUriAsync(link);
        }

        private async void MarkdownText_ImageClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out var link)) await Launcher.LaunchUriAsync(link);
        }


        private void CurrentSubreddit_Checked(object sender, RoutedEventArgs e)
        {
            sub = CurrentSub.Name;
            OtherText.IsEnabled = false;
        }


        private void OtherSubreddit_Checked_1(object sender, RoutedEventArgs e)
        {
            OtherText.IsEnabled = true;
            sub = OtherText.Text;
        }


        private async void NewPostButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var refreshToken = localSettings.Values["refresh_token"].ToString();
                var reddit = new RedditClient(appId, refreshToken, secret);
                var pivotitem = (PivotItem)CreatePostPivot.SelectedItem;
                string RichText;
                EditZone.TextDocument.GetText(TextGetOptions.None, out RichText);
                if (pivotitem.Header.ToString() == "Text")
                {
                    // if (string.IsNullOrEmpty(TitleBox.Text) == false &&
                    //     string.IsNullOrEmpty(MarkDownBlock.Text) == false)
                    //  {
                    if (OtherSubreddit.IsChecked == true) sub = OtherText.Text;

                    if (selectedtoken != null)
                    {
                        reddit.Models.LinksAndComments.Submit(new LinksAndCommentsSubmitInput(title: TitleBox.Text,
                                                kind: "self", text: MarkDownBlock.Text, sr: sub, spoiler: Spoil.IsEnabled,
                                                nsfw: NSFW.IsEnabled, flairId: selectedtoken.Id));
                        statusInAppNotification.Show("Posted", 3000);
                    }
                    else
                    {
                        reddit.Models.LinksAndComments.Submit(new LinksAndCommentsSubmitInput(title: TitleBox.Text,
                                  kind: "self", text: MarkDownBlock.Text, sr: sub, spoiler: Spoil.IsEnabled,
                                  nsfw: NSFW.IsEnabled));
                        statusInAppNotification.Show("Posted", 3000);
                    }


                }
                else if (pivotitem.Header.ToString() == "Link")
                {
                    if (string.IsNullOrEmpty(TitleBox.Text) == false &&
                        string.IsNullOrEmpty(Link.Text) == false && Link.Text.StartsWith("http"))
                    {
                        if (OtherSubreddit.IsChecked == true) sub = OtherText.Text;
                        if (selectedtoken != null)
                        {
                            reddit.Models.LinksAndComments.Submit(new LinksAndCommentsSubmitInput(title: TitleBox.Text,
                                url: Link.Text, sr: sub, spoiler: Spoil.IsEnabled, nsfw: NSFW.IsEnabled, flairId: selectedtoken.Id));
                            statusInAppNotification.Show("Posted", 3000);
                 
                        }
                        else
                        {
                            reddit.Models.LinksAndComments.Submit(new LinksAndCommentsSubmitInput(title: TitleBox.Text,
                               url: Link.Text, sr: sub, spoiler: Spoil.IsEnabled, nsfw: NSFW.IsEnabled));
                            statusInAppNotification.Show("Posted", 3000);
                        }
                    }
                    
                }
                else if (pivotitem.Header.ToString() == "Image")
                {
                    if (string.IsNullOrEmpty(TitleBox.Text) == false &&
                        file != null)
                    {
                        if (OtherSubreddit.IsChecked == true) sub = OtherText.Text;
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
                        if (selectedtoken != null)
                        {
                            reddit.Models.LinksAndComments.Submit(new LinksAndCommentsSubmitInput(title: TitleBox.Text,
                            url: image.Link, sr: sub, spoiler: Spoil.IsEnabled, nsfw: NSFW.IsEnabled, flairId: selectedtoken.Id));
                            statusInAppNotification.Show("Posted", 3000);
                        }
                        else
                        {
                            reddit.Models.LinksAndComments.Submit(new LinksAndCommentsSubmitInput(title: TitleBox.Text,
                         url: image.Link, sr: sub, spoiler: Spoil.IsEnabled, nsfw: NSFW.IsEnabled));
                            statusInAppNotification.Show("Posted", 3000);
                        }
                    }
                    else
                    {
                        statusInAppNotification.Show("Failed", 3000);
                    }
                }
            }
            catch
            {
                statusInAppNotification.Show("Failed", 3000);
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
                var picker = new FileOpenPicker();
                picker.ViewMode = PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".png");

                file = await picker.PickSingleFileAsync();
                using (var fileStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap
                    var bitmapImage = new BitmapImage();
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

        private async void Album_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (albumexisting == false)
                {
                    albumsubmitbutton.IsEnabled = true;
                    var client = new ImgurClient("62976f1fad07416", "481ef93212ba3d3f0bb9bf2e152fc36815fb21d3");
                    var endpoint = new AlbumEndpoint(client);
                    var album = await endpoint.CreateAlbumAsync();
                    albumexisting = true;
                    //   try
                    //  {
                    var picker = new FileOpenPicker();
                    picker.ViewMode = PickerViewMode.Thumbnail;
                    picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                    picker.FileTypeFilter.Add(".jpg");
                    picker.FileTypeFilter.Add(".jpeg");
                    picker.FileTypeFilter.Add(".png");

                    file = await picker.PickSingleFileAsync();
                    var bitmapImage = new BitmapImage();
                    using (var fileStream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        // Set the image source to the selected bitmap

                        // Decode pixel sizes are optional
                        // It's generally a good optimisation to decode to match the size you'll display
                        await bitmapImage.SetSourceAsync(fileStream);

                        CarouselControl.Items.Add(new Image
                        {
                            Source = bitmapImage,
                            Height = 200,
                            Tag = file,
                            Stretch = Stretch.Uniform
                        });
                    }

                    //    CarouselControl.ItemsSource = imagelist;
                    statusInAppNotification.Show("Image selected", 3000);
                    /* }
                     catch
                     {
                         file = null;
                         statusInAppNotification.Show("Failed", 3000);
                     }*/
                }
                else
                {
                    //   try
                    //  {
                    var picker = new FileOpenPicker();
                    picker.ViewMode = PickerViewMode.Thumbnail;
                    picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                    picker.FileTypeFilter.Add(".jpg");
                    picker.FileTypeFilter.Add(".jpeg");
                    picker.FileTypeFilter.Add(".png");

                    file = await picker.PickSingleFileAsync();
                    var bitmapImage = new BitmapImage();
                    using (var fileStream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        // Set the image source to the selected bitmap

                        // Decode pixel sizes are optional
                        // It's generally a good optimisation to decode to match the size you'll display
                        await bitmapImage.SetSourceAsync(fileStream);

                        /*  imagelist.Add(new Images()
                          {
                             filepath = file.Path
                          });*/
                        CarouselControl.Items.Add(new Image
                        {
                            Source = bitmapImage,
                            Height = 200,
                            Tag = file,
                            Stretch = Stretch.Uniform
                        });
                    }

                    // CarouselControl.ItemsSource = imagelist;
                    statusInAppNotification.Show("Image selected", 3000);
                }
            }
            catch
            {

                statusInAppNotification.Show("Failed", 3000);
            }
        }
       
        private void Flairlist_TokenItemAdded(TokenizingTextBox sender, TokenizingTextBoxItem args)
        {
            il = false;
       
        }

        private void Flairlist_TokenItemRemoved(TokenizingTextBox sender, TokenItemRemovedEventArgs args)
        {
            Flairlist.SuggestedItemsSource = FI;
            selectedtoken = null;
        }

        private void Flairlist_TokenItemCreating(TokenizingTextBox sender, TokenItemCreatingEventArgs args)
        {
            if (il == false) args.Cancel = true;
            il = false;
        }

        private void Flairlist_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            il = true;
            selectedtoken = (Flair)args.SelectedItem;
            Flairlist.SuggestedItemsSource = null;
        }
        public class Images
        {
            public BitmapImage bitimgsource { get; set; }
            public string filepath { get; internal set; }
        }

        private async void SubmitAlbumButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TitleBox.Text) == false &&
                       string.IsNullOrEmpty(MarkDownBlock.Text) == false)
                {
                    var client = new ImgurClient("62976f1fad07416", "481ef93212ba3d3f0bb9bf2e152fc36815fb21d3");
                    // var Albumendpoint = new AlbumEndpoint(client);
                    //  var album = await Albumendpoint.CreateAlbumAsync();
                    ImgurSharp.Imgur imgur = new ImgurSharp.Imgur("62976f1fad07416");

                    // upload/update/delete your files here!

                    string thumbnailid = "";
                    var endpoint = new ImageEndpoint(client);
                    List<string> idlist = new List<string>();
                    if (OtherSubreddit.IsChecked == true) sub = OtherText.Text;
                    foreach (Image i in CarouselControl.Items)
                    {
                        StorageFile file = i.Tag as StorageFile;
                        var fStream = await file.OpenAsync(FileAccessMode.Read);

                        var reader = new DataReader(fStream.GetInputStreamAt(0));
                        var bytes = new byte[fStream.Size];
                        await reader.LoadAsync((uint)fStream.Size);
                        reader.ReadBytes(bytes);
                        var stream = new MemoryStream(bytes);
                        var image = await endpoint.UploadImageStreamAsync(stream);
                        idlist.Add(image.Id);
                        thumbnailid = image.Id;
                    }
                    //   var added = await Albumendpoint.AddAlbumImagesAsync(album.Id,
                    //          idlist);
                    ImgurSharp.ImgurCreateAlbum createdAlbum = await imgur.CreateAlbumAnonymous(idlist, TitleBox.Text, TitleBox.Text, ImgurSharp.ImgurAlbumPrivacy.Public, ImgurSharp.ImgurAlbumLayout.Horizontal, coverImageId: thumbnailid);
                    var refreshToken = localSettings.Values["refresh_token"].ToString();
                    var reddit = new RedditClient(appId, refreshToken, secret);
                    if(selectedtoken != null){ 
                    reddit.Models.LinksAndComments.Submit(new LinksAndCommentsSubmitInput(title: TitleBox.Text,
                url: "https://imgur.com/gallery/" + createdAlbum.Id, sr: sub, spoiler: Spoil.IsEnabled, nsfw: NSFW.IsEnabled, flairId: selectedtoken.Id));
                    statusInAppNotification.Show("Posted", 3000);
                }
                else
                {
                    reddit.Models.LinksAndComments.Submit(new LinksAndCommentsSubmitInput(title: TitleBox.Text,
       url: "https://imgur.com/gallery/" + createdAlbum.Id, sr: sub, spoiler: Spoil.IsEnabled, nsfw: NSFW.IsEnabled));
                    statusInAppNotification.Show("Posted", 3000);
                }
            }  
           }
            catch
            {
                statusInAppNotification.Show("Failed", 3000);
            }
        }
    }
}
