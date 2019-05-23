using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using UboConfigTool.Modules.Logging.Model;

namespace UboConfigTool.Modules.Logging.Converter
{
    public class LogLevelTypeStringConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            string strLevel = string.Empty;

            switch( (LogLevelType)value )
            {
                case LogLevelType.Error:
                    strLevel = "错误";
                    break;
                case LogLevelType.Fatal:
                    strLevel = "严重错误";
                    break;
                case LogLevelType.Info:
                    strLevel = "信息";
                    break;
                case LogLevelType.Warning:
                    strLevel = "警告";
                    break;
                default:
                    break;
            }

            return strLevel;
        }

        public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}
