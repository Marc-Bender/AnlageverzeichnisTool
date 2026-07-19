using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnlageverzeichnisAppWPF
{
    public partial class UIDocumentHeader : AbstractDocumentHeader
    {
        [ObservableProperty]
        private string companyName = "Mustermann Fabrikations GmbH";
        [ObservableProperty]
        private string companyCityAndZipCode = "12345 Bad Musterhausen";
        [ObservableProperty]
        private int currentlyWorkedOnYear = 2020;

        public UIDocumentHeader(string companyName, string companyCityAndZipCode, int currentlyWorkedOnYear)
        {
            //must use uppercase members to not break bindings!
            this.CompanyName = companyName;
            this.CompanyCityAndZipCode = companyCityAndZipCode;
            this.CurrentlyWorkedOnYear = currentlyWorkedOnYear;
        }

        public UIDocumentHeader()
        {
            this.CompanyName = "Mustermann Fabrikations GmbH";
            this.CompanyCityAndZipCode = "12345 Bad Musterhausen";
            this.CurrentlyWorkedOnYear = 2020;
        }

        public StoredHeader toStoredHeaderType()
        {
            StoredHeader returnedHeader = new StoredHeader();
            returnedHeader.CompanyName = this.CompanyName;
            returnedHeader.CompanyCityAndZipCode = this.CompanyCityAndZipCode;
            returnedHeader.CurrentlyWorkedOnYear = this.CurrentlyWorkedOnYear;
            return returnedHeader;
        }
    }

}
