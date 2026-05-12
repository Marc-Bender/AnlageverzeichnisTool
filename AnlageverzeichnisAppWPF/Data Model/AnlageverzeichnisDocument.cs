using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
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
            flowDoc.PageWidth = A3PaperAbstraction.widthLandscape_mm / A3PaperAbstraction.mm_per_inch * A3PaperAbstraction.DPI;
            flowDoc.PageHeight = A3PaperAbstraction.heightLandscape_mm / A3PaperAbstraction.mm_per_inch * A3PaperAbstraction.DPI;

            flowDoc.Tag = this;

            var tableCompanyInfo = new Table();
            tableCompanyInfo.CellSpacing = 0;

            /*0*/tableCompanyInfo.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            /*1*/tableCompanyInfo.Columns.Add(new TableColumn { Width = new GridLength(3, GridUnitType.Star) }); // spacer
            
            var companyInfoRowGroup = new TableRowGroup();
            tableCompanyInfo.RowGroups.Add(companyInfoRowGroup);

            var row = new TableRow();
            companyInfoRowGroup.Rows.Add(row);
            row.Cells.Add(new TableCell(new Paragraph(new Run(this.Header.CompanyName))));

            row = new TableRow();
            companyInfoRowGroup.Rows.Add(row);
            row.Cells.Add(new TableCell(new Paragraph(new Run(this.Header.CompanyCityAndZipCode))));

            flowDoc.Blocks.Add(tableCompanyInfo);

            var tableHeader = new Table();
            tableHeader.CellSpacing = 0;
            tableHeader.Margin = new Thickness(0,50,0,0);

            tableHeader.Columns.Add(new TableColumn { Width = new GridLength(2, GridUnitType.Star) });
            tableHeader.Columns.Add(new TableColumn { Width = new GridLength(0.5, GridUnitType.Star) }); //historische anschaffungskosten -- jahr
            tableHeader.Columns.Add(new TableColumn { Width = new GridLength(0.5, GridUnitType.Star) }); //historische anschaffungskosten -- euro
            tableHeader.Columns.Add(new TableColumn { Width = new GridLength(0.25, GridUnitType.Star) });
            tableHeader.Columns.Add(new TableColumn { Width = new GridLength(0.75, GridUnitType.Star) });
            tableHeader.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            tableHeader.Columns.Add(new TableColumn { Width = new GridLength(0.5, GridUnitType.Star) }); //geschäftsjahres abschreibung -- %
            tableHeader.Columns.Add(new TableColumn { Width = new GridLength(0.5, GridUnitType.Star) }); //geschäftsjahres abschreibung -- Euro
            tableHeader.Columns.Add(new TableColumn { Width = new GridLength(0.75, GridUnitType.Star) });
            tableHeader.Columns.Add(new TableColumn { Width = new GridLength(0.75, GridUnitType.Star) });

            var headerRowGroup = new TableRowGroup();
            tableHeader.RowGroups.Add(headerRowGroup);

            row = new TableRow();
            headerRowGroup.Rows.Add(row);

            row.Cells.Add(new TableCell(new Paragraph(new Run("Gegenstand"))));
            row.Cells.Add(new TableCell(new Paragraph(new Run("Historische"))) { ColumnSpan=2});
            row.Cells.Add(new TableCell(new Paragraph(new Run("+"))));
            row.Cells.Add(new TableCell(new Paragraph(new Run("Zugänge"))));
            row.Cells.Add(new TableCell(new Paragraph(new Run("Kumulierte"))));
            row.Cells.Add(new TableCell(new Paragraph(new Run("Geschäftsjahres"))) { ColumnSpan=2});
            row.Cells.Add(new TableCell(new Paragraph(new Run("Aktuelle"))));
            row.Cells.Add(new TableCell(new Paragraph(new Run("Vorjahres"))));

            row = new TableRow();
            headerRowGroup.Rows.Add(row);

            row.Cells.Add(new TableCell(new Paragraph(new Run(""))));
            row.Cells.Add(new TableCell(new Paragraph(new Run("Anschaffungskosten"))) {ColumnSpan=2});
            row.Cells.Add(new TableCell(new Paragraph(new Run("-"))));
            row.Cells.Add(new TableCell(new Paragraph(new Run("Abgänge"))));
            row.Cells.Add(new TableCell(new Paragraph(new Run("Abschreibungen"))));
            row.Cells.Add(new TableCell(new Paragraph(new Run("Abschreibungen"))) { ColumnSpan=2});
            row.Cells.Add(new TableCell(new Paragraph(new Run("Buchwerte"))));
            row.Cells.Add(new TableCell(new Paragraph(new Run("Buchwerte"))));

            row = new TableRow();
            headerRowGroup.Rows.Add(row);

            row.Cells.Add(new TableCell(new Paragraph(new Run(""))));
            row.Cells.Add(new TableCell(new Paragraph(new Run("Herstellungskosten"))) { ColumnSpan=2});
            row.Cells.Add(new TableCell(new Paragraph(new Run(""))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(""))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(""))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(""))) { ColumnSpan=2});
            row.Cells.Add(new TableCell(new Paragraph(new Run($"31.12.{this.Header.CurrentlyWorkedOnYear}"))));
            row.Cells.Add(new TableCell(new Paragraph(new Run($"31.12.{this.Header.CurrentlyWorkedOnYear - 1}"))));

            row = new TableRow();
            headerRowGroup.Rows.Add(row);

            row.Cells.Add(new TableCell(new Paragraph(new Run(""))) { BorderThickness = new Thickness(0, 0, 0, 1), BorderBrush = System.Windows.Media.Brushes.Black}); // nothing for the 2nd line in the 1st col
            row.Cells.Add(new TableCell(new Paragraph(new Run("Jahr"))) { BorderThickness = new Thickness(0, 0, 0, 1), BorderBrush = System.Windows.Media.Brushes.Black });
            row.Cells.Add(new TableCell(new Paragraph(new Run("Euro"))) { BorderThickness = new Thickness(0, 0, 0, 1), BorderBrush = System.Windows.Media.Brushes.Black });
            row.Cells.Add(new TableCell(new Paragraph(new Run(""))) { BorderThickness = new Thickness(0, 0, 0, 1), BorderBrush = System.Windows.Media.Brushes.Black });
            row.Cells.Add(new TableCell(new Paragraph(new Run("Euro"))) { BorderThickness = new Thickness(0, 0, 0, 1), BorderBrush = System.Windows.Media.Brushes.Black });
            row.Cells.Add(new TableCell(new Paragraph(new Run("Euro"))) { BorderThickness = new Thickness(0, 0, 0, 1), BorderBrush = System.Windows.Media.Brushes.Black });
            row.Cells.Add(new TableCell(new Paragraph(new Run("%"))) { BorderThickness = new Thickness(0, 0, 0, 1), BorderBrush = System.Windows.Media.Brushes.Black });
            row.Cells.Add(new TableCell(new Paragraph(new Run("Euro"))) { BorderThickness = new Thickness(0, 0, 0, 1), BorderBrush = System.Windows.Media.Brushes.Black });
            row.Cells.Add(new TableCell(new Paragraph(new Run("Euro"))) { BorderThickness = new Thickness(0, 0, 0, 1), BorderBrush = System.Windows.Media.Brushes.Black });
            row.Cells.Add(new TableCell(new Paragraph(new Run("Euro"))) { BorderThickness = new Thickness(0, 0, 0, 1), BorderBrush = System.Windows.Media.Brushes.Black });
            
            flowDoc.Blocks.Add(tableHeader);
            return flowDoc;
        }
    }
}
