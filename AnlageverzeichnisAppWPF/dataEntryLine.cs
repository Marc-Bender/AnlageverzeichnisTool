using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AnlageverzeichnisAppWPF
{
    public class dataEntryLine
    {
        public string? objectDescriptionText { get; set; } = null;
        public int monthOfPurchase { get; set; } = 1;
        public int yearOfPurchase { get; set; }
        public int priceAtPurchase_Cents { get; set; }
        protected int? enterOrLeaveAmount_Cents { get; set; }
        protected int accumulatedDepreciation_Cents { get; set; } = 0;
        public int depreciationPercentage_0P1Pct { get; set; } = 0;
        protected int currentYearDepreciationAmount_Cents { get; set; } = 0;
        protected int currentYearObjectValue_Cents { get; set; } = 0;
        protected int previousYearObjectValue_Cents { get; set; } = 0;

        public bool isHeading { get; set; } = false;
        public bool isLeavingThisYear { get; set; } = false;
        public bool displayAsMemorialValue { get; set; } = false;

        public void calculateDerivedFields(int currentYear)
        {
            if (isHeading == true)
            {
                return; 
            }
            else
            {
                // continue calculating...
            }

            // handle the enter and leave amount
            if(
                    (currentYear == yearOfPurchase)
                  ||(isLeavingThisYear == true)
              )
            {
                enterOrLeaveAmount_Cents = priceAtPurchase_Cents;
            }
            else
            {
                enterOrLeaveAmount_Cents = null;
            }

            // handle the current year deprecation amount
            if (currentYear == yearOfPurchase)
            {
                currentYearDepreciationAmount_Cents = priceAtPurchase_Cents * 1000 / depreciationPercentage_0P1Pct * (12 - monthOfPurchase + 1) / 12;
                // because in the 1st year the cents must be chosen such that the remaining value is a whole euros value
                int centsThisYear = currentYearDepreciationAmount_Cents % 100; 
                currentYearDepreciationAmount_Cents -= centsThisYear;
                int centsPurchasePrice = priceAtPurchase_Cents % 100;
                currentYearDepreciationAmount_Cents += centsPurchasePrice;
            }
            else
            {
                currentYearDepreciationAmount_Cents = priceAtPurchase_Cents * 1000 / depreciationPercentage_0P1Pct;
                int centsThisYear = currentYearDepreciationAmount_Cents % 100;
                currentYearDepreciationAmount_Cents -= centsThisYear;
            }

            // handle the accumulated deprecation amount 
            if (currentYear == yearOfPurchase)
            {
                accumulatedDepreciation_Cents = currentYearDepreciationAmount_Cents;
            }
            else
            {
                accumulatedDepreciation_Cents += currentYearDepreciationAmount_Cents;
            }

        }
    }
}
