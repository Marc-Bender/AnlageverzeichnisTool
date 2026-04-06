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
        private void registerHotkeys()
        {
            if (this.Tag is MainWindowViewModel mwvm)
            {
                mwvm.ActivePageSaveCommand = SaveCommand;
                mwvm.ActivePageReloadCommand = ReloadCommand;
                mwvm.ActivePageApplyCommand = ApplyCommand;
            }
        }
        
        private void unregisterHotkeys()
        {
            if (this.Tag is MainWindowViewModel mwvm)
            {
                mwvm.ActivePageSaveCommand = null;
                mwvm.ActivePageReloadCommand = null;
                mwvm.ActivePageApplyCommand = null;
            }

        }

        public ICommand SaveCommand => new RelayCommand(Save);
        public ICommand ReloadCommand => new RelayCommand(Reload);
        public ICommand ApplyCommand => new RelayCommand(Apply);
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
                }
            }

            MessageBox.Show("Auf gespeicherten Zustand zurückgesetzt!", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Apply()
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
                deprecationInYearsCheckBox.IsChecked = false;
                objectDescriptionTextBox.Focus();
            }
        }
        private void saveButton_Click(object sender, RoutedEventArgs e) => Save();

        private void reloadButton_Click(object sender, RoutedEventArgs e) => Reload();

        private void applyLineButton_Click(object sender, RoutedEventArgs e) => Apply();
        
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
                        Apply();
                    }
                    break;
                    case Key.Divide:
                    {
                        e.Handled = true;
                        this.yearOfPurchaseNumberBox.Focus();
                    }
                    break;
                }
            }
        }
        private void monthOfPurchaseTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Divide)
            {
                e.Handled = true;
                yearOfPurchaseNumberBox.Focus();
                return;
            }

            NumericTextBoxesCommonBehaviors_PreviewKeyDown(sender, e);
        }
        private void yearOfPurchaseTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (
                    (e.Key == Key.Tab)
                 && (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift) == false) // only on "forward" tab get to price field
               )
            {
                e.Handled = true;
                priceAtPurchaseTextBox.Focus();
                return;
            }
            else if (
                        (e.Key == Key.Tab)
                     && (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift) == true) // on "backwards" tab go to month field instead
                    )
            {
                e.Handled = true;
                monthOfPurchaseNumberBox.Focus();
                return;
            }
            NumericTextBoxesCommonBehaviors_PreviewKeyDown(sender, e);
        }

        private void priceAtPurchaseTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (
                   (e.Key == Key.Tab)
                && (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift) == true) // on "backwards" tab go to year field instead of month... 
               )
            {
                e.Handled = true;
                yearOfPurchaseNumberBox.Focus();
                return;
            }
            NumericTextBoxesCommonBehaviors_PreviewKeyDown(sender, e);
        }

        private void deprecationPercentageTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            NumericTextBoxesCommonBehaviors_PreviewKeyDown(sender, e);
        }
        private void NumericTextBoxesCommonBehaviors_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                // Move focus to next control (same behavior as Tab)
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                (Keyboard.FocusedElement as UIElement)?.MoveFocus(request);
            }
            else if (
                          (e.Key == Key.F9)
                        ||(e.SystemKey == Key.F9)
                    )
            {
                deprecationInYearsCheckBox.IsChecked = !deprecationInYearsCheckBox.IsChecked;
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

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            // need to handle like this for esp. w/ the refresh for the change to actually propagate to both the data behind and the UI to update accordingly
            dataEntryLinesDataGrid.BeginEdit();
            dataEntryLinesDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            dataEntryLinesDataGrid.Items.Refresh();
        }

        private void TextBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            dataEntryLinesDataGrid.BeginEdit();
        }

        private void deprecationYearsNumberBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            NumericTextBoxesCommonBehaviors_PreviewKeyDown(sender, e);
        }

        private void dataEntryLinesDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                e.Handled = true;
                dataEntryLinesDataGrid.CommitEdit();
                Keyboard.Focus(this);
                Dispatcher.BeginInvoke(new Action(() =>
                        {
                            Keyboard.Focus(objectDescriptionTextBox);
                        }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }
    }
}
