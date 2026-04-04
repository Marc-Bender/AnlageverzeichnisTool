using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnlageverzeichnisAppWPF
{
    public partial class AnlageverzeichnisDocument : ObservableObject
    {
        public partial class DocumentHeader : ObservableObject
        {
            [ObservableProperty]
            private string companyName = "Mustermann Fabrikations GmbH";
            [ObservableProperty]
            private string companyCityAndZipCode = "12345 Bad Musterhausen";
            [ObservableProperty]
            private int currentlyWorkedOnYear = 2020;

            public DocumentHeader(string companyName, string companyCityAndZipCode, int currentlyWorkedOnYear)
            {
                //must use uppercase members to not break bindings!
                this.CompanyName = companyName;
                this.CompanyCityAndZipCode = companyCityAndZipCode;
                this.CurrentlyWorkedOnYear = currentlyWorkedOnYear;
            }

            public DocumentHeader()
            {
                this.CompanyName = "Mustermann Fabrikations GmbH";
                this.CompanyCityAndZipCode = "12345 Bad Musterhausen";
                this.CurrentlyWorkedOnYear = 2020;
            }
        }

        [ObservableProperty]
        public DocumentHeader header;

        [ObservableProperty]
        public List<dataEntryLine> dataEntryLines = [];

        public AnlageverzeichnisDocument( string companyName, string companyCityAndZipCode, int currentlyWorkedOnYear)
        {
            this.Header = new DocumentHeader(companyName, companyCityAndZipCode, currentlyWorkedOnYear);
        }

        public AnlageverzeichnisDocument()
        {
            this.Header = new DocumentHeader();
        }

        public AnlageverzeichnisDocument(DocumentHeader header)
        {
            this.Header = header;
        }

        public void migrateToNextYear() // will change the datastructure inplace
        {
            this.Header.CurrentlyWorkedOnYear++;

        }
    }
}
