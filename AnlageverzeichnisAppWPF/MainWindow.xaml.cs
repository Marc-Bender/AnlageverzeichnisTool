using CommunityToolkit.Mvvm.ComponentModel;
using System.Net.Http;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            this.DataContext = new MainWindowViewModel();
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            NavigationCommands.BrowseHome.InputGestures.Clear();
            NavigationCommands.BrowseStop.InputGestures.Clear();
            NavigationCommands.Favorites.InputGestures.Clear();
            NavigationCommands.FirstPage.InputGestures.Clear();
            NavigationCommands.GoToPage.InputGestures.Clear();
            NavigationCommands.LastPage.InputGestures.Clear();
            NavigationCommands.NavigateJournal.InputGestures.Clear();
            NavigationCommands.NextPage.InputGestures.Clear();
            NavigationCommands.PreviousPage.InputGestures.Clear();
            NavigationCommands.Search.InputGestures.Clear();
            NavigationCommands.Refresh.InputGestures.Clear(); // to free up F5 and alike for my own usage

            InitializeComponent();

            var modeSelectorPage = new modeSelectorPage();
            modeSelectorPage.Tag = this.DataContext;
            mainFrame.Navigate(modeSelectorPage);
        }
    }
}