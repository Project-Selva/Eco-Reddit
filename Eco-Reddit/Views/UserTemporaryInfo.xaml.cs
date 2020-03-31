using Reddit.Controllers;
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
using WinUI = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Eco_Reddit.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserTemporaryInfo : Page
    {
        public static User PostUser { get; set; }
        User CurrentUser;
        public UserTemporaryInfo()
        {
            this.InitializeComponent();
            var User = PostUser.About();
            CurrentUser = PostUser;
            TitleAuthor.Text = "u/" + User.Name;
            FullNameAuthor.Text = User.Id;
            AuthorKarma.Text = "Karma total: " + (User.CommentKarma + User.LinkKarma);
             AuthorPostKarma.Text = " Post Karma: " + User.LinkKarma;
              AuthorCommentKarma.Text = " Comment Karma: " + User.CommentKarma;
            AuthorDate.Text = "Created: " + User.Created.ToString();
            AuthorFriends.Text = "Friends: " + User.NumFriends.ToString();
            if(User.IsFriend == true)
            {
                NSFWUser.Visibility = Visibility.Visible;
            }
            if (User.IsGold == true)
            {
               PremiumUser.Visibility = Visibility.Visible;
            }
            if (User.IsMod == true)
            {
                ModUser.Visibility = Visibility.Visible;
            }
            if (User.IsSuspended == true)
            {
                SuspendedUser.Visibility = Visibility.Visible;
            }
            if (User.IsVerified == true)
            {
                 VerifiedUser.Visibility = Visibility.Visible;
            }
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
           await CurrentUser.BlockAsync();
        }

        private async void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
          // await CurrentUser.ReportAsync("", );
        }

        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            var newTab = new WinUI.TabViewItem();
            newTab.IconSource = new WinUI.SymbolIconSource() { Symbol = Symbol.Document };
            newTab.Header =  "u/" + CurrentUser.About().Name;
            Frame frame = new Frame();
            newTab.Content = frame;
            UserHomePage.PostUser = PostUser;
            frame.Navigate(typeof(UserHomePage));
            HomePage.MainTab.TabItems.Add(newTab);
            HomePage.MainTab.SelectedItem = newTab;
        }
    }
}
