using System;
using System.Collections.Generic;
using System.Text;

namespace AnlageverzeichnisAppWPF
{
    public class AnlageverzeichnisDocument
    {
        public class Header
        {
            public string companyName { get; set; }
            public string companyCityAndZipCode { get; set; }
            public int currentlyWorkedOnYear { get; set; }
            public Header(string companyName, string companyCityAndZipCode, int currentlyWorkedOnYear)
            {
                this.companyName = companyName;
                this.companyCityAndZipCode = companyCityAndZipCode;
                this.currentlyWorkedOnYear = currentlyWorkedOnYear;
            }
        }

        public Header header { get; set; }
        public List<dataEntryLine> dataEntryLines { get; set; } = [];

        public AnlageverzeichnisDocument( string companyName, string companyCityAndZipCode, int currentlyWorkedOnYear)
        {
            this.header = new Header(companyName, companyCityAndZipCode, currentlyWorkedOnYear);
        }
    }
}
