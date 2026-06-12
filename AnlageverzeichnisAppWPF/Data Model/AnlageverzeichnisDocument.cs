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
    public class PrintedDocumentAbstraction
    {
        public class headerTableColumnWidths
        {
            public static GridLength objectDescription => new GridLength(1.5, GridUnitType.Star);
            public static GridLength dateOfPurchase => new GridLength(0.3, GridUnitType.Star);
            public static GridLength priceAtPurchase => new GridLength(0.5, GridUnitType.Star);
            public static GridLength plusMinus => new GridLength(0.15, GridUnitType.Star);
            public static GridLength enterAndLeaveAmount => new GridLength(0.5, GridUnitType.Star);
            public static GridLength accumulatedDeprecationAmount => new GridLength(0.5, GridUnitType.Star);
            public static GridLength currentYearDeprecationPercentage => new GridLength(0.25, GridUnitType.Star);
            public static GridLength currentYearDeprecationAmount => new GridLength(0.3, GridUnitType.Star);
            public static GridLength currentYearObjectValue => new GridLength(0.5, GridUnitType.Star);
            public static GridLength previousYearObjectValue => new GridLength(0.5, GridUnitType.Star);
        }

        public class sectionSumTableColumnWidths
        {
            public static GridLength spacer => new GridLength(
                                                                    PrintedDocumentAbstraction.headerTableColumnWidths.objectDescription.Value
                                                                  + PrintedDocumentAbstraction.headerTableColumnWidths.dateOfPurchase.Value                
                                                                , GridUnitType.Star
                                                             );
            public static GridLength priceAtPurchase => PrintedDocumentAbstraction.headerTableColumnWidths.priceAtPurchase;
            public static GridLength plusMinus => PrintedDocumentAbstraction.headerTableColumnWidths.plusMinus;
            public static GridLength enterAndLeaveAmount => PrintedDocumentAbstraction.headerTableColumnWidths.enterAndLeaveAmount;
            public static GridLength accumulatedDeprecationAmount => PrintedDocumentAbstraction.headerTableColumnWidths.accumulatedDeprecationAmount;
            public static GridLength spacer2 => PrintedDocumentAbstraction.headerTableColumnWidths.currentYearDeprecationPercentage;
            public static GridLength currentYearDeprecationAmount => PrintedDocumentAbstraction.headerTableColumnWidths.currentYearDeprecationAmount;
            public static GridLength currentYearObjectValue => PrintedDocumentAbstraction.headerTableColumnWidths.currentYearObjectValue;
            public static GridLength previousYearObjectValue => PrintedDocumentAbstraction.headerTableColumnWidths.previousYearObjectValue;
        }
        public class pageEndSumTableColumnWidths
        {
            public static GridLength pageEndSumTableLabel => new GridLength(
                                                                                  PrintedDocumentAbstraction.headerTableColumnWidths.objectDescription.Value
                                                                                + PrintedDocumentAbstraction.headerTableColumnWidths.dateOfPurchase.Value                
                                                                            , GridUnitType.Star
                                                                           );
            public static GridLength priceAtPurchase => PrintedDocumentAbstraction.headerTableColumnWidths.priceAtPurchase;
            public static GridLength spacer => new GridLength(
                                                                    PrintedDocumentAbstraction.headerTableColumnWidths.plusMinus.Value
                                                                  + PrintedDocumentAbstraction.headerTableColumnWidths.enterAndLeaveAmount.Value
                                                               , GridUnitType.Star
                                                             );
            public static GridLength accumulatedDeprecationAmount => PrintedDocumentAbstraction.headerTableColumnWidths.accumulatedDeprecationAmount;
            public static GridLength spacer2 => PrintedDocumentAbstraction.headerTableColumnWidths.currentYearDeprecationPercentage;
            public static GridLength currentYearDeprecationAmount => PrintedDocumentAbstraction.headerTableColumnWidths.currentYearDeprecationAmount;
            public static GridLength currentYearObjectValue => PrintedDocumentAbstraction.headerTableColumnWidths.currentYearObjectValue;
            public static GridLength previousYearObjectValue => PrintedDocumentAbstraction.headerTableColumnWidths.previousYearObjectValue;
        }

        public class sumValues
        {
            public Int64 priceAtPurchase_cent { get; set; }
            public Int64 enterAmount_cent { get; set; }
            public Int64 leaveAmount_cent { get; set; }
            public Int64 accumulatedDeprecation_cent { get; set; }
            public Int64 currentYearDeprecation_cent { get; set; }
            public Int64 currentYearObjectValue_cent { get; set; }
            public Int64 previousYearObjectValue_cent { get; set; }
        }
    }
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

        private void _addCellToRow(TableRow row, string text, int columnSpan = 1, TextAlignment textAlignment = TextAlignment.Left, Thickness? borderThickness = null, TextDecorationCollection? textDecorations = null, bool isItalics = false, bool isBold = false)
        {
            Inline textRun = new Run(text);
            if (isBold==true)
            {
                textRun = new Bold(textRun);
            }
            if (isItalics==true)
            {
                textRun = new Italic(textRun);
            }

            var paragraph = new Paragraph(textRun)
                {
                    TextAlignment = textAlignment
                };
            if (textDecorations is not null)
            {
                paragraph.TextDecorations = (TextDecorationCollection)textDecorations;
            }

            row.Cells.Add(
                            new TableCell(paragraph) 
                                { 
                                    ColumnSpan = columnSpan, 
                                    BorderThickness = borderThickness is not null ? (Thickness)borderThickness : new Thickness(0),
                                    BorderBrush = borderThickness is not null ? System.Windows.Media.Brushes.Black : System.Windows.Media.Brushes.White
                                }
                         );
        }
        private string _numericToFormatedString<T>(T number, IValueConverter converter) => (string)converter.Convert(number, Type.GetType("string"), new object(), new System.Globalization.CultureInfo("de-DE"));
        public FlowDocument generateHeaderFlowDocument()
        {
            var flowDoc = new FlowDocument();

            flowDoc.FontFamily = new System.Windows.Media.FontFamily("Courier New");
            flowDoc.FontSize = 12;
            flowDoc.ColumnWidth = double.PositiveInfinity;
            flowDoc.PageWidth = A3PaperAbstraction.mm_to_diu(A3PaperAbstraction.widthLandscape_mm);
            flowDoc.MaxPageHeight = A3PaperAbstraction.mm_to_diu(A3PaperAbstraction.heightLandscape_mm);
            flowDoc.PagePadding = new Thickness(0);

            flowDoc.Tag = this;

            var tableCompanyInfo = new Table();
            tableCompanyInfo.CellSpacing = 0;

            /*0*/tableCompanyInfo.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            /*1*/tableCompanyInfo.Columns.Add(new TableColumn { Width = new GridLength(3, GridUnitType.Star) }); // spacer
            
            var companyInfoRowGroup = new TableRowGroup();
            tableCompanyInfo.RowGroups.Add(companyInfoRowGroup);

            var row = new TableRow();
            companyInfoRowGroup.Rows.Add(row);
            _addCellToRow(row, this.Header.CompanyName);

            row = new TableRow();
            companyInfoRowGroup.Rows.Add(row);
            _addCellToRow(row, this.Header.CompanyCityAndZipCode);

            row = new TableRow();
            companyInfoRowGroup.Rows.Add(row);
            _addCellToRow(row, $"Anlageverzeichnis zum 31.12.{this.Header.CurrentlyWorkedOnYear}", textAlignment:TextAlignment.Center, isBold: true, textDecorations: TextDecorations.Underline);

            flowDoc.Blocks.Add(tableCompanyInfo);


            var tableHeader = new Table();
            tableHeader.CellSpacing = 0;
            tableHeader.Margin = new Thickness(0,25,0,0);

            tableHeader.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.headerTableColumnWidths.objectDescription});
            tableHeader.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.headerTableColumnWidths.dateOfPurchase});
            tableHeader.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.headerTableColumnWidths.priceAtPurchase}); 
            tableHeader.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.headerTableColumnWidths.plusMinus});
            tableHeader.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.headerTableColumnWidths.enterAndLeaveAmount});
            tableHeader.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.headerTableColumnWidths.accumulatedDeprecationAmount});
            tableHeader.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.headerTableColumnWidths.currentYearDeprecationPercentage});
            tableHeader.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.headerTableColumnWidths.currentYearDeprecationAmount});
            tableHeader.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.headerTableColumnWidths.currentYearObjectValue});
            tableHeader.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.headerTableColumnWidths.previousYearObjectValue});

            var headerRowGroup = new TableRowGroup();
            tableHeader.RowGroups.Add(headerRowGroup);

            row = new TableRow();
            headerRowGroup.Rows.Add(row);

            _addCellToRow(row, "Gegenstand");
            _addCellToRow(row, "Historische", 2, TextAlignment.Center);
            _addCellToRow(row, "+", textAlignment: TextAlignment.Center);
            _addCellToRow(row, "Zugänge", textAlignment: TextAlignment.Right);
            _addCellToRow(row, "Kumulierte", textAlignment: TextAlignment.Center);
            _addCellToRow(row, "Geschäftsjahres", columnSpan: 2, textAlignment: TextAlignment.Center);
            _addCellToRow(row, "Aktuelle", textAlignment: TextAlignment.Right);
            _addCellToRow(row, "Vorjahres", textAlignment: TextAlignment.Right);

            row = new TableRow();
            headerRowGroup.Rows.Add(row);

            _addCellToRow(row, "\u00A0");
            _addCellToRow(row, "Anschaffungskosten", 2, TextAlignment.Center);
            _addCellToRow(row, "-", textAlignment: TextAlignment.Center);
            _addCellToRow(row, "Abgänge", textAlignment: TextAlignment.Right);
            _addCellToRow(row, "Abschreibungen", textAlignment: TextAlignment.Center);
            _addCellToRow(row, "Abschreibungen", 2, TextAlignment.Center);
            _addCellToRow(row, "Buchwerte", textAlignment: TextAlignment.Right);
            _addCellToRow(row, "Buchwerte", textAlignment: TextAlignment.Right);

            row = new TableRow();
            headerRowGroup.Rows.Add(row);

            _addCellToRow(row, "\u00A0");
            _addCellToRow(row, "Herstellungskosten", 2, TextAlignment.Center);
            _addCellToRow(row, "\u00A0", textAlignment: TextAlignment.Center);
            _addCellToRow(row, "\u00A0", textAlignment: TextAlignment.Right);
            _addCellToRow(row, "\u00A0", textAlignment: TextAlignment.Center);
            _addCellToRow(row, "\u00A0", 2, TextAlignment.Center);
            _addCellToRow(row, $"31.12.{this.Header.CurrentlyWorkedOnYear}", textAlignment: TextAlignment.Right);
            _addCellToRow(row, $"31.12.{this.Header.CurrentlyWorkedOnYear - 1}", textAlignment: TextAlignment.Right);

            row = new TableRow();
            headerRowGroup.Rows.Add(row);

            _addCellToRow(row, "\u00A0", borderThickness: new Thickness(0, 0, 0, 1)); // nothing for the 2nd line in the 1st col
            _addCellToRow(row, "Jahr", textAlignment: TextAlignment.Center, borderThickness: new Thickness(0, 0, 0, 1));
            _addCellToRow(row, "Euro", textAlignment: TextAlignment.Right, borderThickness: new Thickness(0, 0, 0, 1));
            _addCellToRow(row, "\u00A0", textAlignment: TextAlignment.Center, borderThickness: new Thickness(0, 0, 0, 1));
            _addCellToRow(row, "Euro", textAlignment: TextAlignment.Right, borderThickness: new Thickness(0, 0, 0, 1));
            _addCellToRow(row, "Euro", textAlignment: TextAlignment.Right, borderThickness: new Thickness(0, 0, 0, 1));
            _addCellToRow(row, "%", textAlignment: TextAlignment.Center, borderThickness: new Thickness(0, 0, 0, 1));
            _addCellToRow(row, "Euro", textAlignment: TextAlignment.Right, borderThickness: new Thickness(0, 0, 0, 1));
            _addCellToRow(row, "Euro", textAlignment: TextAlignment.Right, borderThickness: new Thickness(0, 0, 0, 1));
            _addCellToRow(row, "Euro", textAlignment: TextAlignment.Right, borderThickness: new Thickness(0, 0, 0, 1));
            
            flowDoc.Blocks.Add(tableHeader);
            return flowDoc;
        }

        public FlowDocument generateTableLineFlowDocument(int index)
        {
            var flowDoc = new FlowDocument();
            flowDoc.FontFamily = new System.Windows.Media.FontFamily("Courier New");
            flowDoc.FontSize = 12;
            flowDoc.ColumnWidth = double.PositiveInfinity;
            flowDoc.PageWidth = A3PaperAbstraction.mm_to_diu(A3PaperAbstraction.widthLandscape_mm);
            flowDoc.MaxPageHeight = A3PaperAbstraction.mm_to_diu(A3PaperAbstraction.heightLandscape_mm);
            flowDoc.PagePadding = new Thickness(0); // absolutely required in order to not draw the padding area in the paginator later! this would cause the data line to appear to be invisible but have the correct size eitherway
            flowDoc.Tag = this;

            var dataLineTable = new Table();
            dataLineTable.CellSpacing = 0;
            /*0*/dataLineTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.headerTableColumnWidths.objectDescription });
            /*1*/dataLineTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.headerTableColumnWidths.dateOfPurchase });
            /*2*/dataLineTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.headerTableColumnWidths.priceAtPurchase });
            /*3*/dataLineTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.headerTableColumnWidths.plusMinus });
            /*4*/dataLineTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.headerTableColumnWidths.enterAndLeaveAmount });
            /*5*/dataLineTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.headerTableColumnWidths.accumulatedDeprecationAmount });
            /*6*/dataLineTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.headerTableColumnWidths.currentYearDeprecationPercentage });
            /*7*/dataLineTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.headerTableColumnWidths.currentYearDeprecationAmount });
            /*8*/dataLineTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.headerTableColumnWidths.currentYearObjectValue });
            /*8*/dataLineTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.headerTableColumnWidths.previousYearObjectValue });

            var dataLineTableRowGroup = new TableRowGroup();
            dataLineTable.RowGroups.Add(dataLineTableRowGroup);

            var line = this.DataEntryLines[index];
            var row = new TableRow();
            dataLineTableRowGroup.Rows.Add(row);
            if (line.IsHeading == true)
            {
                _addCellToRow(row, line.ObjectDescriptionText, textDecorations: TextDecorations.Underline, isItalics: true);
                flowDoc.Blocks.Add(dataLineTable);

                return flowDoc;
            }
            else
            {
                _addCellToRow(row, line.ObjectDescriptionText);
            }

            var dateConverter = new dateOfPurchaseConverter();
            var date = (string)dateConverter.Convert([line.MonthOfPurchase, line.YearOfPurchase], Type.GetType("string"), new object(), new System.Globalization.CultureInfo("de-DE"));
            _addCellToRow(row, date, textAlignment: TextAlignment.Center);
            var centsConverter = new CentsToEuroStringConverter();
            _addCellToRow(row, _numericToFormatedString(line.PriceAtPurchase_Cents, centsConverter), textAlignment: TextAlignment.Right);
            if (line.EnterOrLeaveAmount_Cents is not null)
            {
                _addCellToRow(row, line.EnterOrLeaveAmount_Cents >= 0 ? "+" : "-", textAlignment: TextAlignment.Center);
                _addCellToRow(row, _numericToFormatedString(Math.Abs((long)line.EnterOrLeaveAmount_Cents), centsConverter), textAlignment: TextAlignment.Right);                    
            }
            else
            {
                _addCellToRow(row, "\u00A0");
                _addCellToRow(row, "\u00A0");
            }

            _addCellToRow(row, _numericToFormatedString(line.AccumulatedDepreciation_Cents, centsConverter), textAlignment: TextAlignment.Right);

            var percentageConverter = new TenthPctToPercentageString();
            _addCellToRow(row, _numericToFormatedString(line.DepreciationPercentage_0P1Pct, percentageConverter), textAlignment: TextAlignment.Center);
                
            _addCellToRow(row, _numericToFormatedString(line.CurrentYearDepreciationAmount_Cents, centsConverter), textAlignment: TextAlignment.Right);
            _addCellToRow(row, _numericToFormatedString(line.CurrentYearObjectValue_Cents, centsConverter), textAlignment: TextAlignment.Right);
            _addCellToRow(row, _numericToFormatedString(line.PreviousYearObjectValue_Cents, centsConverter), textAlignment: TextAlignment.Right);    

            flowDoc.Blocks.Add(dataLineTable);

            return flowDoc;
        }

        public List<FlowDocument> generatePerTableLineFlowDocuments()
        {
            List<FlowDocument> flowDocuments = new List<FlowDocument>();
            for(int i=0;i<this.DataEntryLines.Count;i++)
            {
                flowDocuments.Add(generateTableLineFlowDocument(i));
            }
            return flowDocuments;
        }
        public FlowDocument generateTotalsSumFlowDocument()
        {
            var flowDoc = new FlowDocument();
            flowDoc.FontFamily = new System.Windows.Media.FontFamily("Courier New");
            flowDoc.FontSize = 12;
            flowDoc.ColumnWidth = double.PositiveInfinity;
            flowDoc.PageWidth = A3PaperAbstraction.mm_to_diu(A3PaperAbstraction.widthLandscape_mm);
            flowDoc.PageHeight = A3PaperAbstraction.mm_to_diu(A3PaperAbstraction.heightLandscape_mm);
            flowDoc.PagePadding = new Thickness(0);
            flowDoc.Tag = this;

            var totalsSumTable = new Table();
            totalsSumTable.CellSpacing = 0;
            /*0*/totalsSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.spacer });
            /*1*/totalsSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.priceAtPurchase });
            /*2*/totalsSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.plusMinus });
            /*3*/totalsSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.enterAndLeaveAmount });
            /*4*/totalsSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.accumulatedDeprecationAmount });
            /*5*/totalsSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.spacer2 });
            /*6*/totalsSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.currentYearDeprecationAmount });
            /*7*/totalsSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.currentYearObjectValue });
            /*8*/totalsSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.previousYearObjectValue });

            var totalsSumTableRowGroup = new TableRowGroup();
            totalsSumTable.RowGroups.Add(totalsSumTableRowGroup);

            var totalsSumTableValues = new PrintedDocumentAbstraction.sumValues();
            foreach (var line in this.DataEntryLines)
            {
                totalsSumTableValues.priceAtPurchase_cent += line.PriceAtPurchase_Cents;
                totalsSumTableValues.enterAmount_cent += line.EnterOrLeaveAmount_Cents is not null && line.EnterOrLeaveAmount_Cents > 0 ? (long)line.EnterOrLeaveAmount_Cents : 0;
                totalsSumTableValues.leaveAmount_cent += line.EnterOrLeaveAmount_Cents is not null && line.EnterOrLeaveAmount_Cents < 0 ? (long)-line.EnterOrLeaveAmount_Cents : 0;
                totalsSumTableValues.accumulatedDeprecation_cent += line.AccumulatedDepreciation_Cents;
                totalsSumTableValues.currentYearDeprecation_cent += line.CurrentYearDepreciationAmount_Cents;
                totalsSumTableValues.currentYearObjectValue_cent += line.CurrentYearObjectValue_Cents;
                totalsSumTableValues.previousYearObjectValue_cent += line.PreviousYearObjectValue_Cents;
            }

            var totalsSumTableThicknessFirstLine = new Thickness(
                                                                    0,
                                                                    1,
                                                                    0,
                                                                    totalsSumTableValues.leaveAmount_cent != 0 ? 0 : 1 // if there is a leave amount there is a seperate line being added to the sum table thus the bottom line will be the next line thus this line will not need a bottom border in that case only
                                                               );

            var row = new TableRow();
            totalsSumTableRowGroup.Rows.Add(row);

            _addCellToRow(row, "G e s a m t s u m m e n", borderThickness: totalsSumTableThicknessFirstLine);

            var centsConverter = new CentsToEuroStringConverter();

            _addCellToRow(row, _numericToFormatedString(totalsSumTableValues.priceAtPurchase_cent, centsConverter), borderThickness: totalsSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);
            _addCellToRow(row, totalsSumTableValues.enterAmount_cent != 0 ? "+" : "\u00A0", borderThickness: totalsSumTableThicknessFirstLine, textAlignment: TextAlignment.Center);
            _addCellToRow(row, totalsSumTableValues.enterAmount_cent != 0 ? _numericToFormatedString(totalsSumTableValues.enterAmount_cent, centsConverter) : "\u00A0", borderThickness: totalsSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);
            _addCellToRow(row, _numericToFormatedString(totalsSumTableValues.accumulatedDeprecation_cent, centsConverter), borderThickness: totalsSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);
            _addCellToRow(row, "\u00A0", borderThickness: totalsSumTableThicknessFirstLine);
            _addCellToRow(row, _numericToFormatedString(totalsSumTableValues.currentYearDeprecation_cent, centsConverter), borderThickness: totalsSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);
            _addCellToRow(row, _numericToFormatedString(totalsSumTableValues.currentYearObjectValue_cent, centsConverter), borderThickness: totalsSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);
            _addCellToRow(row, _numericToFormatedString(totalsSumTableValues.previousYearObjectValue_cent, centsConverter), borderThickness: totalsSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);
 
            if (totalsSumTableValues.leaveAmount_cent != 0)
            {
                var sectionSumTableThicknessLeaveLine = new Thickness(0, 0, 0, 1);

                row = new TableRow();
                totalsSumTableRowGroup.Rows.Add(row);
                _addCellToRow(row, "\u00A0", borderThickness: sectionSumTableThicknessLeaveLine);
                _addCellToRow(row, "\u00A0", borderThickness: sectionSumTableThicknessLeaveLine);
                _addCellToRow(row, totalsSumTableValues.leaveAmount_cent != 0 ? "-" : "\u00A0", borderThickness: sectionSumTableThicknessLeaveLine, textAlignment: TextAlignment.Center);
                _addCellToRow(row, totalsSumTableValues.leaveAmount_cent != 0 ? _numericToFormatedString(totalsSumTableValues.leaveAmount_cent, centsConverter) : "\u00A0", borderThickness: sectionSumTableThicknessLeaveLine, textAlignment: TextAlignment.Right);
                for (int i = 0;i<5;i++)
                {
                    _addCellToRow(row, "\u00A0", borderThickness: sectionSumTableThicknessLeaveLine);
                }
            }

            var spacer = new TableRow();

            for (int i = 0; i < 9; i++)
            {
                var rect = new System.Windows.Shapes.Rectangle
                {
                    Height = 2, // tiny gap
                    Fill = System.Windows.Media.Brushes.Transparent
                };

                var ui = new BlockUIContainer(rect)
                {
                    Margin = new Thickness(0),
                    Padding = new Thickness(0)
                };

                var cell = new TableCell(ui)
                {
                    BorderThickness = new Thickness(0),
                    Padding = new Thickness(0)
                };

                spacer.Cells.Add(cell);
            }

            totalsSumTableRowGroup.Rows.Add(spacer);

            var rowForDoubleUnderlineSecondLine = new TableRow();
            for (int i = 0; i < 9; i++)
            {
                var spacerCell = new TableCell(new Paragraph(new Run("\u00A0"))) // linebreak as a placeholder as a cell must not be totally empty for WPF to work
                {
                    BorderThickness = new Thickness(0, 1, 0, 0),
                    BorderBrush = System.Windows.Media.Brushes.Black,
                    LineHeight = 2
                };
                rowForDoubleUnderlineSecondLine.Cells.Add(spacerCell);
            }
            totalsSumTableRowGroup.Rows.Add(rowForDoubleUnderlineSecondLine);


            flowDoc.Blocks.Add(totalsSumTable);



            return flowDoc;
        }

        public FlowDocument generateSectionSumFlowDocument(int sectionStartDataLineIndexNumber, int sectionEndDataLineIndexNumber)
        {
            var sectionStartDataLineIndex = new Index(sectionStartDataLineIndexNumber);
            var sectionEndDataLineIndex = new Index(sectionEndDataLineIndexNumber);

            var flowDoc = new FlowDocument();
            flowDoc.FontFamily = new System.Windows.Media.FontFamily("Courier New");
            flowDoc.FontSize = 12;
            flowDoc.ColumnWidth = double.PositiveInfinity;
            flowDoc.PageWidth = A3PaperAbstraction.mm_to_diu(A3PaperAbstraction.widthLandscape_mm);
            flowDoc.PageHeight = A3PaperAbstraction.mm_to_diu(A3PaperAbstraction.heightLandscape_mm);
            flowDoc.PagePadding = new Thickness(0);

            flowDoc.Tag = this;

            var sectionSumTable = new Table();
            sectionSumTable.CellSpacing = 0;
            /*0*/sectionSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.spacer});
            /*1*/sectionSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.priceAtPurchase});
            /*2*/sectionSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.plusMinus});
            /*3*/sectionSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.enterAndLeaveAmount});
            /*4*/sectionSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.accumulatedDeprecationAmount});
            /*5*/sectionSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.spacer2});
            /*6*/sectionSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.currentYearDeprecationAmount});
            /*7*/sectionSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.currentYearObjectValue});
            /*8*/sectionSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.previousYearObjectValue});

            var sectionSumTableRowGroup = new TableRowGroup();
            sectionSumTable.RowGroups.Add(sectionSumTableRowGroup);

            var sectionSumValues = new PrintedDocumentAbstraction.sumValues();
            foreach (var line in this.DataEntryLines.Take(new Range(sectionStartDataLineIndex, sectionEndDataLineIndex)))
            {
                    sectionSumValues.priceAtPurchase_cent += line.PriceAtPurchase_Cents;
                    sectionSumValues.enterAmount_cent += line.EnterOrLeaveAmount_Cents is not null && line.EnterOrLeaveAmount_Cents > 0 ? (long)line.EnterOrLeaveAmount_Cents : 0;
                    sectionSumValues.leaveAmount_cent += line.EnterOrLeaveAmount_Cents is not null && line.EnterOrLeaveAmount_Cents < 0 ? (long)-line.EnterOrLeaveAmount_Cents : 0;
                    sectionSumValues.accumulatedDeprecation_cent += line.AccumulatedDepreciation_Cents;
                    sectionSumValues.currentYearDeprecation_cent += line.CurrentYearDepreciationAmount_Cents;
                    sectionSumValues.currentYearObjectValue_cent += line.CurrentYearObjectValue_Cents;
                    sectionSumValues.previousYearObjectValue_cent += line.PreviousYearObjectValue_Cents;
            }

            var sectionSumTableThicknessFirstLine = new Thickness(
                                                                    0,
                                                                    1,
                                                                    0,
                                                                    sectionSumValues.leaveAmount_cent != 0 ? 0 : 1 // if there is a leave amount there is a seperate line being added to the sum table thus the bottom line will be the next line thus this line will not need a bottom border in that case only
                                                               );

            var row = new TableRow();
            sectionSumTableRowGroup.Rows.Add(row);
            _addCellToRow(row, "A b s c h n i t t s s u m m e n");

            var centsConverter = new CentsToEuroStringConverter();
            _addCellToRow(row, _numericToFormatedString(sectionSumValues.priceAtPurchase_cent, centsConverter), borderThickness: sectionSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);
            _addCellToRow(row, sectionSumValues.enterAmount_cent != 0 ? "+" : "\u00A0", borderThickness: sectionSumTableThicknessFirstLine, textAlignment: TextAlignment.Center);
            _addCellToRow(row, sectionSumValues.enterAmount_cent != 0 ? _numericToFormatedString(sectionSumValues.enterAmount_cent, centsConverter) : "\u00A0", borderThickness: sectionSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);
            _addCellToRow(row, _numericToFormatedString(sectionSumValues.accumulatedDeprecation_cent, centsConverter), borderThickness: sectionSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);
            _addCellToRow(row, "\u00A0", borderThickness: sectionSumTableThicknessFirstLine);
            _addCellToRow(row, _numericToFormatedString(sectionSumValues.currentYearDeprecation_cent, centsConverter), borderThickness: sectionSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);
            _addCellToRow(row, _numericToFormatedString(sectionSumValues.currentYearObjectValue_cent, centsConverter), borderThickness: sectionSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);
            _addCellToRow(row, _numericToFormatedString(sectionSumValues.previousYearObjectValue_cent, centsConverter), borderThickness: sectionSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);

            if(sectionSumValues.leaveAmount_cent != 0)
            {
                var sectionSumTableThicknessLeaveLine = new Thickness(0,0,0,1);

                row = new TableRow();
                sectionSumTableRowGroup.Rows.Add(row);
                _addCellToRow(row, "\u00A0");
                _addCellToRow(row, "\u00A0", borderThickness: sectionSumTableThicknessLeaveLine);
                _addCellToRow(row, sectionSumValues.leaveAmount_cent != 0 ? "-" : "\u00A0", borderThickness: sectionSumTableThicknessLeaveLine, textAlignment: TextAlignment.Center);
                _addCellToRow(row, sectionSumValues.leaveAmount_cent != 0 ? _numericToFormatedString(sectionSumValues.leaveAmount_cent, centsConverter) : "\u00A0", borderThickness: sectionSumTableThicknessLeaveLine, textAlignment: TextAlignment.Right);
                for(int i=0;i<5;i++)
                {
                    _addCellToRow(row, "\u00A0", borderThickness: sectionSumTableThicknessLeaveLine);
                }
            }

            var spacer = new TableRow();
            
            for (int i=0;i<9;i++)
            {
                var rect = new System.Windows.Shapes.Rectangle
                {
                    Height = 2, // tiny gap
                    Fill = System.Windows.Media.Brushes.Transparent
                };

                var ui = new BlockUIContainer(rect)
                {
                    Margin = new Thickness(0),
                    Padding = new Thickness(0)
                };

                var cell = new TableCell(ui)
                {
                    BorderThickness = new Thickness(0),
                    Padding = new Thickness(0)
                };

                spacer.Cells.Add(cell);
            }

            sectionSumTableRowGroup.Rows.Add(spacer);

            var rowForDoubleUnderlineSecondLine = new TableRow();
            for (int i=0;i<9;i++)
            {
                TableCell spacerCell;
                if (i==0)
                {
                    spacerCell = new TableCell(new Paragraph(new Run("\u00A0"))); // linebreak as a placeholder as a cell must not be totally empty for WPF to work
                }
                else
                {
                    spacerCell = new TableCell(new Paragraph(new Run("\u00A0"))) // linebreak as a placeholder as a cell must not be totally empty for WPF to work
                    {
                        BorderThickness = new Thickness(0, 1, 0, 0),
                        BorderBrush = System.Windows.Media.Brushes.Black,
                        LineHeight = 2
                    };
                }
                
                rowForDoubleUnderlineSecondLine.Cells.Add(spacerCell);
            }
            sectionSumTableRowGroup.Rows.Add(rowForDoubleUnderlineSecondLine);

            flowDoc.Blocks.Add(sectionSumTable);

            return flowDoc;
        }

        public FlowDocument[] generatePageEndSumFlowDocuments(int sectionStartDataLineIndexNumber, int pageEndDataLineIndexNumber)
        {
            var sectionStartDataLineIndex = new Index(sectionStartDataLineIndexNumber);
            var pageEndDataLineIndex = new Index(pageEndDataLineIndexNumber);

            var flowDocPage0 = new FlowDocument();
            flowDocPage0.FontFamily = new System.Windows.Media.FontFamily("Courier New");
            flowDocPage0.FontSize = 12;
            flowDocPage0.ColumnWidth = double.PositiveInfinity;
            flowDocPage0.PageWidth = A3PaperAbstraction.mm_to_diu(A3PaperAbstraction.widthLandscape_mm);
            flowDocPage0.PageHeight = A3PaperAbstraction.mm_to_diu(A3PaperAbstraction.heightLandscape_mm);
            flowDocPage0.PagePadding = new Thickness(0);

            flowDocPage0.Tag = this;

            var sectionSumTablePage0 = new Table();
            sectionSumTablePage0.CellSpacing = 0;
            /*0*/sectionSumTablePage0.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.spacer});
            /*1*/sectionSumTablePage0.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.priceAtPurchase});
            /*2*/sectionSumTablePage0.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.plusMinus});
            /*3*/sectionSumTablePage0.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.enterAndLeaveAmount});
            /*4*/sectionSumTablePage0.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.accumulatedDeprecationAmount});
            /*5*/sectionSumTablePage0.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.spacer2});
            /*6*/sectionSumTablePage0.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.currentYearDeprecationAmount});
            /*7*/sectionSumTablePage0.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.currentYearObjectValue});
            /*8*/sectionSumTablePage0.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.previousYearObjectValue});

            var sectionSumPage0TableRowGroup = new TableRowGroup();
            sectionSumTablePage0.RowGroups.Add(sectionSumPage0TableRowGroup);

            var sectionSumValues = new PrintedDocumentAbstraction.sumValues();
            foreach (var line in this.DataEntryLines.Take(new Range(sectionStartDataLineIndex, pageEndDataLineIndex)))
            {
                    sectionSumValues.priceAtPurchase_cent += line.PriceAtPurchase_Cents;
                    sectionSumValues.enterAmount_cent += line.EnterOrLeaveAmount_Cents is not null && line.EnterOrLeaveAmount_Cents > 0 ? (long)line.EnterOrLeaveAmount_Cents : 0;
                    sectionSumValues.leaveAmount_cent += line.EnterOrLeaveAmount_Cents is not null && line.EnterOrLeaveAmount_Cents < 0 ? (long)-line.EnterOrLeaveAmount_Cents : 0;
                    sectionSumValues.accumulatedDeprecation_cent += line.AccumulatedDepreciation_Cents;
                    sectionSumValues.currentYearDeprecation_cent += line.CurrentYearDepreciationAmount_Cents;
                    sectionSumValues.currentYearObjectValue_cent += line.CurrentYearObjectValue_Cents;
                    sectionSumValues.previousYearObjectValue_cent += line.PreviousYearObjectValue_Cents;
            }

            var sectionSumTableThicknessFirstLine = new Thickness(
                                                                    0,
                                                                    1,
                                                                    0,
                                                                    sectionSumValues.leaveAmount_cent != 0 ? 0 : 1 // if there is a leave amount there is a seperate line being added to the sum table thus the bottom line will be the next line thus this line will not need a bottom border in that case only
                                                               );

            var row = new TableRow();
            sectionSumPage0TableRowGroup.Rows.Add(row);
            _addCellToRow(row, "Ü b e r t r a g");

            var centsConverter = new CentsToEuroStringConverter();
            _addCellToRow(row, _numericToFormatedString(sectionSumValues.priceAtPurchase_cent, centsConverter), borderThickness: sectionSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);
            _addCellToRow(row, sectionSumValues.enterAmount_cent != 0 ? "+" : "\u00A0", borderThickness: sectionSumTableThicknessFirstLine, textAlignment: TextAlignment.Center);
            _addCellToRow(row, sectionSumValues.enterAmount_cent != 0 ? _numericToFormatedString(sectionSumValues.enterAmount_cent, centsConverter) : "\u00A0", borderThickness: sectionSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);
            _addCellToRow(row, _numericToFormatedString(sectionSumValues.accumulatedDeprecation_cent, centsConverter), borderThickness: sectionSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);
            _addCellToRow(row, "\u00A0", borderThickness: sectionSumTableThicknessFirstLine);
            _addCellToRow(row, _numericToFormatedString(sectionSumValues.currentYearDeprecation_cent, centsConverter), borderThickness: sectionSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);
            _addCellToRow(row, _numericToFormatedString(sectionSumValues.currentYearObjectValue_cent, centsConverter), borderThickness: sectionSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);
            _addCellToRow(row, _numericToFormatedString(sectionSumValues.previousYearObjectValue_cent, centsConverter), borderThickness: sectionSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);

            if(sectionSumValues.leaveAmount_cent != 0)
            {
                var sectionSumTableThicknessLeaveLine = new Thickness(0,0,0,1);

                row = new TableRow();
                sectionSumPage0TableRowGroup.Rows.Add(row);
                _addCellToRow(row, "\u00A0");
                _addCellToRow(row, "\u00A0", borderThickness: sectionSumTableThicknessLeaveLine);
                _addCellToRow(row, sectionSumValues.leaveAmount_cent != 0 ? "-" : "\u00A0", borderThickness: sectionSumTableThicknessLeaveLine, textAlignment: TextAlignment.Center);
                _addCellToRow(row, sectionSumValues.leaveAmount_cent != 0 ? _numericToFormatedString(sectionSumValues.leaveAmount_cent, centsConverter) : "\u00A0", borderThickness: sectionSumTableThicknessLeaveLine, textAlignment: TextAlignment.Right);
                for(int i=0;i<5;i++)
                {
                    _addCellToRow(row, "\u00A0", borderThickness: sectionSumTableThicknessLeaveLine);
                }
            }

            var spacer = new TableRow();
            
            for (int i=0;i<9;i++)
            {
                var rect = new System.Windows.Shapes.Rectangle
                {
                    Height = 2, // tiny gap
                    Fill = System.Windows.Media.Brushes.Transparent
                };

                var ui = new BlockUIContainer(rect)
                {
                    Margin = new Thickness(0),
                    Padding = new Thickness(0)
                };

                var cell = new TableCell(ui)
                {
                    BorderThickness = new Thickness(0),
                    Padding = new Thickness(0)
                };

                spacer.Cells.Add(cell);
            }

            sectionSumPage0TableRowGroup.Rows.Add(spacer);

            var rowForDoubleUnderlineSecondLine = new TableRow();
            for (int i=0;i<9;i++)
            {
                TableCell spacerCell;
                if (i==0)
                {
                    spacerCell = new TableCell(new Paragraph(new Run("\u00A0"))); // linebreak as a placeholder as a cell must not be totally empty for WPF to work
                }
                else
                {
                    spacerCell = new TableCell(new Paragraph(new Run("\u00A0"))) // linebreak as a placeholder as a cell must not be totally empty for WPF to work
                    {
                        BorderThickness = new Thickness(0, 1, 0, 0),
                        BorderBrush = System.Windows.Media.Brushes.Black,
                        LineHeight = 2
                    };
                }
                
                rowForDoubleUnderlineSecondLine.Cells.Add(spacerCell);
            }
            sectionSumPage0TableRowGroup.Rows.Add(rowForDoubleUnderlineSecondLine);
            flowDocPage0.Blocks.Add(sectionSumTablePage0);


            sectionSumTableThicknessFirstLine = new Thickness(
                                                        0,
                                                        0,
                                                        0,
                                                        sectionSumValues.leaveAmount_cent != 0 ? 0 : 1 // if there is a leave amount there is a seperate line being added to the sum table thus the bottom line will be the next line thus this line will not need a bottom border in that case only
                                                   );

            // the below is for the echo of the previous page values on the top of the next page
            var flowDocPage1 = new FlowDocument();
            flowDocPage1.FontFamily = new System.Windows.Media.FontFamily("Courier New");
            flowDocPage1.FontSize = 12;
            flowDocPage1.ColumnWidth = double.PositiveInfinity;
            flowDocPage1.PageWidth = A3PaperAbstraction.mm_to_diu(A3PaperAbstraction.widthLandscape_mm);
            flowDocPage1.PageHeight = A3PaperAbstraction.mm_to_diu(A3PaperAbstraction.heightLandscape_mm);
            flowDocPage1.PagePadding = new Thickness(0);

            flowDocPage1.Tag = this;

            var sectionSumTablePage1 = new Table();
            sectionSumTablePage1.CellSpacing = 0;
            /*0*/sectionSumTablePage1.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.spacer});
            /*1*/sectionSumTablePage1.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.priceAtPurchase});
            /*2*/sectionSumTablePage1.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.plusMinus});
            /*3*/sectionSumTablePage1.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.enterAndLeaveAmount});
            /*4*/sectionSumTablePage1.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.accumulatedDeprecationAmount});
            /*5*/sectionSumTablePage1.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.spacer2});
            /*6*/sectionSumTablePage1.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.currentYearDeprecationAmount});
            /*7*/sectionSumTablePage1.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.currentYearObjectValue});
            /*8*/sectionSumTablePage1.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.previousYearObjectValue});

            var sectionSumPage1TableRowGroup = new TableRowGroup();
            sectionSumTablePage1.RowGroups.Add(sectionSumPage1TableRowGroup);

            row = new TableRow();
            sectionSumPage1TableRowGroup.Rows.Add(row);

            _addCellToRow(row, "Ü b e r t r a g");
            _addCellToRow(row, _numericToFormatedString(sectionSumValues.priceAtPurchase_cent, centsConverter), borderThickness: sectionSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);
            _addCellToRow(row, sectionSumValues.enterAmount_cent != 0 ? "+" : "\u00A0", borderThickness: sectionSumTableThicknessFirstLine, textAlignment: TextAlignment.Center);
            _addCellToRow(row, sectionSumValues.enterAmount_cent != 0 ? _numericToFormatedString(sectionSumValues.enterAmount_cent, centsConverter) : "\u00A0", borderThickness: sectionSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);
            _addCellToRow(row, _numericToFormatedString(sectionSumValues.accumulatedDeprecation_cent, centsConverter), borderThickness: sectionSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);
            _addCellToRow(row, "\u00A0", borderThickness: sectionSumTableThicknessFirstLine);
            _addCellToRow(row, _numericToFormatedString(sectionSumValues.currentYearDeprecation_cent, centsConverter), borderThickness: sectionSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);
            _addCellToRow(row, _numericToFormatedString(sectionSumValues.currentYearObjectValue_cent, centsConverter), borderThickness: sectionSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);
            _addCellToRow(row, _numericToFormatedString(sectionSumValues.previousYearObjectValue_cent, centsConverter), borderThickness: sectionSumTableThicknessFirstLine, textAlignment: TextAlignment.Right);

            if (sectionSumValues.leaveAmount_cent != 0)
            {
                var sectionSumTableThicknessLeaveLine = new Thickness(0, 0, 0, 1);

                row = new TableRow();
                sectionSumPage1TableRowGroup.Rows.Add(row);
                _addCellToRow(row, "\u00A0");
                _addCellToRow(row, "\u00A0", borderThickness: sectionSumTableThicknessLeaveLine);
                _addCellToRow(row, sectionSumValues.leaveAmount_cent != 0 ? "-" : "\u00A0", borderThickness: sectionSumTableThicknessLeaveLine, textAlignment: TextAlignment.Center);
                _addCellToRow(row, sectionSumValues.leaveAmount_cent != 0 ? _numericToFormatedString(sectionSumValues.leaveAmount_cent, centsConverter) : "\u00A0", borderThickness: sectionSumTableThicknessLeaveLine, textAlignment: TextAlignment.Right);
                for (int i = 0; i < 5; i++)
                {
                    _addCellToRow(row, "\u00A0", borderThickness: sectionSumTableThicknessLeaveLine);
                }
            }

            flowDocPage1.Blocks.Add(sectionSumTablePage1);


            return new[] {flowDocPage0, flowDocPage1};
        }

    }
}
