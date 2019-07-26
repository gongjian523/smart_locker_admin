﻿using System;
using System.Windows.Data;

namespace CFLMedCab.Converter
{
    class InventoryActualSwichStaConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((string)value == "正常")
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value == true)
                return "正常";
            else
                return "损坏";
        }
    }
}