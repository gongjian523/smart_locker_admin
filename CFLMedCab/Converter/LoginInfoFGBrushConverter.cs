using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace CFLMedCab.Converter
{
    class LoginInfoFGBrushConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((int)value == 0)
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2424"));
            else
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#457EFF"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
