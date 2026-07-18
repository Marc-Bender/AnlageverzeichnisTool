using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace AnlageverzeichnisAppWPF.PrintedDocumentAbstraction
{
    public class headerTableColumnWidths
    {
        public static GridLength objectDescription => new GridLength(1.5, GridUnitType.Star);
        public static GridLength dateOfPurchase => new GridLength(0.3, GridUnitType.Star);
        public static GridLength priceAtPurchase => new GridLength(0.5, GridUnitType.Star);
        public static GridLength plusMinus => new GridLength(0.15, GridUnitType.Star);
        public static GridLength enterAndLeaveAmount => new GridLength(0.5, GridUnitType.Star);
        public static GridLength accumulatedDeprecationAmount => new GridLength(0.5, GridUnitType.Star);
        public static GridLength currentYearDeprecationPercentage => new GridLength(0.25, GridUnitType.Star);
        public static GridLength currentYearDeprecationAmount => new GridLength(0.3, GridUnitType.Star);
        public static GridLength currentYearObjectValue => new GridLength(0.5, GridUnitType.Star);
        public static GridLength previousYearObjectValue => new GridLength(0.5, GridUnitType.Star);
    }
}
