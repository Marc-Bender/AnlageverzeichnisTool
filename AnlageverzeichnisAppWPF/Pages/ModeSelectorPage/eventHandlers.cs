using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AnlageverzeichnisAppWPF
{
    public partial class modeSelectorPage : Page
    {
        private void newButton_Click(object sender, RoutedEventArgs e) => newFunction();
        private void newFromPreviousButton_Click(object sender, RoutedEventArgs e) => newFromExistingFunction();
        private void loadButton_Click(object sender, RoutedEventArgs e) => openFunction();

    }
}
