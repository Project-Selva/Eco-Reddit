using System;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Reddit.Controllers;
using Eco_Reddit.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.System;
using Eco_Reddit.Views;
using Windows.UI.Xaml.Input;
using Reddit;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using WinUI = Microsoft.UI.Xaml.Controls;
using System.Windows.Input;

namespace Eco_Reddit.ViewModels
{
    /// <summary>
    /// A base class for viewmodels for sample pages in the app.
    /// </summary>
    public class PostUserControlViewModel : ObservableObject
    {

        public string appId = "mp8hDB_HfbctBg";
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public string secret = "UCIGqKPDABnjb0XtMh0Q_LhrNks";
        public Post PostItem { get; set; }

        public PostUserControlViewModel()
        {
        }

    }
}
