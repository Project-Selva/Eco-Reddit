using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls;

namespace Selva.Views
{
    public sealed partial class WhatsNewDialog : ContentDialog
    {
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public WhatsNewDialog()
        {
            // TODO WTS: Update the contents of this dialog every time you release a new version of the app
            RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            InitializeComponent();
         
        }

    
    }
}
