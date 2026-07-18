using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace AnlageverzeichnisAppWPF.PrintedDocumentAbstraction
{
    public class sectionSumTableColumnWidths
    {
        public static GridLength spacer => new GridLength(
                                                                PrintedDocumentAbstraction.headerTableColumnWidths.objectDescription.Value
                                                              + PrintedDocumentAbstraction.headerTableColumnWidths.dateOfPurchase.Value
                                                            , GridUnitType.Star
                                                         );
        public static GridLength priceAtPurchase => PrintedDocumentAbstraction.headerTableColumnWidths.priceAtPurchase;
        public static GridLength plusMinus => PrintedDocumentAbstraction.headerTableColumnWidths.plusMinus;
        public static GridLength enterAndLeaveAmount => PrintedDocumentAbstraction.headerTableColumnWidths.enterAndLeaveAmount;
        public static GridLength accumulatedDeprecationAmount => PrintedDocumentAbstraction.headerTableColumnWidths.accumulatedDeprecationAmount;
        public static GridLength spacer2 => PrintedDocumentAbstraction.headerTableColumnWidths.currentYearDeprecationPercentage;
        public static GridLength currentYearDeprecationAmount => PrintedDocumentAbstraction.headerTableColumnWidths.currentYearDeprecationAmount;
        public static GridLength currentYearObjectValue => PrintedDocumentAbstraction.headerTableColumnWidths.currentYearObjectValue;
        public static GridLength previousYearObjectValue => PrintedDocumentAbstraction.headerTableColumnWidths.previousYearObjectValue;
    }
}
