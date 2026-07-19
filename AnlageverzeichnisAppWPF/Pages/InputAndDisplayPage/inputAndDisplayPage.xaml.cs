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
        public inputAndDisplayPage(UIDocument document, string fileName)
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
                  (e.Row.Item is UIDataLine line)
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
                    if(lineobj is UIDataLine line)
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
                     && (label.DataContext is UIDataLine thisLine)
                  )
                {
                    if (thisLine.IsHeading == true)
                    {
                        thisLine.IsCurrentHeading = true; // before setting the property for the line clicked -- this ensures that there is only ever one line with this being set in the document; which is needed because under that heading there shall be all the new items be added
                    }
                    else
                    {
                        UIDataLine newHeadingLine = new UIDataLine(thisLine.currentYear);
                        newHeadingLine.IsHeading = true;
                        newHeadingLine.IsCurrentHeading = true;
                        newHeadingLine.ObjectDescriptionText = "Neue Überschrift";
                        vm.Document.DataEntryLines.Insert(
                                                            vm.Document.DataEntryLines.IndexOf(thisLine),
                                                            newHeadingLine
                                                         );
                    }
                }
            }
        }

        private void isHeadingCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            deprecationInYearsCheckBox.IsChecked = false;
        }

        private void DataGridRow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                row.IsSelected = true;
                row.Focus();
            }
        }

        private void deleteRowsButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.DataContext is inputAndDisplayPageViewModel vm)
            {
               var selected = dataEntryLinesDataGrid.SelectedItems.Cast<UIDataLine>().ToList();
               foreach (var item in selected)
                {
                    if(item is UIDataLine line)
                    {
                        vm.Document.DataEntryLines.Remove(line);
                    }
                }
            }
        }

        private void recalculateRowsButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.DataContext is inputAndDisplayPageViewModel vm)
            {
                foreach (var item in dataEntryLinesDataGrid.SelectedItems)
                {
                    if(item is UIDataLine line)
                    {
                        line.calculateDerivedFields(vm.Document.Header.CurrentlyWorkedOnYear);
                    }
                }
            }
        }

        private void isAggregationPositionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is inputAndDisplayPageViewModel vm)
            {
                if (vm.CurrentlyEditedLine is UIDataLine line)
                {
                    line.MonthOfPurchase = 1;
                    line.DepreciationPercentage_0P1Pct = 200;
                }
            }

        }

        private void isNonDeprecatingCheckbox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
