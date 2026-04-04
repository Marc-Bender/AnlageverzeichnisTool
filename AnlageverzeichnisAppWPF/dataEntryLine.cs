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
        private string objectDescriptionText = "Gegenstandsbeschreibung";

        [ObservableProperty]
        public int monthOfPurchase = 1;
        
        [ObservableProperty]
        public int yearOfPurchase = 1900;
        
        [ObservableProperty]
        public int priceAtPurchase_Cents = 0;
        
        [ObservableProperty]
        protected int? enterOrLeaveAmount_Cents;
        
        [ObservableProperty]
        protected int accumulatedDepreciation_Cents  = 0;
        
        [ObservableProperty]
        public int depreciationPercentage_0P1Pct  = 0;
        
        [ObservableProperty]
        protected int currentYearDepreciationAmount_Cents  = 0;
        
        [ObservableProperty]
        protected int currentYearObjectValue_Cents  = 0;
        
        [ObservableProperty]
        protected int previousYearObjectValue_Cents  = 0;

        [ObservableProperty]
        public bool isHeading  = false;
        
        [ObservableProperty]
        public bool isLeavingThisYear  = false;
        
        [ObservableProperty]
        public bool displayAsMemorialValue  = false;

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

            // handle the enter and leave amount
            if(
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
                CurrentYearDepreciationAmount_Cents = PriceAtPurchase_Cents * 1000 / DepreciationPercentage_0P1Pct * (12 - MonthOfPurchase + 1) / 12;
                // because in the 1st year the cents must be chosen such that the remaining value is a whole euros value
                int centsThisYear = CurrentYearDepreciationAmount_Cents % 100; 
                CurrentYearDepreciationAmount_Cents -= centsThisYear;
                int centsPurchasePrice = PriceAtPurchase_Cents % 100;
                CurrentYearDepreciationAmount_Cents += centsPurchasePrice;
            }
            else
            {
                CurrentYearDepreciationAmount_Cents = PriceAtPurchase_Cents * 1000 / DepreciationPercentage_0P1Pct;
                int centsThisYear = CurrentYearDepreciationAmount_Cents % 100;
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
