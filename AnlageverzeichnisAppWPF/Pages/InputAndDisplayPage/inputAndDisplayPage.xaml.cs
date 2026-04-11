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

        private void dataEntryLinesDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if(
                  (e.Row.Item is dataEntryLine line)
                &&(e.Column == isleavingcheckboxcol)
              )
            {
                line.IsCalculateDerivedFieldsNeeded = true;
            }
        }
    }
}
