using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
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
    public partial class inputAndDisplayPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private AnlageverzeichnisDocument document = new();
        [ObservableProperty]
        private dataEntryLine currentlyEditedLine = new();

        public inputAndDisplayPageViewModel()
        {

        }

        public inputAndDisplayPageViewModel(AnlageverzeichnisDocument document)
        {
            this.Document = document;

        }
    }

    /// <summary>
    /// Interaktionslogik für inputAndDisplayPage.xaml
    /// </summary>
    public partial class inputAndDisplayPage : Page
    {
        private string filename { get; set; } = "";
        public inputAndDisplayPage()
        {
            DataContext = new inputAndDisplayPageViewModel();
            InitializeComponent();
        }
        public inputAndDisplayPage(AnlageverzeichnisDocument document, string fileName)
        {
            DataContext = new inputAndDisplayPageViewModel(document);
            this.filename = fileName; // so that we have a way of memorizing where the file is stored that had been created in the general information input page
            InitializeComponent();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            using (var outfile = new StreamWriter(this.filename))
            {
                outfile.Write(JsonSerializer.Serialize<AnlageverzeichnisDocument>((AnlageverzeichnisDocument)(this.DataContext)));
            }

            MessageBox.Show("Gespeichert!", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void reloadButton_Click(object sender, RoutedEventArgs e)
        {
            using (var infile = new StreamReader(this.filename))
            {
                this.DataContext=JsonSerializer.Deserialize<AnlageverzeichnisDocument>(infile.ReadToEnd());
            }

            MessageBox.Show("Gespeichert!", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void applyLineButton_Click(object sender, RoutedEventArgs e)
        {
            ((inputAndDisplayPageViewModel)(this.DataContext)).Document.DataEntryLines.Append(((inputAndDisplayPageViewModel)(this.DataContext)).CurrentlyEditedLine);
        }
    }
}
