using System;
using System.IO;
using System.Threading.Tasks;
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
        addPerson aP = new addPerson("lol");
        aP.Show();
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
        var p = ClearPath(path);
        //foreach (var ph in await _client.QueryAsync<Photo>("Select (Select Photo {id}) filter .full_path = .full_path limit 1;"))
        //{
        //Author_box.Text = "select (Select Photo filter .full_path = " + p +" limit 1).author.full_name;";
        //Author_box.Text = "Select Photo filter .full_path = " + path +" limit 1;";
        var authorFullName = $"select (select Photo filter .full_path = '{p}' limit 1).author.nick;";
        var rating = $"select (select Photo filter .full_path = '{p}' limit 1).rating;";
        var _event = $"select (select (select Photo filter .full_path = '{p}' limit 1).event).title;";
        var face = $"select (select (select Photo filter .full_path = '{p}' limit 1).face).full_name;";

        
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

    private static string ClearPath(string path)
    {
        return (path?.Replace(@"\", @"/"));

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
    private async void Ok_click(object sender, RoutedEventArgs e)
    {
        var l = (ListBoxItem)list_of_files.Items[list_of_files.SelectedIndex];
        var path = PathTextBox.Text;

        var rating = 0;
        var author_nick = Author_box.Text;
        
        var Author_name = "";
        var Author_surname = "";

        if (await FindPhByNick(author_nick))
        {
            var ph = await _client.QuerySingleAsync<Photographer?>(
                $"select Photographer {{name, surname}} filter .nick = '{author_nick}' limit 1;");
            Author_name = ph.name;
            Author_surname = ph.surname;
            MessageBox.Show(Author_surname.ToString());
        }
        else
        {
            addPerson aP = new addPerson(author_nick);
            aP.ShowDialog();
            if (aP.DialogResult == true)
            {
                // Окно было закрыто с помощью кнопки "ОК"
                Author_name = aP.name; 
                Author_surname = aP.surname;
            }
        }
        
        
        var Event = Event_box.Text;
        var namef = l.Content.ToString()?.Replace("📁", "");
        
        
        var bigQurey =
            $"with _n := '{namef}', _d := '{ClearPath(path)}', _r := {rating}, " + 
            $"_A_n :='{Author_name}', _A_s := '{Author_surname}', _A_nick := '{author_nick}', " + "_A := (insert Photographer {name:= _A_n, surname := _A_s, nick := _A_nick, }unless conflict on .nick else (select Photographer))," + 
            $" _E_n := '{Event}', _E_d :=  <datetime>'{DateTime.Now:yyyy-MM-ddTHH:mm:ssZ}', " + 
            "_E := (insert Event {title:= _E_n, date := _E_d }unless conflict on (.title, .date) else (select Event) ), " + 
            "_P_n := 'Pavel', _P_s := 'Solomatov', _P1_n := 'G', _P1_s := 'Leb', _P := (insert Person {name:= _P_n, surname := _P_s }unless conflict on (.name, .surname) else (select Person) ), _P1 := (insert Person {name:= _P1_n, surname := _P1_s }unless conflict on (.name, .surname) else (select Person) ), _Ps := {_P,_P1} Insert Photo {name:= _n, directory := _d, rating := _r, author := _A, event := _E, face := _Ps } unless conflict on (.directory, .name)else (update Photo filter .full_path = (select(_d ++ _n))set {rating:= _r, author := _A, event := _E, face := _Ps } );";
        
        var res = ShowQurey(bigQurey);
        if (res)
        {
            askDb(bigQurey);
            MessageBox.Show("Yes");
        }
        
    }

    private async Task<bool> FindPhByNick(string author_nick)
    {
        foreach (var ph in await _client.QueryAsync<string>("select( select Photographer {nick}).nick;"))
        {
            if (ph == author_nick)
            {
                return true;
            }
        }

        return false;
    }

    private async Task askDb(string bigQurey)
    {
        await _client.QueryAsync<Photo>(bigQurey);
    }


    private static bool IsDir(string path)
    {
        return Directory.Exists(path);
    }

    private bool ShowQurey(string q)
    {
        var res = MessageBox.Show(q, "caption", MessageBoxButton.YesNo);
        return res == MessageBoxResult.Yes;
    }

}