using System;
using System.Windows.Data;

namespace CFLMedCab.Converter
{
    class InventoryActualSwichStaConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((int)value == 1)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value == true)
                return 1;
            else
                return 0;
        }
    }
}