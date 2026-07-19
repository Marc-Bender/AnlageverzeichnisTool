using System;
using System.Collections.Generic;
using System.Text;

namespace AnlageverzeichnisAppWPF.PrintedDocumentAbstraction
{
    public class sumValues
    {
        public Int64 priceAtPurchase_cent { get; set; }
        public Int64 enterAmount_cent { get; set; }
        public Int64 leaveAmount_cent { get; set; }
        public Int64 accumulatedDeprecation_cent { get; set; }
        public Int64 currentYearDeprecation_cent { get; set; }
        public Int64 currentYearObjectValue_cent { get; set; }
        public Int64 previousYearObjectValue_cent { get; set; }
    }
}
