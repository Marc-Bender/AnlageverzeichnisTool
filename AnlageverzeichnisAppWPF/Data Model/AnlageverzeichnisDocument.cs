using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reflection.Metadata;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace AnlageverzeichnisAppWPF
{
    public class PrintedDocumentAbstraction
    {
        public class headerTableColumnWidths
        {
            public static GridLength objectDescription => new GridLength(2, GridUnitType.Star);
            public static GridLength dateOfPurchase => new GridLength(0.5, GridUnitType.Star);
            public static GridLength priceAtPurchase => new GridLength(0.5, GridUnitType.Star);
            public static GridLength plusMinus => new GridLength(0.25, GridUnitType.Star);
            public static GridLength enterAndLeaveAmount => new GridLength(0.75, GridUnitType.Star);
            public static GridLength accumulatedDeprecationAmount => new GridLength(1, GridUnitType.Star);
            public static GridLength currentYearDeprecationPercentage => new GridLength(0.5, GridUnitType.Star);
            public static GridLength currentYearDeprecationAmount => new GridLength(0.5, GridUnitType.Star);
            public static GridLength currentYearObjectValue => new GridLength(0.75, GridUnitType.Star);
            public static GridLength previousYearObjectValue => new GridLength(0.75, GridUnitType.Star);
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

        static FlowDocument MergeDocuments(List<FlowDocument> docs)
        {
            var merged = new FlowDocument();

            foreach (var doc in docs)
            {
                // Make a copy of the blocks before iterating to prevent the doc.Blocks collection to change implicitly thereby throwing exceptions..
                var blocks = doc.Blocks.ToList();
                foreach (var block in blocks)
                {
                    merged.Blocks.Add(block);
                }
            }

            return merged;
        }


        public FlowDocument generateHeaderFlowDocument()
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

        public FlowDocument generateTableFlowDocument(int sectionStartDataLineIndexNumber, int sectionEndDataLineIndexNumber)
        {
            return new FlowDocument();
        }
        public FlowDocument generateTotalsSumFlowDocument()
        {
            var flowDoc = new FlowDocument();
            flowDoc.FontFamily = new System.Windows.Media.FontFamily("Courier New");
            flowDoc.FontSize = 12;
            flowDoc.ColumnWidth = double.PositiveInfinity;
            flowDoc.PageWidth = A3PaperAbstraction.widthLandscape_mm / A3PaperAbstraction.mm_per_inch * A3PaperAbstraction.DPI;
            flowDoc.PageHeight = A3PaperAbstraction.heightLandscape_mm / A3PaperAbstraction.mm_per_inch * A3PaperAbstraction.DPI;

            flowDoc.Tag = this;

            var totalsSumTable = new Table();
            totalsSumTable.CellSpacing = 0;
            /*0*/
            totalsSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.spacer });
            /*1*/
            totalsSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.priceAtPurchase });
            /*2*/
            totalsSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.plusMinus });
            /*3*/
            totalsSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.enterAndLeaveAmount });
            /*4*/
            totalsSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.accumulatedDeprecationAmount });
            /*5*/
            totalsSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.spacer2 });
            /*6*/
            totalsSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.currentYearDeprecationAmount });
            /*7*/
            totalsSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.currentYearObjectValue });
            /*8*/
            totalsSumTable.Columns.Add(new TableColumn { Width = PrintedDocumentAbstraction.sectionSumTableColumnWidths.previousYearObjectValue });

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
            row.Cells.Add(new TableCell(new Paragraph(new Run("G e s a m t s u m m e n")))
            {
                BorderThickness = totalsSumTableThicknessFirstLine,
                BorderBrush = System.Windows.Media.Brushes.Black
            }
                         );
            var centsConverter = new CentsToEuroStringConverter();
            row.Cells.Add(new TableCell(new Paragraph(new Run((string)centsConverter.Convert(totalsSumTableValues.priceAtPurchase_cent, Type.GetType("string"), new object(), new System.Globalization.CultureInfo("de-DE")))))
            {
                BorderThickness = totalsSumTableThicknessFirstLine,
                BorderBrush = System.Windows.Media.Brushes.Black
            }
                         );
            row.Cells.Add(new TableCell(new Paragraph(new Run(totalsSumTableValues.enterAmount_cent != 0 ? "+" : "")))
            {
                BorderThickness = totalsSumTableThicknessFirstLine,
                BorderBrush = System.Windows.Media.Brushes.Black
            }
                         );
            row.Cells.Add(new TableCell(new Paragraph(new Run(
                                                                totalsSumTableValues.enterAmount_cent != 0 ?
                                                                      (string)centsConverter.Convert(totalsSumTableValues.enterAmount_cent, Type.GetType("string"), new object(), new System.Globalization.CultureInfo("de-DE"))
                                                                    : ""
                                                             )))
            {
                BorderThickness = totalsSumTableThicknessFirstLine,
                BorderBrush = System.Windows.Media.Brushes.Black
            }
                         );

            row.Cells.Add(new TableCell(new Paragraph(new Run((string)centsConverter.Convert(totalsSumTableValues.accumulatedDeprecation_cent, Type.GetType("string"), new object(), new System.Globalization.CultureInfo("de-DE")))))
            {
                BorderThickness = totalsSumTableThicknessFirstLine,
                BorderBrush = System.Windows.Media.Brushes.Black
            }
                         );

            row.Cells.Add(new TableCell(new Paragraph(new Run(" ")))
            {
                BorderThickness = totalsSumTableThicknessFirstLine,
                BorderBrush = System.Windows.Media.Brushes.Black
            }
                         );

            row.Cells.Add(new TableCell(new Paragraph(new Run((string)centsConverter.Convert(totalsSumTableValues.currentYearDeprecation_cent, Type.GetType("string"), new object(), new System.Globalization.CultureInfo("de-DE")))))
            {
                BorderThickness = totalsSumTableThicknessFirstLine,
                BorderBrush = System.Windows.Media.Brushes.Black
            }
                         );

            row.Cells.Add(new TableCell(new Paragraph(new Run((string)centsConverter.Convert(totalsSumTableValues.currentYearObjectValue_cent, Type.GetType("string"), new object(), new System.Globalization.CultureInfo("de-DE")))))
            {
                BorderThickness = totalsSumTableThicknessFirstLine,
                BorderBrush = System.Windows.Media.Brushes.Black
            }
                         );

            row.Cells.Add(new TableCell(new Paragraph(new Run((string)centsConverter.Convert(totalsSumTableValues.previousYearObjectValue_cent, Type.GetType("string"), new object(), new System.Globalization.CultureInfo("de-DE")))))
            {
                BorderThickness = totalsSumTableThicknessFirstLine,
                BorderBrush = System.Windows.Media.Brushes.Black
            }
                         );

            if (totalsSumTableValues.leaveAmount_cent != 0)
            {
                var sectionSumTableThicknessLeaveLine = new Thickness(0, 0, 0, 1);

                row = new TableRow();
                totalsSumTableRowGroup.Rows.Add(row);
                row.Cells.Add(new TableCell(new Paragraph(new Run(" ")))
                {
                    BorderThickness = sectionSumTableThicknessLeaveLine,
                    BorderBrush = System.Windows.Media.Brushes.Black
                }
                             );
                row.Cells.Add(new TableCell(new Paragraph(new Run(" ")))
                {
                    BorderThickness = sectionSumTableThicknessLeaveLine,
                    BorderBrush = System.Windows.Media.Brushes.Black
                }
                             );
                row.Cells.Add(new TableCell(new Paragraph(new Run(totalsSumTableValues.leaveAmount_cent != 0 ? "-" : "")))
                {
                    BorderThickness = sectionSumTableThicknessLeaveLine,
                    BorderBrush = System.Windows.Media.Brushes.Black
                }
                             );
                row.Cells.Add(new TableCell(new Paragraph(new Run(
                                                                    totalsSumTableValues.leaveAmount_cent != 0 ?
                                                                          (string)centsConverter.Convert(totalsSumTableValues.leaveAmount_cent, Type.GetType("string"), new object(), new System.Globalization.CultureInfo("de-DE"))
                                                                        : ""
                                                                 )))
                {
                    BorderThickness = sectionSumTableThicknessLeaveLine,
                    BorderBrush = System.Windows.Media.Brushes.Black
                }
                             );

                row.Cells.Add(new TableCell(new Paragraph(new Run(" ")))
                {
                    BorderThickness = sectionSumTableThicknessLeaveLine,
                    BorderBrush = System.Windows.Media.Brushes.Black
                }
                             );

                row.Cells.Add(new TableCell(new Paragraph(new Run(" ")))
                {
                    BorderThickness = sectionSumTableThicknessLeaveLine,
                    BorderBrush = System.Windows.Media.Brushes.Black
                }
                             );

                row.Cells.Add(new TableCell(new Paragraph(new Run(" ")))
                {
                    BorderThickness = sectionSumTableThicknessLeaveLine,
                    BorderBrush = System.Windows.Media.Brushes.Black
                }
                             );

                row.Cells.Add(new TableCell(new Paragraph(new Run(" ")))
                {
                    BorderThickness = sectionSumTableThicknessLeaveLine,
                    BorderBrush = System.Windows.Media.Brushes.Black
                }
                             );

                row.Cells.Add(new TableCell(new Paragraph(new Run(" ")))
                {
                    BorderThickness = sectionSumTableThicknessLeaveLine,
                    BorderBrush = System.Windows.Media.Brushes.Black
                }
                             );
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

        public FlowDocument toFlowDocument()
        {
            var headerFlowDoc = this.generateHeaderFlowDocument();

            var headingsDataLines = this.DataEntryLines.Where(x => x.IsHeading == true).ToList();

            var totalsSumFlowDoc = this.generateTotalsSumFlowDocument();

            List<FlowDocument> allDocumentFragments = new();
            allDocumentFragments.Add(headerFlowDoc);

            for (int i=0;i<headingsDataLines.Count();i++)
            {
                FlowDocument? tableFlowDoc = null;
                FlowDocument? sectionSumFlowDoc = null;
                if(i<headingsDataLines.Count() - 1)
                {
                    // if the current heading is not the last heading the section goes until the next heading
                    var startIndex = this.DataEntryLines.IndexOf(headingsDataLines[i]);
                    var endIndex = this.DataEntryLines.IndexOf(headingsDataLines[i + 1]);

                    tableFlowDoc = this.generateTableFlowDocument(startIndex, endIndex);
                    sectionSumFlowDoc = this.generateSectionSumFlowDocument(startIndex,endIndex);

                }
                else
                {
                    // if the current heading _IS_ the last heading then the section goes until the end of the document.
                    var startIndex = this.DataEntryLines.IndexOf(headingsDataLines[i]);
                    var endIndex = this.DataEntryLines.Count();

                    tableFlowDoc = this.generateTableFlowDocument(startIndex, endIndex);
                    sectionSumFlowDoc = this.generateSectionSumFlowDocument(startIndex,endIndex);
                }

                allDocumentFragments.Add(tableFlowDoc); // guaranteed by design that tableFlowDoc is never null
                allDocumentFragments.Add(sectionSumFlowDoc); // guaranteed by design that sectionSumFlowDoc is never null
            }

            allDocumentFragments.Add(totalsSumFlowDoc);
            var mergedDocument = MergeDocuments(allDocumentFragments);
            mergedDocument.FontFamily = new System.Windows.Media.FontFamily("Courier New");
            mergedDocument.FontSize = 12;
            mergedDocument.ColumnWidth = double.PositiveInfinity;
            mergedDocument.PageWidth = A3PaperAbstraction.widthLandscape_mm / A3PaperAbstraction.mm_per_inch * A3PaperAbstraction.DPI;
            mergedDocument.PageHeight = A3PaperAbstraction.heightLandscape_mm / A3PaperAbstraction.mm_per_inch * A3PaperAbstraction.DPI;

            mergedDocument.Tag = this;

            return mergedDocument;
        }
        

        public FlowDocument generateSectionSumFlowDocument(int sectionStartDataLineIndexNumber, int sectionEndDataLineIndexNumber)
        {
            var sectionStartDataLineIndex = new Index(sectionStartDataLineIndexNumber);
            var sectionEndDataLineIndex = new Index(sectionEndDataLineIndexNumber);

            var flowDoc = new FlowDocument();
            flowDoc.FontFamily = new System.Windows.Media.FontFamily("Courier New");
            flowDoc.FontSize = 12;
            flowDoc.ColumnWidth = double.PositiveInfinity;
            flowDoc.PageWidth = A3PaperAbstraction.widthLandscape_mm / A3PaperAbstraction.mm_per_inch * A3PaperAbstraction.DPI;
            flowDoc.PageHeight = A3PaperAbstraction.heightLandscape_mm / A3PaperAbstraction.mm_per_inch * A3PaperAbstraction.DPI;

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
            row.Cells.Add(new TableCell(new Paragraph(new Run(" "))) 
                            {
                                BorderThickness = sectionSumTableThicknessFirstLine,
                                BorderBrush = System.Windows.Media.Brushes.Black
                            }
                         );
            var centsConverter = new CentsToEuroStringConverter();
            row.Cells.Add(new TableCell(new Paragraph(new Run((string)centsConverter.Convert(sectionSumValues.priceAtPurchase_cent, Type.GetType("string"), new object(), new System.Globalization.CultureInfo("de-DE")))))
                            {
                                BorderThickness = sectionSumTableThicknessFirstLine,
                                BorderBrush = System.Windows.Media.Brushes.Black
                            }
                         );
            row.Cells.Add(new TableCell(new Paragraph(new Run(sectionSumValues.enterAmount_cent != 0 ? "+" : "")))
                            {
                                BorderThickness = sectionSumTableThicknessFirstLine,
                                BorderBrush = System.Windows.Media.Brushes.Black
                            }
                         );
            row.Cells.Add(new TableCell(new Paragraph(new Run(
                                                                sectionSumValues.enterAmount_cent != 0 ?
                                                                      (string)centsConverter.Convert(sectionSumValues.enterAmount_cent, Type.GetType("string"), new object(), new System.Globalization.CultureInfo("de-DE"))
                                                                    : ""
                                                             )))
                            {
                                BorderThickness = sectionSumTableThicknessFirstLine,
                                BorderBrush = System.Windows.Media.Brushes.Black
                            }
                         );

            row.Cells.Add(new TableCell(new Paragraph(new Run((string)centsConverter.Convert(sectionSumValues.accumulatedDeprecation_cent, Type.GetType("string"), new object(), new System.Globalization.CultureInfo("de-DE")))))
                            {
                                BorderThickness = sectionSumTableThicknessFirstLine,
                                BorderBrush = System.Windows.Media.Brushes.Black
                            }
                         );

            row.Cells.Add(new TableCell(new Paragraph(new Run(" ")))
                            {
                                BorderThickness = sectionSumTableThicknessFirstLine,
                                BorderBrush = System.Windows.Media.Brushes.Black
                            }
                         );

            row.Cells.Add(new TableCell(new Paragraph(new Run((string)centsConverter.Convert(sectionSumValues.currentYearDeprecation_cent, Type.GetType("string"), new object(), new System.Globalization.CultureInfo("de-DE")))))
                            {
                                BorderThickness = sectionSumTableThicknessFirstLine,
                                BorderBrush = System.Windows.Media.Brushes.Black
                            }
                         );

            row.Cells.Add(new TableCell(new Paragraph(new Run((string)centsConverter.Convert(sectionSumValues.currentYearObjectValue_cent, Type.GetType("string"), new object(), new System.Globalization.CultureInfo("de-DE")))))
                            {
                                BorderThickness = sectionSumTableThicknessFirstLine,
                                BorderBrush = System.Windows.Media.Brushes.Black
                            }
                         );

            row.Cells.Add(new TableCell(new Paragraph(new Run((string)centsConverter.Convert(sectionSumValues.previousYearObjectValue_cent, Type.GetType("string"), new object(), new System.Globalization.CultureInfo("de-DE")))))
                            {
                                BorderThickness = sectionSumTableThicknessFirstLine,
                                BorderBrush = System.Windows.Media.Brushes.Black
                            }
                         );

            if(sectionSumValues.leaveAmount_cent != 0)
            {
                var sectionSumTableThicknessLeaveLine = new Thickness(0,0,0,1);

                row = new TableRow();
                sectionSumTableRowGroup.Rows.Add(row);
                row.Cells.Add(new TableCell(new Paragraph(new Run(" "))) 
                                {
                                    BorderThickness = sectionSumTableThicknessLeaveLine,
                                    BorderBrush = System.Windows.Media.Brushes.Black
                                }
                             );
                row.Cells.Add(new TableCell(new Paragraph(new Run(" ")))
                                {
                                    BorderThickness = sectionSumTableThicknessLeaveLine,
                                    BorderBrush = System.Windows.Media.Brushes.Black
                                }
                             );
                row.Cells.Add(new TableCell(new Paragraph(new Run(sectionSumValues.leaveAmount_cent != 0 ? "-" : "")))
                                {
                                    BorderThickness = sectionSumTableThicknessLeaveLine,
                                    BorderBrush = System.Windows.Media.Brushes.Black
                                }
                             );
                row.Cells.Add(new TableCell(new Paragraph(new Run(
                                                                    sectionSumValues.leaveAmount_cent != 0 ?
                                                                          (string)centsConverter.Convert(sectionSumValues.leaveAmount_cent, Type.GetType("string"), new object(), new System.Globalization.CultureInfo("de-DE"))
                                                                        : ""
                                                                 )))
                                {
                                    BorderThickness = sectionSumTableThicknessLeaveLine,
                                    BorderBrush = System.Windows.Media.Brushes.Black
                                }
                             );

                row.Cells.Add(new TableCell(new Paragraph(new Run(" ")))
                                {
                                    BorderThickness = sectionSumTableThicknessLeaveLine,
                                    BorderBrush = System.Windows.Media.Brushes.Black
                                }
                             );

                row.Cells.Add(new TableCell(new Paragraph(new Run(" ")))
                                {
                                    BorderThickness = sectionSumTableThicknessLeaveLine,
                                    BorderBrush = System.Windows.Media.Brushes.Black
                                }
                             );

                row.Cells.Add(new TableCell(new Paragraph(new Run(" ")))
                                {
                                    BorderThickness = sectionSumTableThicknessLeaveLine,
                                    BorderBrush = System.Windows.Media.Brushes.Black
                                }
                             );

                row.Cells.Add(new TableCell(new Paragraph(new Run(" ")))
                                {
                                    BorderThickness = sectionSumTableThicknessLeaveLine,
                                    BorderBrush = System.Windows.Media.Brushes.Black
                                }
                             );

                row.Cells.Add(new TableCell(new Paragraph(new Run(" ")))
                                {
                                    BorderThickness = sectionSumTableThicknessLeaveLine,
                                    BorderBrush = System.Windows.Media.Brushes.Black
                                }
                             );
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
                var spacerCell = new TableCell(new Paragraph(new Run("\u00A0"))) // linebreak as a placeholder as a cell must not be totally empty for WPF to work
                {
                    BorderThickness = new Thickness(0, 1, 0, 0),
                    BorderBrush = System.Windows.Media.Brushes.Black,
                    LineHeight = 2
                };
                rowForDoubleUnderlineSecondLine.Cells.Add(spacerCell);
            }
            sectionSumTableRowGroup.Rows.Add(rowForDoubleUnderlineSecondLine);


            flowDoc.Blocks.Add(sectionSumTable);



            return flowDoc;
        }

    }
}
