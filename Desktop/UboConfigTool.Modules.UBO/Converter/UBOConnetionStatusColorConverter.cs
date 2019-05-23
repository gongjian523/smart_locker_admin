using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using UboConfigTool.Infrastructure.Models;

namespace UboConfigTool.Modules.UBO.Converter
{
    public class UBOConnetionStatusColorConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            SolidColorBrush corBrush;
            switch( (UBOConnectionType)value )
            {
                case UBOConnectionType.Connected:
                    corBrush = new SolidColorBrush( (Color)ColorConverter.ConvertFromString( "#FFff6600" ) );
                    break;
                case UBOConnectionType.Disconnected:
                    corBrush = new SolidColorBrush( (Color)ColorConverter.ConvertFromString( "#FF373737" ) );
                    break;
                default:
                    corBrush = new SolidColorBrush( (Color)ColorConverter.ConvertFromString( "#FF373737" ) );
                    break;
            }
            return corBrush;
        }

        public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}
