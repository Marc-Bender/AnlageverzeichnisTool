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

            const int A3PaperHeightLandscape_mm = 297;
            const int A3PaperWidthLandscape_mm = 420;
            const int DPI = 96;
            const float mm_per_inch = 25.4f;
            const int pageMargins_mm = 15;

            flowDoc.PageWidth  = A3PaperWidthLandscape_mm / mm_per_inch * DPI;
            flowDoc.PageHeight = (float)A3PaperHeightLandscape_mm / mm_per_inch * DPI;
            flowDoc.PagePadding = new Thickness((float)pageMargins_mm / DPI * mm_per_inch);

            flowDoc.FontFamily = new System.Windows.Media.FontFamily("Courier New");
            flowDoc.FontSize = 12;
            flowDoc.ColumnWidth = double.PositiveInfinity;

            var table = new Table();
            table.CellSpacing = 0;

            // Define columns
            table.Columns.Add(new TableColumn { Width = new GridLength(80) });
            table.Columns.Add(new TableColumn { Width = new GridLength(200) });
            table.Columns.Add(new TableColumn { Width = new GridLength(100) });

            // Header row group
            var headerGroup = new TableRowGroup();
            table.RowGroups.Add(headerGroup);

            var headerRow = new TableRow();
            headerGroup.Rows.Add(headerRow);

            headerRow.Cells.Add(new TableCell(new Paragraph(new Bold(new Run("ID")))));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Bold(new Run("Description")))));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Bold(new Run("Amount")))));

            // Data rows
            var bodyGroup = new TableRowGroup();
            table.RowGroups.Add(bodyGroup);

            foreach (var item in this.DataEntryLines)
            {
                var row = new TableRow();
                bodyGroup.Rows.Add(row);

                row.Cells.Add(new TableCell(new Paragraph(new Run(item.ObjectDescriptionText))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.MonthOfPurchase.ToString()))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.YearOfPurchase.ToString()))));
            }

            // Add table to FlowDocument
            flowDoc.Blocks.Add(table);



            return flowDoc;
        }
    }
}
