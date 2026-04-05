using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// Interaktionslogik für generalInformationInputPage.xaml
    /// </summary>
    public partial class generalInformationInputPage : Page
    {
        public generalInformationInputPage()
        {
            DataContext = new AnlageverzeichnisDocument.DocumentHeader();
            InitializeComponent();
        }

        private void currentlyWorkedOnYearTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog();
            
            saveDialog.DefaultExt = ".json";
            saveDialog.AddExtension = true;
            saveDialog.Filter = "JSON-Dateien | *.json";
            if (
                    (saveDialog.ShowDialog() == true)
                  &&(saveDialog.FileName != "")
               )
            {
                var document = new AnlageverzeichnisDocument((AnlageverzeichnisDocument.DocumentHeader)(this.DataContext));
                using (var outfile = new StreamWriter(saveDialog.FileName))
                {
                    outfile.Write(JsonSerializer.Serialize<AnlageverzeichnisDocument>(document));
                }
                var inputAndDisplayPage = new inputAndDisplayPage(document, saveDialog.FileName);
                inputAndDisplayPage.Tag = this.Tag;
                if (this.Tag is MainWindowViewModel mwvm)
                {
                    mwvm.ActivePageSaveCommand = inputAndDisplayPage.SaveCommand;
                    mwvm.ActivePageReloadCommand = inputAndDisplayPage.ReloadCommand;
                    mwvm.ActivePageApplyCommand = inputAndDisplayPage.ApplyCommand;
                }
                NavigationService.Navigate(inputAndDisplayPage);
            }
            else
            {
                MessageBox.Show("Keine Datei gewählt -- Anlageverzeichnis anlage abgebrochen", "Keine Datei", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }
    }
}
