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
            }
            else
            {
              IsSubVisible = true;
                CurrentSubreddit.IsChecked = true;
                CurrentSubreddit.Content = "r/" + currentsub;
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

        private void OtherSubreddit_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void OtherSubreddit_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void CurrentSubreddit_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CurrentSubreddit_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void OtherSubreddit_Checked_1(object sender, RoutedEventArgs e)
        {

        }

        private void OtherSubreddit_Unchecked_1(object sender, RoutedEventArgs e)
        {

        }
    }

}
