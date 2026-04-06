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
            this.Loaded += (_, __) => { companyNameTextBox.Focus();};
        }

    }
}
