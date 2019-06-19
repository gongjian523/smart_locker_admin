using CFLMedCab.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CFLMedCab.Converter
{
    /// <summary>
    /// 领用属性
    /// </summary>
    public class RequisitionAttributeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((RequisitionAttribute)value == RequisitionAttribute.无单领用)
                return "无单领用";
            else
                return "有单领用";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
