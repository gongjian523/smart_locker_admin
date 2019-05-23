using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using UboConfigTool.Infrastructure.Models;

namespace UboConfigTool.Modules.UBO.Converter
{
    public class UBOGroupUSBConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            if( value == null || parameter == null )
                return Visibility.Collapsed;

            var result = Visibility.Collapsed;

            switch( (USB_Access_Role)value )
            {
                case USB_Access_Role.Dog_key:
                    result = ( parameter.ToString() == "DogKey" ? Visibility.Visible : Visibility.Collapsed );
                    break;
                case USB_Access_Role.Read_Only:
                    result = ( parameter.ToString() == "ReadOnly" ? Visibility.Visible : Visibility.Collapsed );
                    break;
                case USB_Access_Role.Read_Write:
                    result = ( parameter.ToString() == "ReadWrite" ? Visibility.Visible : Visibility.Collapsed );
                    break;
                default:
                    break;
            }

            return result;
        }

        public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}
