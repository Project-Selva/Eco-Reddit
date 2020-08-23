using Selva.Helpers;
using Microsoft.Toolkit.Uwp;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Selva.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserCommentPage : Page
    {
        public static User PostUser;
        User PostUserU;
        public UserCommentPage()
        {
            this.InitializeComponent();
            GetUserPostsClass.UserToGetPostsFrom = PostUserU;
            var Postscollection = new IncrementalLoadingCollection<GetUserComments, Comment>();
        UserList.ItemsSource = Postscollection; 
        }
        private void HomeList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase != 0) throw new Exception("We should be in phase 0, but we are not.");

            // It's phase 0, so this item's title will already be bound and displayed.

            args.RegisterUpdateCallback(ShowPhase1);

            args.Handled = true;
        }
        private async void ShowPhase1(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase != 1) throw new Exception("We should be in phase 1, but we are not.");
            
            ///   TextFlairBlock.Foreground = post.Listing.LinkFlairBackgroundColor;

            //  args.RegisterUpdateCallback(this.ShowPhase2);
        }
        private void HomeList_ItemClick(object sender, ItemClickEventArgs e)
        {

                var P = e.ClickedItem as Comment;
            
        }

    }
}
