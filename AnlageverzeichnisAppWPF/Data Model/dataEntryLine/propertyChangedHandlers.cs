using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnlageverzeichnisAppWPF
{
    public partial class dataEntryLine : ObservableObject
    {
        //partial void OnMonthOfPurchaseChanged(int oldValue, int newValue) => handleCalculateDerivedFieldsOnUpdate();
        //partial void OnYearOfPurchaseChanged(int oldValue, int newValue) => handleCalculateDerivedFieldsOnUpdate();
        //partial void OnIsLeavingThisYearChanged(bool oldValue, bool newValue) => handleCalculateDerivedFieldsOnUpdate();
        public void handleCalculateDerivedFieldsOnUpdate()
        {
            if(IsCalculateDerivedFieldsNeeded == true)
            {
                calculateDerivedFields(currentYear);
                IsCalculateDerivedFieldsNeeded = false;
            }
        }
    }
}
