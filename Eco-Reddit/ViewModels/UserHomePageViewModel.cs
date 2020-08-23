using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Reddit.Controllers;
using Selva.Helpers;
using Microsoft.Toolkit.Uwp;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;
using Selva.Core.Models;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.System;
using Selva.Views;
using Windows.UI.Xaml.Input;

namespace Selva.ViewModels
{
    /// <summary>
    /// A base class for viewmodels for sample pages in the app.
    /// </summary>
    public class UserHomePageViewModel : ObservableObject
    {


        public UserHomePageViewModel()
        {
     
        }

        public IncrementalLoadingCollection<GetUserPostsClass, Post> Posts { get; set; }
        public String reason { get; set; }
        public Reddit.Controllers.User CurrentUser { get; set; }

        /// <summary>
        /// Gets the <see cref="IAsyncRelayCommand{T}"/> responsible for loading the source markdown docs.
        /// </summary>
        public async Task StartPosts(Reddit.Controllers.User user)
        {
            await Task.Run(async () =>
            {
                //   GetUserPostsClass.limit = 10;
                //    GetUserPostsClass.skipInt = 0;

                GetUserPostsClass.UserToGetPostsFrom = user;
                Posts = new IncrementalLoadingCollection<GetUserPostsClass, Post>();

            });
        }

        public async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await CurrentUser.BlockAsync();
        }

        public async void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            await CurrentUser.ReportAsync(reason);
        }


    }
}
