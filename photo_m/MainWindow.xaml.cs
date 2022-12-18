using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using EdgeDB;

namespace photo_m;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly EdgeDBClient _client;

    public MainWindow()
    {
        InitializeComponent();
        Log.Content = "e";
        EdgeDBClientPoolConfig config = new()
        {
            ConnectionTimeout = 5000u
        };
        _client = new EdgeDBClient(config);
    }

    private void Back(object sender, RoutedEventArgs e)
    {
        PathTextBox.Text = Path.GetFullPath(Path.Combine(PathTextBox.Text, @"..\"));
        ShowDir();
    }

    private void f_mode(object sender, RoutedEventArgs e)
    {
        Photographs pWin = new Photographs();
        pWin.Show();
    }

    private void e_mode(object sender, RoutedEventArgs e)
    {
        Photographs pWin = new Photographs();
        pWin.Show();
    }

    private void p_mode(object sender, RoutedEventArgs e)
    {
        Photographs pWin = new Photographs();
        pWin.Show();
    }

    private void Find(object sender, RoutedEventArgs e)
    {
        ShowDir();
    }

    private void ShowDir()
    {
        if (!PathTextBox.Text.EndsWith(@"\")) PathTextBox.Text += @"\";
        list_of_files.SelectedIndex = -1;
        list_of_files.Items.Clear();
        var directoryInfo = new DirectoryInfo(PathTextBox.Text);

        foreach (var file in directoryInfo.GetDirectories())
        {
            list_of_files.Items.Add(new ListBoxItem { Content = "📁" + file.Name });
        }

        foreach (var file in directoryInfo.GetFiles())
        {
            if (Path.GetExtension(file.FullName) == ".jpg" || Path.GetExtension(file.FullName) == ".png")
            {
                list_of_files.Items.Add(new ListBoxItem { Content = file.Name });
            }
        }
    }

    private void List_of_files_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (list_of_files.SelectedIndex > -1)
        {
            ListBoxItem l = (ListBoxItem)list_of_files.Items[list_of_files.SelectedIndex];
            var path = PathTextBox.Text + l.Content.ToString()?.Replace("📁", "");
            Log.Content = path;
            if (IsDir(path))
            {
                PathTextBox.Text = path;
                ShowDir();
            }
            else
            {
                ImageV.Source = new BitmapImage(new Uri(path));
                ShowInfoPhoto(path);
            }
        }
    }

    private string[] baceInfo = new[]
    {
        "", "", "", ""
    };

    private async void ShowInfoPhoto(string path)
    {
        ClearBox();
        string p = "'" + path?.Replace(@"\", @"/") + "'";
        //foreach (var ph in await _client.QueryAsync<Photo>("Select (Select Photo {id}) filter .full_path = .full_path limit 1;"))
        //{
        //Author_box.Text = "select (Select Photo filter .full_path = " + p +" limit 1).author.full_name;";
        //Author_box.Text = "Select Photo filter .full_path = " + path +" limit 1;";
        var authorFullName = $"select (select Photo filter .full_path = {p} limit 1).author.full_name;";
        var rating = $"select (select Photo filter .full_path = {p} limit 1).rating;";
        var _event = $"select (select (select Photo filter .full_path = {p} limit 1).event).title;";
        var face = $"select (select (select Photo filter .full_path = {p} limit 1).face).full_name;";

        
        Author_box.Text = await _client.QuerySingleAsync<string>(authorFullName);
        baceInfo[0] = Author_box.Text;
        Rate_box.Text = (await _client.QuerySingleAsync<int>(rating)).ToString();
        baceInfo[1] = Rate_box.Text;
        Event_box.Text = (await _client.QuerySingleAsync<string>(_event));
        baceInfo[2] = Event_box.Text;

        foreach (var pers in await _client.QueryAsync<string>(face))
        {
            Face_box.Text += pers.ToString() + "; ";
        }

        baceInfo[3] = Face_box.Text;
    }


    private void ClearBox()
    {
        Author_box.Text = "";
        Rate_box.Text = "";
        Event_box.Text = "";
        Face_box.Text = "";
    }

    private void Cancel_click(object sender, RoutedEventArgs e)
    {
        Author_box.Text = baceInfo[0];
        Rate_box.Text = baceInfo[1];
        Event_box.Text = baceInfo[2];
        Face_box.Text = baceInfo[3];
    }
    private void Ok_click(object sender, RoutedEventArgs e)
    {
        
        MessageBox.Show("Sucses");
    }
    
    private static bool IsDir(string path)
    {
        return Directory.Exists(path);
    }

}