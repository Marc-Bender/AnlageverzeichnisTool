using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using System.Text;
using System.Windows;
using System.Windows.Documents;

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
        private DocumentHeader header;

        [ObservableProperty]
        private ObservableCollection<dataEntryLine> dataEntryLines = [];

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
        public FlowDocument toFlowDocument()
        {
            var flowDoc = new FlowDocument();

            flowDoc.FontFamily = new System.Windows.Media.FontFamily("Courier New");
            flowDoc.FontSize = 12;
            flowDoc.ColumnWidth = double.PositiveInfinity;
            flowDoc.Tag = this;

            var table = new Table();
            table.CellSpacing = 0;

            // Define columns
            /*0*/table.Columns.Add(new TableColumn { Width = new GridLength(300) });
            /*1*/table.Columns.Add(new TableColumn { Width = new GridLength(100) });
            /*2*/table.Columns.Add(new TableColumn { Width = new GridLength(70) });
            /*3*/table.Columns.Add(new TableColumn { Width = new GridLength(100) });
            /*4*/table.Columns.Add(new TableColumn { Width = new GridLength(100) });
            /*5*/table.Columns.Add(new TableColumn { Width = new GridLength(100) });
            /*6*/table.Columns.Add(new TableColumn { Width = new GridLength(100) });
            /*7*/table.Columns.Add(new TableColumn { Width = new GridLength(100) });

            // do not draw the table header here and leave that up to the paginator

            // Data rows
            var bodyGroup = new TableRowGroup();
            table.RowGroups.Add(bodyGroup);

            foreach (var item in this.DataEntryLines)
            {
                var row = new TableRow();
                bodyGroup.Rows.Add(row);

                /*0*/row.Cells.Add(new TableCell(new Paragraph(new Run(item.ObjectDescriptionText))));
                /*1*/row.Cells.Add(new TableCell(new Paragraph(new Run($"{item.MonthOfPurchase}/{item.YearOfPurchase}"))));
                string sign = "";
                string enterOrLeaveString = "";
                if(item.EnterOrLeaveAmount_Cents is not null)
                {
                    sign = item.EnterOrLeaveAmount_Cents < 0 ? "-" : "+";
                    enterOrLeaveString = $"{Math.Abs(new Decimal((long)item.EnterOrLeaveAmount_Cents))}";
                }

                /*2*/row.Cells.Add(new TableCell(new Paragraph(new Run($"{sign}"))));
                /*3*/row.Cells.Add(new TableCell(new Paragraph(new Run(enterOrLeaveString))));
                /*4*/row.Cells.Add(new TableCell(new Paragraph(new Run($"{item.AccumulatedDepreciation_Cents}"))));
                /*5*/row.Cells.Add(new TableCell(new Paragraph(new Run($"{item.CurrentYearDepreciationAmount_Cents}"))));
                /*6*/row.Cells.Add(new TableCell(new Paragraph(new Run($"{item.CurrentYearDepreciationAmount_Cents}"))));
                /*7*/row.Cells.Add(new TableCell(new Paragraph(new Run($"{item.PreviousYearObjectValue_Cents}"))));
            }

            // Add table to FlowDocument
            flowDoc.Blocks.Add(table);



            return flowDoc;
        }
    }
}
