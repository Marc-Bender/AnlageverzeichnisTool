using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AnlageverzeichnisAppWPF
{
    public partial class UIDataLine : AbstractDataLine
    {
        public void calculateDerivedFields(int currentYear)
        {
            if (IsHeading == true)
            {
                return;
            }
            else if (IsNonDeprecating == true)
            {
                DepreciationPercentage_0P1Pct = 0;
                CurrentYearDepreciationAmount_Cents = 0;
                CurrentYearObjectValue_Cents = PriceAtPurchase_Cents;
                PreviousYearObjectValue_Cents = currentYear == YearOfPurchase ? 0 : PriceAtPurchase_Cents;
                if (IsLeavingThisYear == true)
                {
                    EnterOrLeaveAmount_Cents = -PriceAtPurchase_Cents;
                }
                else if (currentYear == YearOfPurchase)
                {
                    EnterOrLeaveAmount_Cents = PriceAtPurchase_Cents;
                }
                else
                {
                    EnterOrLeaveAmount_Cents = 0;
                }
                return;
            }
            else
            {
                // continue calculating...
            }

            if (
                    (DepreciationPercentage_0P1Pct <= 0)
                  ||(
                        (PriceAtPurchase_Cents < 200)
                     && (IsAggregatingPosition == false) // to prove that in certain years there where no aggregating positions zero lines must be allowed in these cases but otherwise a minimum price is meaningful
                    )
                  ||(DepreciationPercentage_0P1Pct >= 1000)
                  ||(
                        // might happen in case of any edits in the datagrid!
                           (currentYear < YearOfPurchase) 
                        || (YearOfPurchase < 1900) // minimum value allowed in entry mask as minimum for allowed values in data grid
                        || (MonthOfPurchase > 12) //same limiting values as for the input field in the 
                        || (MonthOfPurchase < 1)
                    )
                )
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                // not attempting to divide by zero
            }

            // handle the enter and leave amount
            if (currentYear == YearOfPurchase)
            {
                EnterOrLeaveAmount_Cents = PriceAtPurchase_Cents;
            }
            else if (IsLeavingThisYear == true)
            {
                EnterOrLeaveAmount_Cents = -PriceAtPurchase_Cents;
            }
            else
            {
                EnterOrLeaveAmount_Cents = null;
            }

            if (IsAggregatingPosition == true)
            {
                MonthOfPurchase = 1; // by definition
                DepreciationPercentage_0P1Pct = 200; // by definition
                ObjectDescriptionText = "Sammelposten"; // by definition -- strictly this is not needed as the checkbox in the datagrid would show this and since the text upon export is hardcoded anyways but this should be more obvious in the datagrid
            }


            Int64 initialYearDeprecationAmount()
            {
                Int64 CurrentYearDeprecationAmountCalculated = ((PriceAtPurchase_Cents * DepreciationPercentage_0P1Pct) / 1000) * (12 - MonthOfPurchase + 1) / 12;
                // because in the 1st year the cents must be chosen such that the remaining value is a whole euros value
                byte centsThisYear = (byte)(CurrentYearDeprecationAmountCalculated % 100);
                CurrentYearDeprecationAmountCalculated -= centsThisYear;
                byte centsPurchasePrice = (byte)(PriceAtPurchase_Cents % 100);
                CurrentYearDeprecationAmountCalculated += centsPurchasePrice;
                return CurrentYearDeprecationAmountCalculated;
            }

            Int64 subsequentYearDeprecationAmount()
            {
                Int64 CurrentYearDeprecationAmountCalculated = (PriceAtPurchase_Cents * DepreciationPercentage_0P1Pct) / 1000;
                byte centsThisYear = (byte)(CurrentYearDeprecationAmountCalculated % 100);
                CurrentYearDeprecationAmountCalculated -= centsThisYear;
                return CurrentYearDeprecationAmountCalculated;
            }

            // handle the current year deprecation amount
            if (currentYear == YearOfPurchase)
            {
                CurrentYearDepreciationAmount_Cents = initialYearDeprecationAmount();
            }
            else
            {
                CurrentYearDepreciationAmount_Cents = subsequentYearDeprecationAmount();
            }

            // handle the accumulated deprecation amount 
            if (IsLeavingThisYear == true)
            {
                AccumulatedDepreciation_Cents = 0;
            }
            else if (currentYear == YearOfPurchase)
            {
                AccumulatedDepreciation_Cents = CurrentYearDepreciationAmount_Cents;
            }
            else
            {
                AccumulatedDepreciation_Cents = initialYearDeprecationAmount();
                for (int i = 0; i < currentYear - YearOfPurchase; i++)
                {
                    AccumulatedDepreciation_Cents += subsequentYearDeprecationAmount();
                }
                if (IsAggregatingPosition == false)
                {
                    // memorial values are only a thing for non-aggregated positions ...
                    AccumulatedDepreciation_Cents = Math.Min(AccumulatedDepreciation_Cents, PriceAtPurchase_Cents - 100); // to enable setting to memorial value reserve the last 100ct ie 1eur...
                }
                else
                {
                    // .. so in case of aggregating values the last 1EUR is not reserved 
                    AccumulatedDepreciation_Cents = Math.Min(AccumulatedDepreciation_Cents, PriceAtPurchase_Cents); // still clamp the value to the upper logical bound to avoid over-deprecation...
                }
            }

            var currentYearObjectValueTheoretical = PriceAtPurchase_Cents; // initializer -- to be overwritten in the if else below

            // handle the current year remaining value
            if (currentYear == YearOfPurchase)
            {
                CurrentYearObjectValue_Cents = PriceAtPurchase_Cents - CurrentYearDepreciationAmount_Cents;
            }
            else if (IsLeavingThisYear == true)
            {
                CurrentYearObjectValue_Cents = 0; // if the object is leaving it is fully written off by definition ... 
            }
            else
            {
                CurrentYearObjectValue_Cents = PriceAtPurchase_Cents - AccumulatedDepreciation_Cents;
            }

            // handle the previous year remaining value
            if (currentYear == YearOfPurchase)
            {
                PreviousYearObjectValue_Cents = 0; // has not been purchased yet -- thus there is no previous value yet...
            }
            else 
            {
                var accumulatedDepreciationTheoretical = initialYearDeprecationAmount();
                for (int i = 0; i < currentYear - 1 - YearOfPurchase; i++)
                {
                    accumulatedDepreciationTheoretical += subsequentYearDeprecationAmount();
                }
                accumulatedDepreciationTheoretical = Math.Min(accumulatedDepreciationTheoretical, PriceAtPurchase_Cents - 100); // to enable setting to memorial value reserve the last 100ct ie 1eur
                PreviousYearObjectValue_Cents = PriceAtPurchase_Cents - accumulatedDepreciationTheoretical;
            }

            // need to do this check in the very end to ensure that the current year value has been calculated before being used here
            if (
                    (IsLeavingThisYear == false)
               )
            {
                var deprecationAmountThisYearTheoretical = currentYear == YearOfPurchase ? initialYearDeprecationAmount() : subsequentYearDeprecationAmount();
                if(IsAggregatingPosition == false) 
                {
                    CurrentYearDepreciationAmount_Cents = Math.Min(deprecationAmountThisYearTheoretical, Math.Max(PreviousYearObjectValue_Cents, CurrentYearObjectValue_Cents) - 100); // to enable setting to memorial value reserve the last 100ct ie 1eur...
                }
                else
                {
                    // for aggregate positions the last 1EUR is not reserved eitherways so it can be treated as forced to leave... 
                    CurrentYearDepreciationAmount_Cents = Math.Min(deprecationAmountThisYearTheoretical, Math.Max(PreviousYearObjectValue_Cents, CurrentYearObjectValue_Cents));
                }
            }
            else
            {
                CurrentYearDepreciationAmount_Cents = Math.Min(subsequentYearDeprecationAmount(), CurrentYearObjectValue_Cents); // here the last 100ct (ie 1EUR) do not need to be reserved b/c the object is leaving anyways.
            }

        }
    }
}
