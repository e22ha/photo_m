using System.Windows;
using EdgeDB;

namespace photo_m;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly EdgeDBClient _client = new();
        public MainWindow()
        {
            InitializeComponent();
            status.Content = "Hello\n";

        }

        private void Go(object sender, RoutedEventArgs e)
        {
            Query();
        }

        private async void Query()
        {
            status.FontSize = 18;
            status.Content += await _client.QuerySingleAsync<string>("SELECT 1+1") + "\n";
        }

        private void Cls(object sender, RoutedEventArgs e)
        {
            status.FontSize = 32;
            status.Content = "Hello\n";
        }
    }
