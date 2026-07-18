using CommunityToolkit.Mvvm.ComponentModel;

namespace AnlageverzeichnisAppWPF
{
    public abstract class AbstractDataLine : ObservableObject
    {
        public virtual string? ObjectDescriptionText { get; set; }
        public virtual int MonthOfPurchase {get; set;}
        public virtual int YearOfPurchase {get; set;}
        public virtual Int64 PriceAtPurchase_Cents {get; set;}
        public virtual Int64? EnterOrLeaveAmount_Cents {get; set;}
        public virtual Int64 AccumulatedDepreciation_Cents {get; set;}
        public virtual int DepreciationPercentage_0P1Pct {get; set;}
        public virtual Int64 CurrentYearDepreciationAmount_Cents {get; set;}
        public virtual Int64 CurrentYearObjectValue_Cents {get; set;}
        public virtual Int64 PreviousYearObjectValue_Cents {get; set;}
        public virtual bool IsHeading {get; set;}
        public virtual bool IsLeavingThisYear {get; set;}
        public virtual bool IsAggregatingPosition {get; set;}
        public virtual bool IsNonDeprecating {get; set;}
    }
}
