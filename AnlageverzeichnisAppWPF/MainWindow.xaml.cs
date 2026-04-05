using CommunityToolkit.Mvvm.ComponentModel;
using System.Text;
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
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private ICommand? activePageSaveCommand;
        [ObservableProperty]
        private ICommand? activePageReloadCommand;
        [ObservableProperty]
        private ICommand? activePageApplyCommand;
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            this.DataContext = new MainWindowViewModel();
            InitializeComponent();
            var modeSelectorPage = new modeSelectorPage();
            modeSelectorPage.Tag = this.DataContext;
            mainFrame.Navigate(modeSelectorPage);
        }
    }
}