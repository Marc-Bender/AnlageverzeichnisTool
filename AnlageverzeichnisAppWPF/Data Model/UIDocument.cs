using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reflection.Metadata;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace AnlageverzeichnisAppWPF
{
    public partial class UIDocument : AbstractAnlageVerzeichnisDocument<UIDocumentHeader, ObservableCollection<UIDataLine>, UIDataLine> 
    {
        [ObservableProperty]
        private UIDocumentHeader header;

        [ObservableProperty]
        private ObservableCollection<UIDataLine> dataEntryLines = [];

        public UIDocument( string companyName, string companyCityAndZipCode, int currentlyWorkedOnYear)
        {
            this.Header = new UIDocumentHeader(companyName, companyCityAndZipCode, currentlyWorkedOnYear);
        }

        public UIDocument()
        {
            this.Header = new UIDocumentHeader();
        }

        public UIDocument(UIDocumentHeader header)
        {
            this.Header = header;
        }

        public void migrateToNextYear() // will change the datastructure inplace
        {
            this.Header.CurrentlyWorkedOnYear++;
            this.DataEntryLines = new ObservableCollection<UIDataLine>(this.DataEntryLines.Where(
                                                                                                    x => (
                                                                                                              (
                                                                                                                    (x.IsLeavingThisYear == false)
                                                                                                                 && (x.IsAggregatingPosition == false)
                                                                                                              )
                                                                                                            ||(
                                                                                                                    (x.IsAggregatingPosition == true)
                                                                                                                 && (this.Header.CurrentlyWorkedOnYear - x.YearOfPurchase <= 4) // maximum difference of 4 yrs b/c deprecation percentage is hardcoded at 20% for aggregate positions...
                                                                                                              )
                                                                                                         )
                                                                                                   ));
            this.applyCurrentYearToImportedDataEntries(); // need to apply the current year to the data lines after modifying to ensure subsequent calculation of derived fields (actual migration) will calculate the correct values
            foreach (var line in DataEntryLines)
            {
                line.IsCalculateDerivedFieldsNeeded = true;
                line.handleCalculateDerivedFieldsOnUpdate();
            }

        }
        public void applyCurrentYearToImportedDataEntries()
        {
            foreach (var line in DataEntryLines)
            {
                line.currentYear = this.Header.CurrentlyWorkedOnYear;
            }
        }
    }
}
