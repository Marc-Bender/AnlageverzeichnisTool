using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace AnlageverzeichnisAppWPF
{
    public class CentsToEuroStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (
                    (value is not null)
                 && (value is Int64 cents)
               )
            {
                Int64 euros = cents / 100;
                byte rest = (byte)(cents % 100);
                return $"{euros},{rest:00}";
            }
            else return "0,00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && str != "")
            {
                try
                {
                    Int64 centsValue = 0;
                    Int64.TryParse(str.Replace(",", ""), out centsValue);
                    return centsValue;
                }
                catch
                {
                    // may be reached if the value contains some invalid characters eg. the * was pressed on the letter side of the keyboard not the numpad...
                }
            }
            return 0;
        }
    }
    public class TenthPctToPercentageString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int _0P1PctValue = (int)value;
            int fullPercents = _0P1PctValue / 10;
            int rest = _0P1PctValue % 10;
            return $"{fullPercents},{rest:0}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && str != "")
            {
                try
                {
                    int tenthsValue = 0;
                    int.TryParse(str.Replace(",", ""), out tenthsValue);
                    return tenthsValue;
                }
                catch
                {
                    // may be reached if the value contains some invalid characters eg. the * was pressed on the letter side of the keyboard not the numpad...
                }
            }
            return 0;
        }
    }
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => !(bool)value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => !(bool)value;
    }
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = value is bool v && v;
            return b ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    public class BoolInverseToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = value is bool v && v;
            return b ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    public class BoldHeadingsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = value is bool v && v;
                return b ? FontWeights.Bold : FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    public class EditingToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = value is bool v && v;
                return b ? "White": "Transparent";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

}
