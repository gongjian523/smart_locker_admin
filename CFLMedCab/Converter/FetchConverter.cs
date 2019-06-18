using CFLMedCab.Model;
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
    /// 操作属性
    /// </summary>
    public class FetchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((OperateType)value == OperateType.入库)
                return "入库";
            else
                return "出库";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

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
