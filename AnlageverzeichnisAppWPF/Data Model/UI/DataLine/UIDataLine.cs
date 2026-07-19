using CommunityToolkit.Mvvm.ComponentModel;

namespace AnlageverzeichnisAppWPF
{
    public partial class UIDataLine : AbstractDataLine
    {
        [ObservableProperty]
        private bool isCalculateDerivedFieldsNeeded;

        [ObservableProperty]
        private bool isCurrentHeading = false;

        [ObservableProperty]
        private bool isInvalid = false; // a dirty flag to be used for denoting that the current entry is erroneous and thus should be highlighted in some way

        public int currentYear;

        [ObservableProperty]
        private string objectDescriptionText = "";

        [ObservableProperty]
        private int monthOfPurchase = 1;
        
        [ObservableProperty]
        private int yearOfPurchase = 1900;
        
        [ObservableProperty]
        private Int64 priceAtPurchase_Cents = 0;
        
        [ObservableProperty]
        private Int64? enterOrLeaveAmount_Cents;
        
        [ObservableProperty]
        private Int64 accumulatedDepreciation_Cents  = 0;
        
        [ObservableProperty]
        private int depreciationPercentage_0P1Pct  = 0;
        
        [ObservableProperty]
        private Int64 currentYearDepreciationAmount_Cents  = 0;
        
        [ObservableProperty]
        private Int64 currentYearObjectValue_Cents  = 0;
        
        [ObservableProperty]
        private Int64 previousYearObjectValue_Cents  = 0;

        [ObservableProperty]
        private bool isHeading  = false;
        
        [ObservableProperty]
        private bool isLeavingThisYear  = false;

        [ObservableProperty]
        private bool isAggregatingPosition = false;
        
        [ObservableProperty]
        private bool isNonDeprecating = false;
        
        public UIDataLine()
        {
            // needed for easy json deserialize only! otherwise the constructor with parameters would be used and the deserializer fails!
            // but this also means that when loading from json the current year member will stay at 0 if not initialized otherwise so after loading a file the current year must be set manually for each item!
        }

        public UIDataLine(int currentYear)
        {
            this.currentYear = currentYear;
            YearOfPurchase = currentYear; // to allow the data entry mask using this later to always default to the current year for each entry line
        }

        public StoredDataline toStoredLineType()
        {
            StoredDataline returnedLine = new StoredDataline();
            returnedLine.ObjectDescriptionText = ObjectDescriptionText;
            returnedLine.MonthOfPurchase = MonthOfPurchase;
            returnedLine.YearOfPurchase = YearOfPurchase;
            returnedLine.PriceAtPurchase_Cents = PriceAtPurchase_Cents;
            returnedLine.EnterOrLeaveAmount_Cents = EnterOrLeaveAmount_Cents;
            returnedLine.AccumulatedDepreciation_Cents = AccumulatedDepreciation_Cents;
            returnedLine.DepreciationPercentage_0P1Pct = DepreciationPercentage_0P1Pct;
            returnedLine.CurrentYearDepreciationAmount_Cents = CurrentYearDepreciationAmount_Cents;
            returnedLine.CurrentYearObjectValue_Cents = CurrentYearObjectValue_Cents;
            returnedLine.PreviousYearObjectValue_Cents = PreviousYearObjectValue_Cents;
            returnedLine.IsHeading = IsHeading;
            returnedLine.IsLeavingThisYear = IsLeavingThisYear;
            returnedLine.IsAggregatingPosition = IsAggregatingPosition;
            returnedLine.IsNonDeprecating = IsNonDeprecating;

            return returnedLine;
        }
    }
}
