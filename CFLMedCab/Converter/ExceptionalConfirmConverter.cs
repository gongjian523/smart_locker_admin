using CFLMedCab.Model.Enum;
using System;
using System.Globalization;
using System.Windows.Data;

namespace CFLMedCab.Converter
{
    class ExceptionalConfirmConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((ExceptionFlag)value == ExceptionFlag.正常)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == true)
                return (int)ExceptionFlag.正常;
            else
                return (int)ExceptionFlag.异常;
        }
    }
}
