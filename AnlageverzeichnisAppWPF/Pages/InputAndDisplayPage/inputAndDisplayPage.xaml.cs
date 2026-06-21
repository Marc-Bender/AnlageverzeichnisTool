using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnlageverzeichnisAppWPF
{
    /// <summary>
    /// Interaktionslogik für inputAndDisplayPage.xaml
    /// </summary>
    public partial class inputAndDisplayPage : Page
    {
        private string filename { get; set; } = "";
        public inputAndDisplayPage()
        {
            DataContext = new inputAndDisplayPageViewModel();
            this.Loaded += (_, __) => registerHotkeys();
            this.Unloaded += (_, __) => unregisterHotkeys();
            InitializeComponent();
        }
        public inputAndDisplayPage(AnlageverzeichnisDocument document, string fileName)
        {
            DataContext = new inputAndDisplayPageViewModel(document);
            this.filename = fileName; // so that we have a way of memorizing where the file is stored that had been created in the general information input page
            this.Loaded += (_, __) => registerHotkeys();
            this.Unloaded += (_, __) => unregisterHotkeys();
            InitializeComponent();
        }

        private void dataEntryLinesDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // need to handle recalculation on :
            // mm/jjjj, eur hist, % deprecation, isLeavingCheckbox
            if(
                  (e.Row.Item is dataEntryLine line)
                &&(
                        (e.Column == purchaseDateDataGridColumn)
                      ||(e.Column == historicPriceDataGridColumn)
                      ||(e.Column == percentageDeprecationDataGridColumn)
                      ||(e.Column == isLeavingCheckBoxColumn)
                  )
                &&(sender is System.Windows.Controls.DataGrid grid)
                &&(grid.DataContext is inputAndDisplayPageViewModel vm)
                &&(vm.IsExpertModeEnabled == false)
              )
            {
                line.IsCalculateDerivedFieldsNeeded = true;
            }
        }

        private void dataEntryLinesDataGrid_RowEditEnding(object sender, SelectionChangedEventArgs e)
        {
            if (true)//(e.RemovedItems is Array<object> oldLines)
            {
                foreach (var lineobj in e.RemovedItems)
                {
                    if(lineobj is dataEntryLine line)
                    {
                        if (line.IsCalculateDerivedFieldsNeeded == true)
                        {
                            line.IsInvalid = false;
                            try
                            {
                                line.handleCalculateDerivedFieldsOnUpdate();
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                line.IsInvalid = true;
                                MessageBox.Show("Ein Feld hat einen unerlaubten Wert", "Wert-Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }
                    }
                }
            }    
        }

        private void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(this.DataContext is inputAndDisplayPageViewModel vm)
            {
                foreach(var line in vm.Document.DataEntryLines)
                {
                    line.IsCurrentHeading = false; // first clear the current heading state of all data entries in the document ...
                }

                if(
                        (sender is Label label)
                     && (label.DataContext is dataEntryLine thisLine)
                  )
                {
                    thisLine.IsCurrentHeading = true; // before setting the property for the line clicked -- this ensures that there is only ever one line with this being set in the document; which is needed because under that heading there shall be all the new items be added
                }
            }
        }

        private void isHeadingCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            deprecationInYearsCheckBox.IsChecked = false;
        }
    }
}
