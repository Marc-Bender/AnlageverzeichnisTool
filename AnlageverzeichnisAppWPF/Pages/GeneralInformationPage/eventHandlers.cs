using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace AnlageverzeichnisAppWPF
{
    public partial class generalInformationInputPage : Page
    {
        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog();

            saveDialog.DefaultExt = ".json";
            saveDialog.AddExtension = true;
            saveDialog.Filter = "JSON-Dateien | *.json";
            if (
                    (saveDialog.ShowDialog() == true)
                  && (saveDialog.FileName != "")
               )
            {
                var document = new UIDocument((UIDocumentHeader)(this.DataContext));
                using (var outfile = new StreamWriter(saveDialog.FileName))
                {
                    outfile.Write(JsonSerializer.Serialize<StoredDocument>(document.toStoredDocumentType()));
                }
                var inputAndDisplayPage = new inputAndDisplayPage(document, saveDialog.FileName);
                inputAndDisplayPage.Tag = this.Tag;
                NavigationService.Navigate(inputAndDisplayPage);
            }
            else
            {
                MessageBox.Show("Keine Datei gewählt -- Anlageverzeichnis anlage abgebrochen", "Keine Datei", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        private void TextBoxes_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textbox)
            {
                textbox.SelectAll();
            }
        }
    }
}
