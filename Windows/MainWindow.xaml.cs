using GoogleMessage.ViewModels;
using System.Windows;

namespace GoogleMessage.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {            
            InitializeComponent();    
            this.DataContext = viewModel;
        }
    }
}
