using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace AnlageverzeichnisAppWPF
{
    public class A3PaperAbstraction
    {
        public static readonly int heightLandscape_mm = 297;
        public static readonly int widthLandscape_mm = 420;
        public static readonly int DPI = 96;
        public static readonly float mm_per_inch = 25.4f;
        public static readonly int pageMargins_mm = 10;
    }

    public class CustomPaginator : DocumentPaginator
    {
        private readonly DocumentPaginator? _basePaginator;
        private readonly Typeface _headerTypeface = new Typeface("Courier New");
        private readonly double _headerFontSize = 12;
        private readonly Thickness _pageMargins = new Thickness(A3PaperAbstraction.pageMargins_mm / A3PaperAbstraction.mm_per_inch * A3PaperAbstraction.DPI);

        private AnlageverzeichnisDocument? document;
        public CustomPaginator(FlowDocument flowDoc)
        {
            if(flowDoc.Tag is AnlageverzeichnisDocument document)
            {
                this.document = document;
                _basePaginator = ((IDocumentPaginatorSource)flowDoc).DocumentPaginator;
                _basePaginator.PageSize = new Size(
                                                    A3PaperAbstraction.widthLandscape_mm / A3PaperAbstraction.mm_per_inch * A3PaperAbstraction.DPI, 
                                                    A3PaperAbstraction.heightLandscape_mm / A3PaperAbstraction.mm_per_inch * A3PaperAbstraction.DPI
                                                  );
            }
        }

        public override DocumentPage GetPage(int pageNumber)
        {
            if(
                    (this._basePaginator is null)
                 || (this.document is null)
              )
            {
                return new DocumentPage(new DrawingVisual());
            }
            // Get the original FlowDocument page
            DocumentPage page = _basePaginator.GetPage(pageNumber);

            // Create a new visual to draw header/footer
            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
                if (pageNumber == 0)
                {
                    // Draw title 
                    FormattedText title = new FormattedText(
                        $"Anlageverzeichnis zum 31.12.{this.document.Header.CurrentlyWorkedOnYear}",
                        new CultureInfo("de-de"),
                        FlowDirection.LeftToRight,
                        _headerTypeface,
                        _headerFontSize,
                        Brushes.Black,
                        1.0);
                
                    dc.DrawText(title, new Point(
                                                    (page.Size.Width / 2) - (title.Width / 2) ,
                                                    _pageMargins.Top
                                                 )
                               );
                }
                else
                {
                    // no title on the subsequent pages
                }

                // Draw footer (page number)
                FormattedText footer = new FormattedText(
                    $"Seite {pageNumber + 1}",
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    _headerTypeface,
                    12,
                    Brushes.Gray,
                    1.0);

                dc.DrawText(footer, new Point(
                                                (page.Size.Width / 2) + (footer.Width / 2), 
                                                page.Size.Height - _pageMargins.Bottom + 5
                                             )
                           );

                // Draw the original page content
                dc.DrawRectangle(
                    new VisualBrush(page.Visual),
                    null,
                    new Rect(
                                _pageMargins.Left, 
                                0,
                                page.Size.Width - _pageMargins.Left - _pageMargins.Right,
                                page.Size.Height - _pageMargins.Bottom 
                            )
                    );

            }

            return new DocumentPage(
                visual,
                page.Size,
                page.BleedBox,
                page.ContentBox);
        }

        public override bool IsPageCountValid => _basePaginator is not null?_basePaginator.IsPageCountValid:false;
        public override int PageCount => _basePaginator is not null ? _basePaginator.PageCount : 0;
        public override Size PageSize { 
                    get => _basePaginator is not null ? _basePaginator.PageSize : new Size(); 
                    set => _basePaginator.PageSize = value ; 
        }
        public override IDocumentPaginatorSource Source => _basePaginator.Source;
    }
}
