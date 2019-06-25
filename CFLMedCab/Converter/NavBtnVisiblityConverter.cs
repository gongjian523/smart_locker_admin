using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using static CFLMedCab.Model.Enum.NavBtnEnum;
using static CFLMedCab.Model.Enum.UserIdEnum;

namespace CFLMedCab.Converter
{
    class NavBtnVisiblityConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if((UserIdType)value == UserIdType.医生 || (UserIdType)value == UserIdType.医院管理员 || (UserIdType)value == UserIdType.护士)
            {
                if ((NavBtnType)parameter == NavBtnType.一般领用 || (NavBtnType)parameter == NavBtnType.手术领用 || (NavBtnType)parameter == NavBtnType.领用退回)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
            else
            {
                if ((NavBtnType)parameter == NavBtnType.一般领用 || (NavBtnType)parameter == NavBtnType.手术领用 || (NavBtnType)parameter == NavBtnType.领用退回)
                    return Visibility.Hidden;
                else
                    return Visibility.Visible;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
