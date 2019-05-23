using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace UboConfigTool.Controls.Converters
{
    /// <summary>
    /// Value converter that translates true to <see cref="Visibility.Visible"/> and false to
    /// <see cref="Visibility.Collapsed"/> and vice-versa, when IsReverse is false. When IsReverse is true then
    /// it translates true to <see cref="Visibility.Collapsed"/> and false to
    /// <see cref="Visibility.Visible"/> and vice=versa.
    /// NOTE: IsReverse is false by default.
    /// </summary>
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public bool IsReverse { get; set; }

        public BooleanToVisibilityConverter()
        {
            IsReverse = false;
        }

        #region IValueConverter Members

        /// <summary>
        /// Converts boolean? to Visibility
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var result = Visibility.Collapsed;

            if (value is bool)
            {
                result = (IsReverse ^ (bool)value) ? Visibility.Visible : Visibility.Collapsed;
            }

            return result;
        }

        /// <summary>
        /// Converts Visibility to boolean?
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var result = false;

            if (value is Visibility)
            {
                result = IsReverse ^ ((Visibility)value == Visibility.Visible);
            }

            return result;
        }

        #endregion
    }
}
