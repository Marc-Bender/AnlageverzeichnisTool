using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace AnlageverzeichnisAppWPF
{
    public class CentsToDecimalConverter : IValueConverter
    {
        public object Convert(object centValue, Type targetType, object parameter, CultureInfo culture)
        {
            /*
            if ((int)centValue >= 100)
                return $"{centValue.ToString()[..^2]},{(int)centValue % 100}";   // display as decimal
            else
                return $"0,{(int)centValue % 100}";
            */
            return centValue;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var x = ((string)value).Split(",");

            double y;
            var success = double.TryParse(((string)value), out y);
            /*
            if (success)
                return y ;
            
            if (x.Length == 2)
            {
                if (x[1] != "")
                { 
                    x[1] = "0";
                }
                return int.Parse(x[0]) * 100 + int.Parse(x[1]);
            }
            else
                return int.Parse((string)value);
            */
            return value;

        }
    }
}
