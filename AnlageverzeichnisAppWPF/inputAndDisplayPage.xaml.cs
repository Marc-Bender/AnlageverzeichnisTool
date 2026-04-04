using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Data;
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
        private dataEntryLine currentlyEditedLine;

        public inputAndDisplayPageViewModel()
        {
            this.CurrentlyEditedLine = new dataEntryLine(Document.Header.CurrentlyWorkedOnYear);
        }

        public inputAndDisplayPageViewModel(AnlageverzeichnisDocument document)
        {
            this.Document = document;
            this.CurrentlyEditedLine = new dataEntryLine(Document.Header.CurrentlyWorkedOnYear);
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
                if (this.DataContext is inputAndDisplayPageViewModel vm)
                {
                    outfile.Write(JsonSerializer.Serialize<AnlageverzeichnisDocument>((AnlageverzeichnisDocument)(vm.Document)));
                }
            }

            MessageBox.Show("Gespeichert!", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void reloadButton_Click(object sender, RoutedEventArgs e)
        {
            using (var infile = new StreamReader(this.filename))
            {
                if (this.DataContext is inputAndDisplayPageViewModel vm)
                {
                    vm.Document = JsonSerializer.Deserialize<AnlageverzeichnisDocument>(infile.ReadToEnd());
                }
            }

            MessageBox.Show("Gespeichert!", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void applyLineButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is inputAndDisplayPageViewModel vm)
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
                vm.Document.DataEntryLines.Add(vm.CurrentlyEditedLine);
                vm.CurrentlyEditedLine = new dataEntryLine(vm.Document.Header.CurrentlyWorkedOnYear);
                objectDescriptionTextBox.Focus();
            }
        }

        private void objectDescriptionTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(this.DataContext is inputAndDisplayPageViewModel vm)
            {
                if (
                        (e.Key == Key.F10)
                     || (e.SystemKey == Key.F10)
                   )
                {
                    vm.CurrentlyEditedLine.IsHeading = !vm.CurrentlyEditedLine.IsHeading;
                    e.Handled = true; 
                }
                else if (
                            (e.Key == Key.F11)
                         || (e.SystemKey == Key.F11)
                        )
                {
                    applyLineButton_Click(sender, e);
                }
            }
        }
        private void NumericTextBoxes_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.DataContext is inputAndDisplayPageViewModel vm)
            {
                switch(e.Key)
                {
                    case Key.OemComma:
                    {
                        e.Handled = true; // mask the comma from being typed as typing the comma is not needed with the existing value converters inplace
                    }
                    break;
                    case Key.OemMinus:
                    case Key.Subtract:
                    {
                        vm.CurrentlyEditedLine.IsLeavingThisYear = !vm.CurrentlyEditedLine.IsLeavingThisYear;
                        e.Handled = true;
                    }
                    break;
                    case Key.E:
                    {
                        vm.CurrentlyEditedLine.DisplayAsMemorialValue = !vm.CurrentlyEditedLine.DisplayAsMemorialValue;
                        e.Handled = true;
                    }
                    break;
                    case Key.Multiply:
                    {
                        e.Handled = true;
                        applyLineButton_Click(sender, e);
                    }
                    break;
                }
            }
        }
        private void NumericTextBoxes_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true; // mask the comma from being typed as typing the comma is not needed with the existing value converters inplace
                                  // Move focus to next control (same behavior as Tab)
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                (Keyboard.FocusedElement as UIElement)?.MoveFocus(request);
            }
            else if (
                        (e.Key == Key.F11)
                     || (e.SystemKey == Key.F11)
                    )
            {
                applyLineButton_Click(sender, e);
            }


        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if(sender is TextBox textbox)
            {
                textbox.SelectAll();
            }
        }
        private void TextBox_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            if(
                   (sender is TextBox textbox )
                && (textbox.IsKeyboardFocusWithin == false)
              )
            {
                e.Handled = true;
                textbox.Focus();
            }
        }
    }
}
