using System;
using System.Globalization;
using System.Windows.Data;

namespace CFLMedCab.Converter
{
    class DateStrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (((DateTime)value).Year == 1)
                return "";
            else
                return ((DateTime)value).ToString("yyyy-MM-dd");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}