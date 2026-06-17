using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;

namespace AnlageverzeichnisAppWPF
{
    public partial class inputAndDisplayPage : Page
    {
        private void registerHotkeys()
        {
            if (this.Tag is MainWindowViewModel mwvm)
            {
                mwvm.ActivePageSaveCommand = SaveCommand;
                mwvm.ActivePageReloadCommand = ReloadCommand;
                mwvm.ActivePageApplyCommand = ApplyCommand;
                mwvm.ActivePageNewCommand = NewEntryCommand;
                mwvm.ActivePagePDFCommand = CreatePDFCommand;
            }
        }

        private void unregisterHotkeys()
        {
            if (this.Tag is MainWindowViewModel mwvm)
            {
                mwvm.ActivePageSaveCommand = null;
                mwvm.ActivePageReloadCommand = null;
                mwvm.ActivePageApplyCommand = null;
                mwvm.ActivePageNewCommand = null;
                mwvm.ActivePagePDFCommand = null;
            }

        }

        public ICommand SaveCommand => new RelayCommand(Save);
        public ICommand ReloadCommand => new RelayCommand(Reload);
        public ICommand ApplyCommand => new RelayCommand(Apply);
        public ICommand NewEntryCommand => new RelayCommand(NewEntry);
        public ICommand CreatePDFCommand => new RelayCommand(async () => await CreatePDF());
        private void Save()
        {
            using (var outfile = new StreamWriter(this.filename))
            {
                if (this.DataContext is inputAndDisplayPageViewModel vm)
                {
                    outfile.Write(JsonSerializer.Serialize<AnlageverzeichnisDocument>((AnlageverzeichnisDocument)(vm.Document)));
                }
            }

            MessageBox.Show("Gespeichert!", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Reload()
        {
            using (var infile = new StreamReader(this.filename))
            {
                if (this.DataContext is inputAndDisplayPageViewModel vm)
                {
                    vm.Document = JsonSerializer.Deserialize<AnlageverzeichnisDocument>(infile.ReadToEnd());
                    if (vm.Document is AnlageverzeichnisDocument document)
                    {
                        document.applyCurrentYearToImportedDataEntries(); // needs to be done to ensure correct calculation of derived fields if the contents are modified in the datagrid later.
                    }
                }
            }

            MessageBox.Show("Auf gespeicherten Zustand zurückgesetzt!", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Apply()
        {
            if (
                    (this.DataContext is inputAndDisplayPageViewModel vm)
                 && (vm.Document.DataEntryLines is ObservableCollection<dataEntryLine> lines)
               )
            {
                try
                {
                    vm.CurrentlyEditedLine.calculateDerivedFields(vm.Document.Header.CurrentlyWorkedOnYear);
                }
                catch (NoNullAllowedException)
                {
                    MessageBox.Show("Ein Wert ist Null der nicht Null sein darf!", "Null-Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                catch (ArgumentOutOfRangeException)
                {
                    MessageBox.Show("Ein Wert hat einen unerlaubten Betrag", "Wert-Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (vm.CurrentlyEditedLine.IsHeading == true)
                {
                    vm.CurrentlyEditedLine.IsCurrentHeading = true; // when a heading is newly added to the dataset then assume that it should be the heading under which all new data shall be generated going forward (until explicitly overridden)
                }

                try
                {
                    var indexOfCurrentHeading = lines.IndexOf(lines.Where(x => x.IsCurrentHeading == true).ElementAt(0));
                    var indexOfHeadingAfterCurrentHeading = lines.IndexOf(
                                                                            lines.Where(x =>
                                                                                                   (x.IsHeading == true)
                                                                                                && (lines.IndexOf(x) > indexOfCurrentHeading)
                                                                                       ).ElementAt(0)
                                                                         );
                    foreach (var line in vm.Document.DataEntryLines)
                    {
                        line.IsCurrentHeading = false; // first clear the current heading state of all data entries in the document ...
                    }

                    lines.Insert(indexOfHeadingAfterCurrentHeading, vm.CurrentlyEditedLine); // inserting at the location of the heading after the selected heading will effectively add the element at the end of the block started with the selected heading... 
                }
                catch
                {
                    foreach (var line in vm.Document.DataEntryLines)
                    {
                        line.IsCurrentHeading = false; // first clear the current heading state of all data entries in the document ...
                    }

                    lines.Add(vm.CurrentlyEditedLine);
                }
                vm.CurrentlyEditedLine = new dataEntryLine(vm.Document.Header.CurrentlyWorkedOnYear);
                deprecationInYearsCheckBox.IsChecked = false;
                objectDescriptionTextBox.Focus();
            }
        }

        private void NewEntry()
        {
            // since this may be reached via hotkey if the expert mode is disabled (ie the button for clicking is not visible / not enabled) we need to check if the expert mode is enabled here to keep things simple
            if(
                    (this.DataContext is inputAndDisplayPageViewModel vm)
                 && (vm.IsExpertModeEnabled == true)
                 && (this.dataEntryLinesDataGrid.ItemsSource is ObservableCollection<dataEntryLine> lines)
              )
            {
                try
                {
                    var indexOfCurrentHeading = lines.IndexOf(lines.Where(x => x.IsCurrentHeading == true).ElementAt(0));
                    var indexOfHeadingAfterCurrentHeading = lines.IndexOf(
                                                                            lines.Where(x =>
                                                                                                (x.IsHeading == true)
                                                                                             && (lines.IndexOf(x) > indexOfCurrentHeading)
                                                                                       ).ElementAt(0)
                                                                         );
                    foreach (var line in vm.Document.DataEntryLines)
                    {
                        line.IsCurrentHeading = false; // first clear the current heading state of all data entries in the document ...
                    }

                    lines.Insert(indexOfHeadingAfterCurrentHeading, new dataEntryLine()); // inserting at the location of the heading after the selected heading will effectively add the element at the end of the block started with the selected heading... 
                }
                catch 
                {
                    foreach (var line in vm.Document.DataEntryLines)
                    {
                        line.IsCurrentHeading = false; // first clear the current heading state of all data entries in the document ...
                    }

                    lines.Add(new dataEntryLine()); 
                }
            }
        }

        private async Task CreatePDF()
        {
            if (
                    (this.DataContext is inputAndDisplayPageViewModel vm)
                  &&(vm.Document is AnlageverzeichnisDocument doc)
               )
            {
                // the below needs to be rethought once the paginator is doing the pagination stuff for real
                this.pdfCreationProgressBar.Visibility = Visibility.Visible;
                var paginator = new CustomPaginator(doc);
                await Task.Run(
                    async () => 
                    {
                        await paginator.BuildPagesAsync(doc);

                    });
                var dlg = new PrintDialog();

                dlg.PrintDocument(paginator, $"Anlageverzeichnis {doc.Header.CurrentlyWorkedOnYear} {doc.Header.CompanyName}");
                this.pdfCreationProgressBar.Visibility = Visibility.Hidden;
            }
        }
    }
}
