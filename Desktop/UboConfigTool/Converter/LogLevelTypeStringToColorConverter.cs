using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace UboConfigTool.Converter
{
    public class LogLevelTypeStringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush logColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6C00"));

            switch ((string)value)
            {
                case "info":
                    logColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4B4B4B"));
                    break;
                case "error":
                    logColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6C00"));
                    break;
                case "fatal":
                    logColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FC000A"));
                    break;
                case "warning":
                    logColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9066"));
                    break;
                default:
                    break;
            }

            return logColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        public Color logColor { get; set; }
    }
}
