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

namespace Eco_Reddit.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AwardsFlyoutFrame : Page
    {
        public static Post post { get; set; }
        public AwardsFlyoutFrame()
        {
            this.InitializeComponent();
        }
        private void Award_Loaded(object sender, RoutedEventArgs e)
        {
                TextBlock Award = sender as TextBlock;
                switch (Award.Name)
                {
                    case "SilverAward":
                        Award.Text = "Gold Awards: " + post.Awards.Gold.ToString();
                        break;
                    case "GoldAward":
                        Award.Text = "Platinum Awards: " + post.Awards.Platinum.ToString();
                        break;
                    case "BronzeAward":
                        Award.Text = "Silver Awards: " + post.Awards.Silver.ToString();
                        break;
                    case "TotalAward":
                        Award.Text = "Total Awards: " + post.Awards.Count.ToString();
                        break;
                }
        }
    }
}
