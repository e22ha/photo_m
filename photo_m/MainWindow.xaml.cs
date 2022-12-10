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
        }

        private void Go(object sender, RoutedEventArgs e)
        {
            Query();
        }

        private async void Query()
        {
            status.FontSize = 18;
            status.Text += await _client.QuerySingleAsync<string>("SELECT 1+1") + "\n";
        }

        private void Cls(object sender, RoutedEventArgs e)
        {
            status.FontSize = 32;
            status.Text = "Hello\n";
        }

        private void GoTo(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void f_mode(object sender, RoutedEventArgs e)
        {
            Photographs p_win = new Photographs();
            p_win.Show();
        }

        private void e_mode(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void p_mode(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
