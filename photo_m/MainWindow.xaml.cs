using System;
using System.Collections.Generic;
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

    private string?[] _baseInfo =
    {
        "", "", "", ""
    };

    private async void ShowInfoPhoto(string? path)
    {
        ClearBox();
        var p = ClearPath(path);
        //foreach (var ph in await _client.QueryAsync<Photo>("Select (Select Photo {id}) filter .full_path = .full_path limit 1;"))
        //{
        //Author_box.Text = "select (Select Photo filter .full_path = " + p +" limit 1).author.full_name;";
        //Author_box.Text = "Select Photo filter .full_path = " + path +" limit 1;";
        var authorFullName = $"select (select Photo filter .full_path = '{p}' limit 1).author.nick;";
        var rating = $"select (select Photo filter .full_path = '{p}' limit 1).rating;";
        var titleEvent = $"select (select (select Photo filter .full_path = '{p}' limit 1).event).title;";
        var face = $"select (select (select Photo filter .full_path = '{p}' limit 1).face).full_name;";


        Author_box.Text = await _client.QuerySingleAsync<string>(authorFullName);
        _baseInfo[0] = Author_box.Text;
        Rate_box.Text = (await _client.QuerySingleAsync<int>(rating)).ToString();
        _baseInfo[1] = Rate_box.Text;
        Event_box.Text = (await _client.QuerySingleAsync<string>(titleEvent));
        _baseInfo[2] = Event_box.Text;

        foreach (var person in await _client.QueryAsync<string>(face))
        {
            Face_box.Text += person + "; ";
        }

        _baseInfo[3] = Face_box.Text;
    }

    private static string? ClearPath(string? path)
    {
        return path?.Replace(@"\", @"/");
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
        Author_box.Text = _baseInfo[0];
        Rate_box.Text = _baseInfo[1];
        Event_box.Text = _baseInfo[2];
        Face_box.Text = _baseInfo[3];
    }

    struct Face
    {
        public string Name { get; set; }
        public string Surname { get; set; }
    }


    private async void Ok_click(object sender, RoutedEventArgs e)
    {
        var l = (ListBoxItem)list_of_files.Items[list_of_files.SelectedIndex];
        var path = PathTextBox.Text;

        var rating = 0;
        var authorNick = Author_box.Text;

        var authorName = "";
        var authorSurname = "";

        if (await FindPhByNick(authorNick))
        {
            var ph = await _client.QuerySingleAsync<Photographer?>(
                $"select Photographer {{name, surname}} filter .nick = '{authorNick}' limit 1;");
            authorName = ph.name;
            authorSurname = ph.surname;
        }
        else
        {
            addPerson aP = new addPerson(authorNick);
            aP.ShowDialog();
            if (aP.DialogResult == true)
            {
                authorName = aP.name;
                authorSurname = aP.surname;
            }
        }


        var eventText = Event_box.Text;
        var nameF = l.Content.ToString()?.Replace("📁", "");

        var faces = Face_box.Text.Split("; ");
        List<Face> listFace = new List<Face>();
        for (var index = 0; index < faces.Length-1; index++)
        {
            var f = faces[index].Split(" ");
            listFace.Add(new Face { Name = f[0], Surname = f[1]});
        }

        
        var faceQuery = "";
        var countFaceQuery = "_Ps := { ";

        for (var i = 0; i < listFace.Count; i++)
        {
            faceQuery += $"_Pn_{i} := '{listFace[i].Name}', _Ps_{i} := '{listFace[i].Surname}', ";
            faceQuery += $"_P{i} := (insert Person {{name:= _Pn_{i}, surname := _Ps_{i} }}unless conflict on (.name, .surname) else (select Person) ), ";
            countFaceQuery += $"_P{i},";
        }

        countFaceQuery = countFaceQuery.Remove(countFaceQuery.LastIndexOf(",", StringComparison.Ordinal));
        countFaceQuery += "} ";
        
        var bigQuery =
            $"with _n := '{nameF}', _d := '{ClearPath(path)}', _r := {rating}, " +
            $"_A_n :='{authorName}', _A_s := '{authorSurname}', _A_nick := '{authorNick}', " +
            "_A := (insert Photographer {name:= _A_n, surname := _A_s, nick := _A_nick, }unless conflict on .nick else (select Photographer))," +
            $" _E_n := '{eventText}', _E_d :=  <datetime>'{DateTime.Today:yyyy-MM-ddTHH:mm:ssZ}', " +
            "_E := (insert Event {title:= _E_n, date := _E_d }unless conflict on (.title, .date) else (select Event) ), " +
            faceQuery + countFaceQuery +
            "Insert Photo {name:= _n, directory := _d, rating := _r, author := _A, event := _E, face := _Ps } unless conflict on (.directory, .name)else (update Photo filter .full_path = (select(_d ++ _n))set {rating:= _r, author := _A, event := _E, face := _Ps } );";

        var res = ShowQuery(bigQuery);
        if (res)
        {
            await AskDb(bigQuery);
        }
    }

    private async Task<bool> FindPhByNick(string authorNick)
    {
        foreach (var ph in await _client.QueryAsync<string>("select( select Photographer {nick}).nick;"))
        {
            if (ph == authorNick)
            {
                return true;
            }
        }

        return false;
    }

    private async Task AskDb(string bigQuery)
    {
        await _client.QueryAsync<Photo>(bigQuery);
    }


    private static bool IsDir(string? path)
    {
        return Directory.Exists(path);
    }

    private static bool ShowQuery(string q)
    {
        var res = MessageBox.Show(q, "caption", MessageBoxButton.YesNo);
        return res == MessageBoxResult.Yes;
    }
}