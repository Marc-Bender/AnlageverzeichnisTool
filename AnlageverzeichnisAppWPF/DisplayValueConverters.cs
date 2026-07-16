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
                var de = new CultureInfo("de-DE");
                return string.Format(de, "{0:N0},{1:00}", euros, rest);
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
                    Int64.TryParse(str.Replace(",", "").Replace(".", ""), out centsValue);
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
    public class YearsTo0P1PctConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(
                    (value is int years)
                 && (years > 0)
              ) 
            {
                return 1000 / years;
            }
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is int deprecation0P1Pct)
            {
                return 1000 / deprecation0P1Pct;
            }
            return value;
        }
    }

    public class CanLeaveConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values.Length == 3)
            {
                var currentYear = (int)values[0];
                var purchaseYear = (int)values[1];
                var isHeading = (bool)values[2];
                // do whatever you want
                return (
                            (currentYear != purchaseYear)
                          &&(isHeading == false)
                       );
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
    public class FirstYearItalicPreviousYearConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2)
            {
                if (
                        (values[0] is int currentYear)
                     && (values[1] is int purchaseYear)
                   )
                {
                    return currentYear == purchaseYear ? FontStyles.Italic : FontStyles.Normal;
                }
            }
            return FontStyles.Normal;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class dateOfPurchaseConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 3)
            {
                if (
                        (values[0] is int purchaseMonth)
                     && (values[1] is int purchaseYear)
                     && (values[2] is bool isAggregatingPosition)
                   )
                {
                    if (isAggregatingPosition == false)
                    {
                        return string.Format("{0:00}/{1}", purchaseMonth, purchaseYear);
                    }
                    else
                    {
                        return $"   {purchaseYear}"; // to ensure consistency w/ the other dates where month/year is used add 3 spaces to front...
                    }
                }
            }
            return "";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value is string dateString)
            {
                var parts = dateString.Split('/');
                if (parts.Length == 2)
                {
                    int month = int.Parse(parts[0]);
                    int year = int.Parse(parts[1]);

                    return new object[] { month, year };
                }
                else if(parts.Length == 1)
                {
                    // may only be the case if the line was in aggregating positions ... 
                    int month = 1;
                    int year = int.Parse(parts[0]);

                    return new object[] { month, year };
                }
            }
            return new object[] { };
        }
    }


}
