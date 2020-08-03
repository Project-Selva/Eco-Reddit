using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Xaml;
using Eco_Reddit.Helpers;
using Eco_Reddit.Services;
using Eco_Reddit.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Toolkit.Uwp;
using Reddit;
using Reddit.Controllers;
using UnhandledExceptionEventArgs = Windows.UI.Xaml.UnhandledExceptionEventArgs;
using Microsoft.Toolkit.Uwp.Helpers;

namespace Eco_Reddit
{
    public sealed partial class App : Application
    {
        private readonly Lazy<ActivationService> _activationService;

        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public App()
        {
            InitializeComponent();
            AppCenter.Start("1748b4f9-4634-4e05-8c3d-26d5d1b6568b",
                typeof(Analytics), typeof(Crashes));
            UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            if (SystemInformation.Instance.IsFirstRun)
            {
                localSettings.Values["AdEnabled"] = true;
                double d = 69;
                localSettings.Values["PostAdFrequency"] = d;
                localSettings.Values["SideBarAdFrequency"] = d;
                localSettings.Values["PostAdEnabled"] = true;
                localSettings.Values["SideBarAdEnabled"] = true;
                localSettings.Values["refresh_token"] = "589558656590-c5zKzVmhsgqmJGqobEebBOAQVl8";
                localSettings.Values["access_token"] = "589558656590-ZeYF5TpcDpIaJy6EEVpKFaz_KOU";
            }
            // Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }

        private ActivationService ActivationService => _activationService.Value;

        private void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            try
            {
                Crashes.TrackError(e.Exception);
            }
            catch
            {
            }
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            var canEnablePrelaunch =
                ApiInformation.IsMethodPresent("Windows.ApplicationModel.Core.CoreApplication", "EnablePrelaunch");
            if (args.PrelaunchActivated == false)
            {
                // On Windows 10 version 1607 or later, this code signals that this app wants to participate in prelaunch
                if (canEnablePrelaunch) TryEnablePrelaunch();

                // TODO: This is not a prelaunch activation. Perform operations which
                // assume that the user explicitly launched the app such as updating
                // the online presence of the user on a social network, updating a
                // what's new feed, etc.


              ActivationService.ActivateAsync(args);
                // Ensure the current window is active
                Window.Current.Activate();
            }


            if (!args.PrelaunchActivated) await ActivationService.ActivateAsync(args);
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Occurs when an exception is not handled on the UI thread.
            try
            {
                Crashes.TrackError(e.Exception);
            }
            catch
            {
            }

            // if you want to suppress and handle it manually, 
            // otherwise app shuts down.
            e.Handled = true;
        }

        private void TryEnablePrelaunch()
        {
            CoreApplication.EnablePrelaunch(true);
        }

        private static void OnUnobservedException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            // Occurs when an exception is not handled on a background thread.
            // ie. A task is fired and forgotten Task.Run(() => {...})
            try
            {
                Crashes.TrackError(e.Exception);
            }
            catch
            {
            }

            // suppress and handle it manually.
            e.SetObserved();
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private ActivationService CreateActivationService()
        {
            try
                  {
                /*      var BackuprefreshToken = "589558656590-c5zKzVmhsgqmJGqobEebBOAQVl8";
                 if (localSettings.Values["refresh_token"].ToString() == BackuprefreshToken
                 ) //remove and replace, this is when user is not signed in and should show different ui
                 {
                     localSettings.Values["AdEnabled"] = true;
                     double d = 69;
                     localSettings.Values["PostAdFrequency"] = d;
                     localSettings.Values["SideBarAdFrequency"] = d;
                     localSettings.Values["PostAdEnabled"] = true;
                     localSettings.Values["SideBarAdEnabled"] = true;
                     return new ActivationService(this, typeof(LoginPage));
                 }
                 else
                 {
                     var appId = "mp8hDB_HfbctBg";
                     var secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
                     var refreshToken = localSettings.Values["refresh_token"].ToString();
                     var Client = new RedditClient(appId, refreshToken, secret);
                     GetPostsClass.SortOrder = "Best";
                     var reddit = new RedditClient(appId, refreshToken, secret);
                     var PostsCollection = new IncrementalLoadingCollection<GetPostsClass, Post>();
                     HomePage.HomePostList = PostsCollection;

                     // Views.HomePage.L.ItemsSource = PostsCollection;
                     return new ActivationService(this, typeof(HomePage));
                 }*/
           
                // Views.HomePage.L.ItemsSource = PostsCollection;
                return new ActivationService(this, typeof(HomePage));
            }
                 catch
                  {
                /* localSettings.Values["AdEnabled"] = true;
                  double d = 69;
                  localSettings.Values["PostAdFrequency"] = d;
                  localSettings.Values["SideBarAdFrequency"] = d;
                  localSettings.Values["PostAdEnabled"] = true;
                  localSettings.Values["SideBarAdEnabled"] = true;*/


                    // Views.HomePage.L.ItemsSource = PostsCollection;
                    return new ActivationService(this, typeof(HomePage));
                  }
        }
    }
}
