using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnlageverzeichnisAppWPF
{
    public partial class UIDataLine : AbstractDataLine
    {
        partial void OnIsHeadingChanged(bool oldValue, bool newValue)
        {
            if (newValue == true)
            {
                IsLeavingThisYear = false;
            }
        }
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
