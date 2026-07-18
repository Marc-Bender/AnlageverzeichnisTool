using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnlageverzeichnisAppWPF
{
    public abstract class AbstractDocumentHeader : ObservableObject
    {
        virtual public string CompanyName { get; set; }
        virtual public string CompanyCityAndZipCode { get; set; }
        virtual public int CurrentlyWorkedOnYear { get; set; }
        public AbstractDocumentHeader(string companyName, string companyCityAndZipCode, int currentlyWorkedOnYear)
        {
            this.CompanyName = companyName;
            this.CompanyCityAndZipCode = companyCityAndZipCode;
            this.CurrentlyWorkedOnYear = currentlyWorkedOnYear;
        }

        public AbstractDocumentHeader()
        {
            this.CompanyName = "Mustermann Fabrikations GmbH";
            this.CompanyCityAndZipCode = "12345 Bad Musterhausen";
            this.CurrentlyWorkedOnYear = 2020;
        }
    }
}
