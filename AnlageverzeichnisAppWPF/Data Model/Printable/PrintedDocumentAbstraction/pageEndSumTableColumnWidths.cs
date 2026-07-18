using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace AnlageverzeichnisAppWPF.PrintedDocumentAbstraction
{
    public class pageEndSumTableColumnWidths
    {
        public static GridLength pageEndSumTableLabel => new GridLength(
                                                                              PrintedDocumentAbstraction.headerTableColumnWidths.objectDescription.Value
                                                                            + PrintedDocumentAbstraction.headerTableColumnWidths.dateOfPurchase.Value
                                                                        , GridUnitType.Star
                                                                       );
        public static GridLength priceAtPurchase => PrintedDocumentAbstraction.headerTableColumnWidths.priceAtPurchase;
        public static GridLength spacer => new GridLength(
                                                                PrintedDocumentAbstraction.headerTableColumnWidths.plusMinus.Value
                                                              + PrintedDocumentAbstraction.headerTableColumnWidths.enterAndLeaveAmount.Value
                                                           , GridUnitType.Star
                                                         );
        public static GridLength accumulatedDeprecationAmount => PrintedDocumentAbstraction.headerTableColumnWidths.accumulatedDeprecationAmount;
        public static GridLength spacer2 => PrintedDocumentAbstraction.headerTableColumnWidths.currentYearDeprecationPercentage;
        public static GridLength currentYearDeprecationAmount => PrintedDocumentAbstraction.headerTableColumnWidths.currentYearDeprecationAmount;
        public static GridLength currentYearObjectValue => PrintedDocumentAbstraction.headerTableColumnWidths.currentYearObjectValue;
        public static GridLength previousYearObjectValue => PrintedDocumentAbstraction.headerTableColumnWidths.previousYearObjectValue;
    }
}
