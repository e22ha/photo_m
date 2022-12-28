using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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

        ShowDir();
    }

    private void Back(object sender, RoutedEventArgs e)
    {
        PathTextBox.Text = Path.GetFullPath(Path.Combine(PathTextBox.Text, @"..\"));
        ShowDir();
    }

    private void f_mode(object sender, RoutedEventArgs e)
    {
        PhotographsWin pWin = new PhotographsWin();
        pWin.Show();
    }

    private void e_mode(object sender, RoutedEventArgs e)
    {
        addPerson aP = new addPerson("default");
        aP.Show();
    }

    private void p_mode(object sender, RoutedEventArgs e)
    {
        PersonView pWin = new PersonView();
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

    private string?[] _baseInfo =
    {
        "", "", "", "", "", ""
    };

    private async void ShowInfoPhoto(string? path)
    {
        ClearBox();
        var p = ClearPath(path);
        //foreach (var ph in await _client.QueryAsync<Photo>("Select (Select Photo {id}) filter .full_path = .full_path limit 1;"))
        //{
        //Author_box.Text = "select (Select Photo filter .full_path = " + p +" limit 1).author.full_name;";
        //Author_box.Text = "Select Photo filter .full_path = " + path +" limit 1;";
        var GetInfoAboutPhoto =
            $@"select Photo {{ author: {{nick}}, camera: {{name}}, rating, event: {{title,date}}, face: {{full_name}} }} filter .full_path = '{p}';";

        foreach (var photo in await _client.QueryAsync<Photo>(GetInfoAboutPhoto))
        {
            if (photo.author?.nick != null) Author_box.Text = photo.author.nick;
            if(photo.camera != null) Camera_box.Text = photo.camera.name;
            if (photo.rating != null) Rate_box.Text = photo.rating.ToString();
            if(photo.@event != null)
            {
                Event_box.Text = photo.@event.title;
                Date_box.Text = DateTime.Parse(photo.@event.date.ToString()).ToString("dd.MM.yyyy");
            }
            
            foreach (var f in photo.face)
            {
                Face_box.Text += f.full_name + "; ";
            }
        }

        _baseInfo[0] = Author_box.Text;
        _baseInfo[1] = Camera_box.Text;
        _baseInfo[2] = Rate_box.Text;
        _baseInfo[3] = Event_box.Text;
        _baseInfo[4] = Date_box.Text;
        _baseInfo[5] = Face_box.Text;
    }

    private static string? ClearPath(string? path)
    {
        return path?.Replace(@"\", @"\\");
    }


    private void ClearBox()
    {
        Author_box.Text = "";
        Camera_box.Text = "";
        Rate_box.Text = "";
        Event_box.Text = "";
        Date_box.Text = "";
        Face_box.Text = "";
    }

    private void Cancel_click(object sender, RoutedEventArgs e)
    {
        Author_box.Text = _baseInfo[0];
        Camera_box.Text = _baseInfo[1];
        Rate_box.Text = _baseInfo[2];
        Event_box.Text = _baseInfo[3];
        Date_box.Text = _baseInfo[4];
        Face_box.Text = _baseInfo[5];
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

        var rating = Rate_box.Text;
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

        var nameF = l.Content.ToString()?.Replace("📁", "");


        var EventQurey = "";
        var InsertEventQurey = "";


        if(Event_box.Text == "") Event_box.Text = "Default Event";
        if(Date_box.Text == "") Date_box.Text = "22.12.2022";

        var eventText = Event_box.Text;
        var eventDateText = Date_box.Text;
        const string format = "yyyy-MM-ddTHH:mm:ssZ";
        var date = DateTime.ParseExact(eventDateText, "dd.MM.yyyy", CultureInfo.InvariantCulture);
        var ClearEventDateText = date.ToString(format);
        EventQurey = $" _E_n := '{eventText}', _E_d :=  <datetime>'{ClearEventDateText}', ";
        InsertEventQurey =
            "_E := (insert Event {title:= _E_n, date := _E_d }unless conflict on (.title, .date) else (select Event) ), ";

        var CameraQurey = "";
        if (Camera_box.Text != "")
        {
            var c = Camera_box.Text.Split(" ");
            CameraQurey =
                $" _C := (insert Camera {{brand:= '{c[0]}', model := '{c[1]}', }}unless conflict on (.brand, .model) else (select Camera)),";
        }
        

        var faceQuery = "";
        var countFaceQuery = "_Ps := { ";

        if (Face_box.Text != "")
        {
            var faces = Face_box.Text.Split("; ");
            List<Face> listFace = new List<Face>();

            for (var index = 0; index < faces.Length - 1; index++)
            {
                var f = faces[index].Split(" ");
                listFace.Add(new Face { Name = f[0], Surname = f[1] });
            }


            faceQuery = "";

            for (var i = 0; i < listFace.Count; i++)
            {
                faceQuery += $"_Pn_{i} := '{listFace[i].Name}', _Ps_{i} := '{listFace[i].Surname}', ";
                faceQuery +=
                    $"_P{i} := (insert Person {{name:= _Pn_{i}, surname := _Ps_{i} }}unless conflict on (.name, .surname) else (select Person) ), ";
                countFaceQuery += $"_P{i},";
            }

            countFaceQuery = countFaceQuery.Remove(countFaceQuery.LastIndexOf(",", StringComparison.Ordinal));
        }

        countFaceQuery += "} ";

        var bigQuery =
            $"with _n := '{nameF}', _d := '{ClearPath(path)}', _r := {rating}, " +
            CameraQurey +
            $"_A := (insert Photographer {{name:= '{authorName}', surname := '{authorSurname}', nick := '{authorNick}', }}unless conflict on .nick else (select Photographer))," +
            EventQurey +
            InsertEventQurey +
            faceQuery + countFaceQuery +
            "Insert Photo {name:= _n, directory := _d, rating := _r, author := _A, event := _E, camera := _C, face := _Ps } unless conflict on (.directory, .name)else (update Photo filter .full_path = (select(_d ++ _n))set {rating:= _r, author := _A, event := _E, camera := _C, face := _Ps } );";

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