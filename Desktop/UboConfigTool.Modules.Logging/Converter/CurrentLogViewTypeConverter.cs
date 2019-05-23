using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using UboConfigTool.Modules.Logging.ViewModel;

namespace UboConfigTool.Modules.Logging.Converter
{
    public class CurrentLogViewTypeConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            if(parameter is string && parameter.ToString() == "reverse")
            {
                return ( (LogViewDisplayType)value == LogViewDisplayType.SystemLog );
            }

            return ( (LogViewDisplayType)value == LogViewDisplayType.DeviceLog );
        }

        public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            if( value is bool )
            {
                return (bool)value ? LogViewDisplayType.DeviceLog : LogViewDisplayType.SystemLog;
            }

            return LogViewDisplayType.None;
        }
    }
}
