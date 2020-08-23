using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls;

namespace Selva.Views
{
    public sealed partial class FirstRunDialog : ContentDialog
    {
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public FirstRunDialog()
        {
            // TODO WTS: Update the contents of this dialog with any important information you want to show when the app is used for the first time.
            RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            InitializeComponent();
            PostSlider.Value = (double) localSettings.Values["PostAdFrequency"];
            SidebarSlider.Value = (double) localSettings.Values["SideBarAdFrequency"];
            if ((bool) localSettings.Values["AdEnabled"])
            {
                AdToggle.IsOn = true;
                AdOptionsContainer.Visibility = Visibility.Visible;
                if ((bool) localSettings.Values["PostAdEnabled"])
                {
                    PostSlider.Visibility = Visibility.Visible;
                    PostAdToggle.IsOn = true;
                }
                else
                {
                    PostSlider.Visibility = Visibility.Collapsed;
                    PostAdToggle.IsOn = false;
                }

                if ((bool) localSettings.Values["SideBarAdEnabled"])
                {
                    SideBarAdToggle.IsOn = true;
                    SidebarSlider.Visibility = Visibility.Visible;
                }
                else
                {
                    SideBarAdToggle.IsOn = false;
                    SidebarSlider.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                AdToggle.IsOn = false;
                AdOptionsContainer.Visibility = Visibility.Collapsed;
            }
        }

        private void AdToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (AdToggle.IsOn)
            {
                localSettings.Values["AdEnabled"] = true;
                AdOptionsContainer.Visibility = Visibility.Visible;
            }
            else
            {
                localSettings.Values["AdEnabled"] = false;
                AdOptionsContainer.Visibility = Visibility.Collapsed;
            }
        }

        private void PostAdToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (PostAdToggle.IsOn)
            {
                localSettings.Values["PostAdEnabled"] = true;
                PostSlider.Visibility = Visibility.Visible;
            }
            else
            {
                localSettings.Values["PostAdEnabled"] = false;
                PostSlider.Visibility = Visibility.Collapsed;
            }
        }

        private void SideBarAdToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (SideBarAdToggle.IsOn)
            {
                localSettings.Values["SideBarAdEnabled"] = true;
                SidebarSlider.Visibility = Visibility.Visible;
            }
            else
            {
                localSettings.Values["SideBarAdEnabled"] = false;
                SidebarSlider.Visibility = Visibility.Collapsed;
            }
        }

        private void PostSlider_ValueChanged(object sender, NumberBoxValueChangedEventArgs e)
        {
            localSettings.Values["PostAdFrequency"] = PostSlider.Value;
        }

        private void SidebarSlider_ValueChanged(object sender, NumberBoxValueChangedEventArgs e)
        {
            localSettings.Values["SideBarAdFrequency"] = SidebarSlider.Value;
        }
    }
}
