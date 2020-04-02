using Microsoft.Toolkit.Uwp.UI.Controls;
using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WinUI = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Eco_Reddit.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SubredditTemporaryInfo : Page
    {
        public static Subreddit InfoSubReddit { get; set; }
        public SubredditTemporaryInfo()
        {
            this.InitializeComponent();
        }
        public void StartUp(object sender, RoutedEventArgs e)
        {
            var subreddit = InfoSubReddit.About();
            Title.Text = "r/" + subreddit.Name;

            Header.Text = subreddit.Title;
            SubscribeButton.Visibility = Visibility.Visible;
            UnSubscribeButton.Visibility = Visibility.Visible;
            SideBar.Visibility = Visibility.Visible;
            Header.Visibility = Visibility.Visible;
            About.Visibility = Visibility.Visible;
            Subs.Visibility = Visibility.Visible;
            Users.Visibility = Visibility.Visible;
            Time.Visibility = Visibility.Visible;
            if (String.IsNullOrEmpty(subreddit.IconImg.ToString()) == false)
            {
                BitmapImage img = new BitmapImage();
                //  img.UriSource = new Uri("/Images/1409938.png");
                //  SubIcon.ProfilePicture = img;
            }
            if (String.IsNullOrEmpty(subreddit.BannerImg.ToString()) == false)
            {
                BitmapImage Bimg = new BitmapImage();
                //   Bimg.UriSource = new Uri("/Images/1409938.png");
                //  BannerIMG.Source = Bimg;
            }
            /*      if (String.IsNullOrEmpty(subreddit.HeaderImg.ToString()) == false)
                  {
                      BitmapImage Himg = new BitmapImage();
                      Himg.UriSource = new Uri(subreddit.HeaderImg.ToString());
                      HeaderIMG.Source = Himg;
                  }*/
            SubscribeButton.Visibility = Visibility.Visible;
            UnSubscribeButton.Visibility = Visibility.Visible;
            About.Text = subreddit.Description;
            Subs.Text = "Active Users: " + subreddit.ActiveUserCount.ToString();
            Users.Text = "Subscribers: " + subreddit.Subscribers.ToString();
            Time.Text = "Created: " + subreddit.Created.ToString();
            if (subreddit.Over18 == true)
            {
                NSFWGrid.Visibility = Visibility.Visible;
            }
            SideBar.Text = "Sidebar isnt supported in EcoReddit Alpha";
            /*  try { 
              SideBarSidebar.Text = subreddit.Sidebar.ToString();
              }
              catch
              {
                  SideBarSidebar.Text = "Couldnt load sidebar";
              }*/
            LoadingControl.Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HomePage.SingletonReference.NavigateJumper(InfoSubReddit.About().Name);
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
        private async void UnSubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var subreddit = InfoSubReddit.About();
                await subreddit.UnsubscribeAsync();
            }
            catch
            {
                return;
            }
        }

        private async void SubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var subreddit = InfoSubReddit.About();
                await subreddit.SubscribeAsync();
            }
            catch
            {
                return;
            }
        }
    }
}
