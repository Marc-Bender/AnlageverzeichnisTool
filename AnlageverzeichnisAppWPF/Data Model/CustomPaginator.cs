using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

namespace AnlageverzeichnisAppWPF
{
    public class A3PaperAbstraction
    {
        public static readonly int heightLandscape_mm = 297;
        public static readonly int widthLandscape_mm = 420;
        public static readonly int DPI = 96;
        public static readonly float mm_per_inch = 25.4f;
        public static readonly int pageMargins_mm = 10;
        public static double mm_to_diu(double mm) => mm / mm_per_inch * DPI;
    }

    public class CustomPaginator : DocumentPaginator
    {
        private readonly Typeface _headerTypeface = new Typeface("Courier New");
        private readonly Thickness _pageMargins = new Thickness(A3PaperAbstraction.pageMargins_mm / A3PaperAbstraction.mm_per_inch * A3PaperAbstraction.DPI);

        private AnlageverzeichnisDocument? document;
        private List<FlowDocument> dataLinesFlowDocuments;

        // calculate the header and totals flowdoc once and then reuse across pages
        private FlowDocument headerFlowDocument, totalsFlowDocument;

        // generate a dummy set of flow docs for the page end sums for calculating their size upfront
        private FlowDocument[] dummyPageEndSumFlowDocuments;

        private double headerSize, totalsSize, pageEndSumPage0Size, pageEndSumPage1Size;
        private DocumentPage headerPage, totalsPage, pageEndSumPage0, pageEndSumPage1;
        
        private bool isSectionInterrupted = false;
        private int currentLineIndex = 0;

        private List<DocumentPage> Pages = new();
        private Size _pageSize = new Size(
                                            A3PaperAbstraction.mm_to_diu(A3PaperAbstraction.widthLandscape_mm), 
                                            A3PaperAbstraction.mm_to_diu(A3PaperAbstraction.heightLandscape_mm)
                                         );

        public static FlowDocument CloneFlowDocument(FlowDocument source)
        {
            var tag = source.Tag;
            source.Tag = null;
            string xaml = XamlWriter.Save(source);
            using var stringReader = new StringReader(xaml);
            using var xmlReader = XmlReader.Create(stringReader);
            source.Tag = tag;
            return (FlowDocument)XamlReader.Load(xmlReader);
        }
        private static double GetDpi()
        {
            var source = PresentationSource.FromVisual(Application.Current.MainWindow);
            return source?.CompositionTarget?.TransformToDevice.M22 * 96 ?? 96;
        }

        public static double MeasureFlowDocumentHeight(FlowDocument doc)
        {
            var viewer = new RichTextBox
            {
                Document = CloneFlowDocument(doc),
                Width = doc.PageWidth, // requires the pagewidth to be set which for the flow docs in this tool is true
                BorderThickness = new Thickness(0),
                Padding = new Thickness(0),
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                LayoutTransform = new ScaleTransform(96.0 / GetDpi(), 96.0 / GetDpi())
            };

            viewer.Measure(new Size(doc.PageWidth, double.PositiveInfinity));
            viewer.Arrange(new Rect(0, 0, doc.PageWidth, viewer.DesiredSize.Height));
            viewer.UpdateLayout();

            return viewer.ExtentHeight;
        }
        public CustomPaginator(AnlageverzeichnisDocument document)
        {
            this.document = document;
            dataLinesFlowDocuments = document.generatePerTableLineFlowDocuments();
            foreach(var linedoc in dataLinesFlowDocuments)
            {
                linedoc.PageWidth = A3PaperAbstraction.mm_to_diu(A3PaperAbstraction.widthLandscape_mm) - _pageMargins.Left - _pageMargins.Right;
            }

            headerFlowDocument = document.generateHeaderFlowDocument();
            headerFlowDocument.PageWidth = A3PaperAbstraction.mm_to_diu(A3PaperAbstraction.widthLandscape_mm) - _pageMargins.Left - _pageMargins.Right;
            totalsFlowDocument = document.generateTotalsSumFlowDocument();
            totalsFlowDocument.PageWidth = A3PaperAbstraction.mm_to_diu(A3PaperAbstraction.widthLandscape_mm) - _pageMargins.Left - _pageMargins.Right;
            dummyPageEndSumFlowDocuments = document.generatePageEndSumFlowDocuments(0, 0);
            var _basePaginator = ((IDocumentPaginatorSource)headerFlowDocument).DocumentPaginator;
            headerPage = _basePaginator.GetPage(0);
            headerSize = MeasureFlowDocumentHeight(headerFlowDocument);
            _basePaginator = ((IDocumentPaginatorSource)totalsFlowDocument).DocumentPaginator;
            totalsPage = _basePaginator.GetPage(0);
            totalsSize = MeasureFlowDocumentHeight(totalsFlowDocument);
            _basePaginator = ((IDocumentPaginatorSource)dummyPageEndSumFlowDocuments[0]).DocumentPaginator;
            pageEndSumPage0 = _basePaginator.GetPage(0);
            pageEndSumPage0Size = MeasureFlowDocumentHeight(dummyPageEndSumFlowDocuments[0]);
            _basePaginator = ((IDocumentPaginatorSource)dummyPageEndSumFlowDocuments[1]).DocumentPaginator;
            pageEndSumPage1 = _basePaginator.GetPage(0);
            pageEndSumPage1Size = MeasureFlowDocumentHeight(dummyPageEndSumFlowDocuments[1]);

            while (true)
            {
                // Create a new visual to draw header/footer
                var visual = new DrawingVisual();
                using (var dc = visual.RenderOpen())
                {
                    double cumulativeHeight = 0;
                    dc.DrawRectangle(
                        new VisualBrush(headerPage.Visual)
                        {
                            Stretch = Stretch.None,
                            AlignmentX = AlignmentX.Left,
                            AlignmentY = AlignmentY.Top
                        },
                        null,
                        new Rect(
                                    _pageMargins.Left,
                                    _pageMargins.Top,
                                    this._pageSize.Width - _pageMargins.Left - _pageMargins.Right,
                                    headerSize
                                )
                        );

                    cumulativeHeight += headerSize;

                    if(this.isSectionInterrupted == true)
                    {
                        this.isSectionInterrupted = false;
                        dc.DrawRectangle(
                            new VisualBrush(pageEndSumPage1.Visual)
                            {
                                Stretch = Stretch.None,
                                AlignmentX = AlignmentX.Left,
                                AlignmentY = AlignmentY.Top
                            },
                            null,
                            new Rect(
                                        _pageMargins.Left,
                                        _pageMargins.Top + cumulativeHeight,
                                        this._pageSize.Width - _pageMargins.Left - _pageMargins.Right,
                                        pageEndSumPage1Size
                                    )
                            );

                        cumulativeHeight += pageEndSumPage1Size;
                    }
                    double cumulativeHeightMax;

                    cumulativeHeightMax = A3PaperAbstraction.mm_to_diu(A3PaperAbstraction.heightLandscape_mm) - _pageMargins.Top - _pageMargins.Bottom - totalsSize * 1.5f; // factor 1.5 as safety for possibly missing line with leave amounts... all sums should be more or less the same size enough for this simplification to hold

                    if (this.document is null)
                    {
                        MessageBox.Show("Fehler beim erstellen des PDF", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    }

                    for (int i = currentLineIndex; i < dataLinesFlowDocuments.Count; i++)
                    {
                        _basePaginator = ((IDocumentPaginatorSource)dataLinesFlowDocuments.ElementAt(currentLineIndex)).DocumentPaginator;
                        var thisLinePage = _basePaginator.GetPage(0);
                        _basePaginator = currentLineIndex + 1 < dataLinesFlowDocuments.Count ? ((IDocumentPaginatorSource)dataLinesFlowDocuments.ElementAt(currentLineIndex + 1)).DocumentPaginator : null;
                        var nextLinePage = _basePaginator is not null ? _basePaginator.GetPage(0) : null;
                        
                        var thisLinePageSize = MeasureFlowDocumentHeight(dataLinesFlowDocuments[currentLineIndex]);
                        var nextLinePageSize = currentLineIndex + 1 < dataLinesFlowDocuments.Count?MeasureFlowDocumentHeight(dataLinesFlowDocuments[currentLineIndex+1]):0;
                        if (
                                (cumulativeHeight + thisLinePageSize < cumulativeHeightMax)
                                && (this.document is not null)
                                && (
                                    // insert current line only if not a heading or if it is a heading and atleast one data line fits after it... this is to ensure that a page end sum is always meaningfully calculatable
                                    (nextLinePage is null)
                                    || (this.document.DataEntryLines.ElementAt(currentLineIndex).IsHeading == false)
                                    || (
                                        (nextLinePage is not null)
                                        && (this.document.DataEntryLines.ElementAt(currentLineIndex).IsHeading == true)
                                        && (cumulativeHeight + thisLinePageSize + nextLinePageSize < cumulativeHeightMax)
                                    )
                                )
                            )
                        {
                            if (this.document.DataEntryLines.ElementAt(currentLineIndex).IsHeading == true)
                            {
                                try
                                {
                                    // at the end of any section there needs to be a sectionend sum block ... 
                                    // 1st find the previous heading
                                    var headingsUntilNow = this.document.DataEntryLines.Where(x => x.IsHeading == true && this.document.DataEntryLines.IndexOf(x) < currentLineIndex);
                                    // then generate the sectionendsum 
                                    var sectionEndSumDoc = this.document.generateSectionSumFlowDocument(
                                                                                                            sectionStartDataLineIndexNumber: this.document.DataEntryLines.IndexOf(headingsUntilNow.Last()),
                                                                                                            sectionEndDataLineIndexNumber: currentLineIndex + 1 
                                                                                                        );
                                   
                                    // paginate the new section end sum document
                                    _basePaginator = ((IDocumentPaginatorSource)sectionEndSumDoc).DocumentPaginator;
                                    sectionEndSumDoc.PageWidth = A3PaperAbstraction.mm_to_diu(A3PaperAbstraction.widthLandscape_mm) - _pageMargins.Left - _pageMargins.Right;
                                    var sectionSumPage = _basePaginator.GetPage(0);
                                    var sectionSumPageSize = MeasureFlowDocumentHeight(sectionEndSumDoc);
                                    
                                    // draw the section end sum

                                    dc.DrawRectangle(
                                        new VisualBrush(sectionSumPage.Visual)
                                        {
                                            Stretch = Stretch.None,
                                            AlignmentX = AlignmentX.Left,
                                            AlignmentY = AlignmentY.Top
                                        },
                                        null,
                                        new Rect(
                                                    _pageMargins.Left,
                                                    _pageMargins.Top + cumulativeHeight,
                                                    this._pageSize.Width - _pageMargins.Left - _pageMargins.Right,
                                                    sectionSumPageSize
                                                )
                                        );
                                    cumulativeHeight += sectionSumPageSize;
                                }
                                catch (InvalidOperationException)
                                {
                                    // may occur if the sequence "headingsUntilNow" does not contain any elements which is fine in case the first line is a heading ... (then there is no last item in there and then there also does not need to be any section end sum in place)
                                    // thus in this case there is nothing to do here
                                }
                            }

                            dc.DrawRectangle(
                                new VisualBrush(thisLinePage.Visual)
                                {
                                    Stretch = Stretch.None,
                                    AlignmentX = AlignmentX.Left,
                                    AlignmentY = AlignmentY.Top,
                                },
                                null,
                                new Rect(
                                            _pageMargins.Left,
                                            _pageMargins.Top + cumulativeHeight,
                                            this._pageSize.Width - _pageMargins.Left - _pageMargins.Right,
                                            thisLinePageSize
                                        )
                                );
                            cumulativeHeight += thisLinePageSize;
                            currentLineIndex++;
                        }
                        else
                        {
                            var headingsUntilNow = this.document.DataEntryLines.Where(x => x.IsHeading == true && this.document.DataEntryLines.IndexOf(x) < currentLineIndex);

                            // then generate the sectionendsum 
                            var pageEndSumFlowDocs = this.document.generatePageEndSumFlowDocuments(
                                                                                                    sectionStartDataLineIndexNumber: this.document.DataEntryLines.IndexOf(headingsUntilNow.Last()),
                                                                                                    pageEndDataLineIndexNumber: currentLineIndex
                                                                                                  );
                            _basePaginator = ((IDocumentPaginatorSource)pageEndSumFlowDocs[0]).DocumentPaginator;
                            pageEndSumFlowDocs[0].PageWidth = A3PaperAbstraction.mm_to_diu(A3PaperAbstraction.widthLandscape_mm) - _pageMargins.Left - _pageMargins.Right;
                            pageEndSumPage0 = _basePaginator.GetPage(0);
                            pageEndSumPage0Size = MeasureFlowDocumentHeight(pageEndSumFlowDocs[0]);
                            _basePaginator = ((IDocumentPaginatorSource)pageEndSumFlowDocs[1]).DocumentPaginator;
                            pageEndSumFlowDocs[1].PageWidth = A3PaperAbstraction.mm_to_diu(A3PaperAbstraction.widthLandscape_mm) - _pageMargins.Left - _pageMargins.Right;
                            pageEndSumPage1 = _basePaginator.GetPage(0);
                            pageEndSumPage1Size = MeasureFlowDocumentHeight(pageEndSumFlowDocs[1]);

                            this.isSectionInterrupted = true;

                            dc.DrawRectangle(
                                new VisualBrush(pageEndSumPage0.Visual)
                                {
                                    Stretch = Stretch.None,
                                    AlignmentX = AlignmentX.Left,
                                    AlignmentY = AlignmentY.Top,
                                },
                                null,
                                new Rect(
                                            _pageMargins.Left,
                                            _pageMargins.Top + cumulativeHeight,
                                            this._pageSize.Width - _pageMargins.Left - _pageMargins.Right,
                                            pageEndSumPage0Size
                                        )
                                );
                            this.Pages.Add(
                                new DocumentPage(
                                    visual,
                                    _pageSize,
                                    new Rect(new Point(0, 0), _pageSize),
                                    new Rect(
                                        _pageMargins.Left,
                                        _pageMargins.Top,
                                        _pageSize.Width,
                                        _pageSize.Height
                                    )
                                )
                            );
                            break; // if the page is full exit the inner loop (over the lines) to return to the outer loop (over the pages) to create a new visual 
                        }

                    }

                    if (currentLineIndex >= this.dataLinesFlowDocuments.Count)
                    {
                        dc.DrawRectangle(
                            new VisualBrush(totalsPage.Visual)
                            {
                                Stretch = Stretch.None,
                                AlignmentX = AlignmentX.Left,
                                AlignmentY = AlignmentY.Top
                            },
                            null,
                            new Rect(
                                        _pageMargins.Left,
                                        _pageMargins.Top + cumulativeHeight,
                                        this._pageSize.Width - _pageMargins.Left - _pageMargins.Right,
                                        totalsSize
                                    )
                            );
                        this.Pages.Add(
                            new DocumentPage(
                                visual,
                                _pageSize,
                                new Rect(new Point(0, 0), _pageSize),
                                new Rect(
                                    _pageMargins.Left,
                                    _pageMargins.Top,
                                    _pageSize.Width,
                                    _pageSize.Height
                                )
                            )
                        );
                        break;
                    }
                }

            }
        }

        public override DocumentPage GetPage(int pageNumber)=>this.Pages.ElementAt(pageNumber);
        
        public override bool IsPageCountValid => true;
        public override Size PageSize
        {
            get => _pageSize;
            set
            {
                _pageSize = value;
                Pages.Clear();
            }
        }
        public override int PageCount => this.Pages.Count;
        public override IDocumentPaginatorSource Source => null;

    }
}
