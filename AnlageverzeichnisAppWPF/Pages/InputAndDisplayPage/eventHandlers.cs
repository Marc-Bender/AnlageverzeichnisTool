using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AnlageverzeichnisAppWPF
{
    public partial class inputAndDisplayPage : Page
    {
        private void saveButton_Click(object sender, RoutedEventArgs e) => Save();

        private void reloadButton_Click(object sender, RoutedEventArgs e) => Reload();

        private void applyLineButton_Click(object sender, RoutedEventArgs e) => Apply();

        private void objectDescriptionTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.DataContext is inputAndDisplayPageViewModel vm)
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
                            (e.Key == Key.F12)
                         || (e.SystemKey == Key.F12)
                        )
                {
                    vm.CurrentlyEditedLine.IsAggregatingPosition = !vm.CurrentlyEditedLine.IsAggregatingPosition;
                    e.Handled = true;
                }
                else if (
                            (e.Key == Key.F8)
                         || (e.SystemKey == Key.F8)
                        )
                {
                    vm.CurrentlyEditedLine.IsNonDeprecating = !vm.CurrentlyEditedLine.IsNonDeprecating;
                    e.Handled = true;
                }
            }
        }
        private void NumericTextBoxes_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.DataContext is inputAndDisplayPageViewModel vm)
            {
                switch (e.Key)
                {
                    case Key.OemComma:
                        {
                            e.Handled = true; // mask the comma from being typed as typing the comma is not needed with the existing value converters inplace
                        }
                        break;
                    case Key.OemMinus:
                    case Key.Subtract:
                        {
                            if(isLeavingThisYearCheckBox.IsEnabled == true)
                            {
                                // avoid changing this via hotkey if not also allowed via mouse click...
                                vm.CurrentlyEditedLine.IsLeavingThisYear = !vm.CurrentlyEditedLine.IsLeavingThisYear;
                            }
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
                    case Key.F12:
                        {
                            e.Handled = true;
                            vm.CurrentlyEditedLine.IsAggregatingPosition = !vm.CurrentlyEditedLine.IsAggregatingPosition;
                        }
                        break;
                    case Key.F8:
                        {
                            e.Handled = true;
                            vm.CurrentlyEditedLine.IsNonDeprecating = !vm.CurrentlyEditedLine.IsNonDeprecating;
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
            }
            else
            {
                NumericTextBoxesCommonBehaviors_PreviewKeyDown(sender, e);
            }
        }
        private void yearOfPurchaseTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (

                    (
                        (e.Key == Key.Tab)
                      ||(e.Key == Key.Enter)
                    )
                 && (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) == false) // only on "forward" tab get to price field
               )
            {
                e.Handled = true;
                priceAtPurchaseTextBox.Focus();
            }
            else if (
                        (
                            (e.Key == Key.Tab)
                         || (e.Key == Key.Enter)
                        )
                     && (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) == true) // on "backwards" tab go to month field instead
                    )
            {
                e.Handled = true;
                monthOfPurchaseNumberBox.Focus();
            }
            else
            {
                NumericTextBoxesCommonBehaviors_PreviewKeyDown(sender, e);
            }
        }

        private void priceAtPurchaseTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (
                   (
                        (e.Key == Key.Tab)
                     || (e.Key == Key.Enter)
                   )
                && (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) == true) // on "backwards" tab go to year field instead of month... 
               )
            {
                e.Handled = true;
                yearOfPurchaseNumberBox.Focus();
            }
            else
            {
                NumericTextBoxesCommonBehaviors_PreviewKeyDown(sender, e);
            }
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
                TraversalRequest request = new TraversalRequest(Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)?FocusNavigationDirection.Previous:FocusNavigationDirection.Next);
                (Keyboard.FocusedElement as UIElement)?.MoveFocus(request);
            }
            else if (
                           (e.Key == Key.F9)
                        || (e.SystemKey == Key.F9)
                    )
            {
                deprecationInYearsCheckBox.IsChecked = !deprecationInYearsCheckBox.IsChecked;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textbox)
            {
                textbox.SelectAll();
            }
        }
        private void TextBox_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            if (
                   (sender is TextBox textbox)
                && (textbox.IsKeyboardFocusWithin == false)
              )
            {
                e.Handled = true;
                textbox.Focus();
            }
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
            if (e.Key == Key.Escape)
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

        private void newEntryButton_Click(object sender, RoutedEventArgs e) => NewEntry();
        private async void createPDFButton_Click(object sender, RoutedEventArgs e) => await CreatePDF();

    }
}
