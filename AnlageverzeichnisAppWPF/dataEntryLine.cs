using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AnlageverzeichnisAppWPF
{
    public partial class dataEntryLine : ObservableObject
    {
        [ObservableProperty]
        private string objectDescriptionText = "";

        [ObservableProperty]
        public int monthOfPurchase = 1;
        
        [ObservableProperty]
        public int yearOfPurchase = 1900;
        
        [ObservableProperty]
        public Int64 priceAtPurchase_Cents = 0;
        
        [ObservableProperty]
        protected Int64? enterOrLeaveAmount_Cents;
        
        [ObservableProperty]
        protected Int64 accumulatedDepreciation_Cents  = 0;
        
        [ObservableProperty]
        public int depreciationPercentage_0P1Pct  = 0;
        
        [ObservableProperty]
        protected Int64 currentYearDepreciationAmount_Cents  = 0;
        
        [ObservableProperty]
        protected Int64 currentYearObjectValue_Cents  = 0;
        
        [ObservableProperty]
        protected Int64 previousYearObjectValue_Cents  = 0;

        [ObservableProperty]
        public bool isHeading  = false;
        
        [ObservableProperty]
        public bool isLeavingThisYear  = false;
        
        [ObservableProperty]
        public bool displayAsMemorialValue  = false;

        public dataEntryLine()
        {
            // needed for easy json deserialize only! otherwise the constructor with parameters would be used and the deserializer fails!
        }

        public dataEntryLine(int currentYear)
        {
            YearOfPurchase = currentYear; // to allow the data entry mask using this later to always default to the current year for each entry line
        }

        public void calculateDerivedFields(int currentYear)
        {
            if (IsHeading == true)
            {
                return; 
            }
            else
            {
                // continue calculating...
            }
            if (DepreciationPercentage_0P1Pct <= 0)
            {
                throw new NoNullAllowedException();
            }
            else
            {
                // not attempting to divide by zero
            }


            // handle the enter and leave amount
            if (
                    (currentYear == YearOfPurchase)
                  ||(IsLeavingThisYear == true)
              )
            {
                EnterOrLeaveAmount_Cents = PriceAtPurchase_Cents;
            }
            else
            {
                EnterOrLeaveAmount_Cents = null;
            }

            // handle the current year deprecation amount
            if (currentYear == YearOfPurchase)
            {
                CurrentYearDepreciationAmount_Cents = ((PriceAtPurchase_Cents * 1000 / DepreciationPercentage_0P1Pct)/1000) * (12 - MonthOfPurchase + 1) / 12;
                // because in the 1st year the cents must be chosen such that the remaining value is a whole euros value
                Int64 centsThisYear = CurrentYearDepreciationAmount_Cents % 100; 
                CurrentYearDepreciationAmount_Cents -= centsThisYear;
                Int64 centsPurchasePrice = PriceAtPurchase_Cents % 100;
                CurrentYearDepreciationAmount_Cents += centsPurchasePrice;
            }
            else
            {
                CurrentYearDepreciationAmount_Cents = (PriceAtPurchase_Cents * 1000 / DepreciationPercentage_0P1Pct)/1000;
                Int64 centsThisYear = CurrentYearDepreciationAmount_Cents % 100;
                CurrentYearDepreciationAmount_Cents -= centsThisYear;
            }

            // handle the accumulated deprecation amount 
            if (currentYear == YearOfPurchase)
            {
                AccumulatedDepreciation_Cents = CurrentYearDepreciationAmount_Cents;
            }
            else
            {
                AccumulatedDepreciation_Cents += CurrentYearDepreciationAmount_Cents;
            }

        }
    }
}
