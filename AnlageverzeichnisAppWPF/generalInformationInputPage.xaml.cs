using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
            InitializeComponent();
        }

        private void currentlyWorkedOnYearTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog();
            do
            {
                saveDialog.DefaultExt = ".json";
                saveDialog.AddExtension = true;
                saveDialog.Filter = ".json | JSON Dateien";
                saveDialog.ShowDialog();
                if(saveDialog.FileName == "")
                { 
                    MessageBox.Show("Ein Dateiname ist erforderlich; bitte noch mal probieren", "Fehler - kein Dateiname", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // no error -- the loop can be left by the outer condition
                }
            }
            while (saveDialog.FileName == "");

            var applicationContext = new AnlageverzeichnisDocument(
                                            companyNameTextBox.Text, 
                                            companyCityAndZipCodeTextB.Text, 
                                            int.Parse(currentlyWorkedOnYearTextBox.Text)
                                            );
            using (var outfile = new StreamWriter(saveDialog.FileName))
            {
                outfile.Write(JsonSerializer.Serialize<AnlageverzeichnisDocument>(applicationContext));
            }

            var inputAndDisplayPage = new inputAndDisplayPage();
            inputAndDisplayPage.Tag = applicationContext;

            NavigationService.Navigate(inputAndDisplayPage);
        }
    }
}
