using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Reddit.Controllers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Selva.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AwardsFlyoutFrame : Page
    {
        public static Post post;

        public AwardsFlyoutFrame()
        {
            InitializeComponent();
        }

        private void Award_Loaded(object sender, RoutedEventArgs e)
        {
            var Award = sender as TextBlock;
            switch (Award.Name)
            {
                case "SilverAward":
                    Award.Text = "Gold Awards: " + post.Awards.Gold;
                    break;
                case "GoldAward":
                    Award.Text = "Platinum Awards: " + post.Awards.Platinum;
                    break;
                case "BronzeAward":
                    Award.Text = "Silver Awards: " + post.Awards.Silver;
                    break;
                case "TotalAward":
                    Award.Text = "Total Awards: " + post.Awards.Count;
                    break;
            }
        }
    }
}
