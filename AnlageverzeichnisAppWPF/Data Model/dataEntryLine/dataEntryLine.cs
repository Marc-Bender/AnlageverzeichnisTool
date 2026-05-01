using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace AnlageverzeichnisAppWPF
{
    public partial class dataEntryLine : ObservableObject
    {
        [ObservableProperty]
        [JsonIgnore]
        private bool isCalculateDerivedFieldsNeeded;

        [ObservableProperty]
        [JsonIgnore]
        private bool isCurrentHeading = false;

        [JsonIgnore]
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
        private bool displayAsMemorialValue  = false;

        public dataEntryLine()
        {
            // needed for easy json deserialize only! otherwise the constructor with parameters would be used and the deserializer fails!
            // but this also means that when loading from json the current year member will stay at 0 if not initialized otherwise so after loading a file the current year must be set manually for each item!
        }

        public dataEntryLine(int currentYear)
        {
            this.currentYear = currentYear;
            YearOfPurchase = currentYear; // to allow the data entry mask using this later to always default to the current year for each entry line
        }

    }
}
