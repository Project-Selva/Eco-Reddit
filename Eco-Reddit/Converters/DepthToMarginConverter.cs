using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Eco_Reddit.Converters
{
    public class DepthToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string _)
        {
            if (value is UInt32)
            {
                return new Thickness((uint)value * 15, 5, 5, 5);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string _)
        {
            throw new NotImplementedException();
        }
    }
}
