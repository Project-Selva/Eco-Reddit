using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Eco_Reddit.Services;
using Eco_Reddit.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace Eco_Reddit
{
    public sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        public App()
        {
            InitializeComponent();
            AppCenter.Start("1748b4f9-4634-4e05-8c3d-26d5d1b6568b",
                   typeof(Analytics), typeof(Crashes));
            UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            // Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }

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
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }
        }
        private async static void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
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

        private async static void OnUnobservedException(object sender, UnobservedTaskExceptionEventArgs e)
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
            return new ActivationService(this, typeof(Views.HomePage));
        }
    }
}
