using System;
using System.Windows;
using System.Windows.Data;

namespace CFLMedCab.Converter
{
    class InventoryConfirmBtnVisiblityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //O 带确认
            if ((int)value == 0)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}