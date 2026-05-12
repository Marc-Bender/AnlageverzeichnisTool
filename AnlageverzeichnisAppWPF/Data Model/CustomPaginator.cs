using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace AnlageverzeichnisAppWPF
{
    public class CustomPaginator : DocumentPaginator
    {
        private static readonly int A3PaperHeightLandscape_mm = 297;
        private static readonly int A3PaperWidthLandscape_mm = 420;
        private static readonly int DPI = 96;
        private static readonly float mm_per_inch = 25.4f;
        private static readonly int pageMargins_mm = 10;

        private readonly DocumentPaginator? _basePaginator;
        private readonly Typeface _headerTypeface = new Typeface("Courier New");
        private readonly double _headerFontSize = 12;
        private readonly Thickness _pageMargins = new Thickness(pageMargins_mm / mm_per_inch * DPI);

        private AnlageverzeichnisDocument? document;
        public CustomPaginator(FlowDocument flowDoc)
        {
            if(flowDoc.Tag is AnlageverzeichnisDocument document)
            {
                this.document = document;
                _basePaginator = ((IDocumentPaginatorSource)flowDoc).DocumentPaginator;
                _basePaginator.PageSize = new Size(
                                                    A3PaperWidthLandscape_mm / mm_per_inch * DPI, 
                                                    A3PaperHeightLandscape_mm / mm_per_inch * DPI
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
                
                FormattedText companyInformationText = new FormattedText(
                    $"{this.document.Header.CompanyName}\n{this.document.Header.CompanyCityAndZipCode}",
                    new CultureInfo("de-de"),
                    FlowDirection.LeftToRight,
                    _headerTypeface,
                    _headerFontSize,
                    Brushes.Black,
                    1.0);
                
                dc.DrawText(companyInformationText, new Point(
                                                                _pageMargins.Left ,
                                                                _pageMargins.Top
                                                             )
                           );

                var mainDocumentStartYCoordinate = _pageMargins.Top + companyInformationText.Height;

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
                                                    _pageMargins.Top + companyInformationText.Height
                                                 )
                               );
                    mainDocumentStartYCoordinate += title.Height;
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
                                _pageMargins.Top + companyInformationText.Height + 10,
                                page.Size.Width,
                                page.Size.Height - mainDocumentStartYCoordinate - _pageMargins.Bottom 
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
