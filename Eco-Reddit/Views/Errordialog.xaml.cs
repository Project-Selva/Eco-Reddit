using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Selva.Views
{
    public sealed partial class Errordialog : ContentDialog
    {
        public Errordialog()
        {
            InitializeComponent();
            try
            {
                ErrorText.Text = Errormessage;
            }
            catch
            {
            }
        }

        public static string Errormessage { get; set; }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
