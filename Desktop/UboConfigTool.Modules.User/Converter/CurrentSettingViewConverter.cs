using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using UboConfigTool.Modules.User.ViewModel;

namespace UboConfigTool.Modules.User.Converter
{
    public class CurrentSettingViewConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            if( parameter == null || !( parameter is string ) )
                return false;

            bool bChecked = false;
            switch( parameter.ToString() )
            {
                case "UserSettingView":
                    bChecked = ( (SettingViewType)value == SettingViewType.UserSettingView );
                    break;
                case "VersionSettingView":
                    bChecked = ( (SettingViewType)value == SettingViewType.VersionSettingView );
                    break;
                case "CloudSettingView":
                    bChecked = ( (SettingViewType)value == SettingViewType.CloudSettingView );
                    break;
                default:
                    break;
            }

            return bChecked;
        }

        public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            SettingViewType curType = SettingViewType.None;

            if( value is bool && parameter != null )
            {
                if( (bool)value )
                {
                    switch( parameter.ToString() )
                    {
                        case "UserSettingView":
                            curType = SettingViewType.UserSettingView;
                            break;
                        case "VersionSettingView":
                            curType = SettingViewType.VersionSettingView;
                            break;
                        case "CloudSettingView":
                            curType = SettingViewType.CloudSettingView;
                            break;
                        default:
                            break;
                    }
                }
            }

            if( curType == SettingViewType.None )
                return null;

            return curType;
        }
    }
}
