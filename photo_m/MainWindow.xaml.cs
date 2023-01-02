using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
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
        var pWin = new ObjectListView("ph");
        pWin.Show();
    }

    private void e_mode(object sender, RoutedEventArgs e)
    {
        var pWin = new ObjectListView("e");
        pWin.Show();
    }

    private void p_mode(object sender, RoutedEventArgs e)
    {
        var pWin = new ObjectListView("p");
        pWin.Show();
    }

    private void Find(object sender, RoutedEventArgs e)
    {
        ShowDir();
    }

    private async void ShowDir()
    {
        if (!PathTextBox.Text.EndsWith(@"\")) PathTextBox.Text += @"\";
        ListOfFiles.SelectedIndex = -1;
        ListOfFiles.Items.Clear();
        var directoryInfo = new DirectoryInfo(PathTextBox.Text);

        foreach (var file in directoryInfo.GetDirectories())
        {
            ListOfFiles.Items.Add(new ListBoxItem { Content = "📁" + file.Name });
        }

        foreach (var file in directoryInfo.GetFiles())
        {
            if (Path.GetExtension(file.FullName) == ".jpg" || Path.GetExtension(file.FullName) == ".png")
            {
                ListOfFiles.Items.Add(new ListBoxItem { Content = file.Name });
            }
        }
        
        await AskCountPhoto();
    }

    private void List_of_files_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ListOfFiles.SelectedIndex <= -1) return;
        ActionForDirOrImg();
    }

    private async void ActionForDirOrImg()
    {
        var l = (ListBoxItem)ListOfFiles.Items[ListOfFiles.SelectedIndex];
        var path = PathTextBox.Text + l.Content.ToString()?.Replace("📁", "");
        if (IsDir(path))
        {
            PathTextBox.Text = path;
            ShowDir();
        }
        else
        {
            ImageV.Source = new BitmapImage(new Uri(path));
            await ShowInfoPhoto(path);
        }
    }

    private readonly string?[] _baseInfo =
    {
        "", "", "", "", "", ""
    };

    private async Task ShowInfoPhoto(string? path)
    {
        ClearBox();
        var p = ClearPath(path);
        //foreach (var ph in await _client.QueryAsync<Photo>("Select (Select Photo {id}) filter .full_path = .full_path limit 1;"))
        //{
        //Author_box.Text = "select (Select Photo filter .full_path = " + p +" limit 1).author.full_name;";
        //Author_box.Text = "Select Photo filter .full_path = " + path +" limit 1;";
        var getInfoAboutPhoto =
            $@"select Photo {{ author: {{nick}}, camera: {{name}}, rating, event: {{title,date}}, face: {{full_name}} }} filter .full_path = '{p}';";

        foreach (var photo in await _client.QueryAsync<Photo>(getInfoAboutPhoto))
        {
            if (photo == null) continue;
            if (photo.author?.nick != null) AuthorBox.Text = photo.author.nick;
            if(photo.camera != null) CameraBox.Text = photo.camera.name;
            if (photo.rating != null) RateBox.Text = photo.rating.ToString();
            if(photo.@event != null)
            {
                EventBox.Text = photo.@event.title;
                DateBox.Text = DateTime.Parse(photo.@event.date.ToString() ?? string.Empty).ToString("dd.MM.yyyy");
            }

            if (photo.face != null)
                foreach (var f in photo.face)
                {
                    FaceBox.Text += f.full_name + "; ";
                }
        }

        _baseInfo[0] = AuthorBox.Text;
        _baseInfo[1] = CameraBox.Text;
        _baseInfo[2] = RateBox.Text;
        _baseInfo[3] = EventBox.Text;
        _baseInfo[4] = DateBox.Text;
        _baseInfo[5] = FaceBox.Text;
    }

    private static string? ClearPath(string? path)
    {
        return path?.Replace(@"\", @"\\");
    }


    private void ClearBox()
    {
        AuthorBox.Text = "";
        CameraBox.Text = "";
        RateBox.Text = "";
        EventBox.Text = "";
        DateBox.Text = "";
        FaceBox.Text = "";
    }

    private void Cancel_click(object sender, RoutedEventArgs e)
    {
        AuthorBox.Text = _baseInfo[0];
        CameraBox.Text = _baseInfo[1];
        RateBox.Text = _baseInfo[2];
        EventBox.Text = _baseInfo[3];
        DateBox.Text = _baseInfo[4];
        FaceBox.Text = _baseInfo[5];
    }

    private struct Face
    {
        public string Name { get; init; }
        public string Surname { get; init; }
    }


    private async void Ok_click(object sender, RoutedEventArgs e)
    {
        var l = (ListBoxItem)ListOfFiles.Items[ListOfFiles.SelectedIndex];
        await AddInfo(l);
        await AskCountPhoto();
    }

    private async Task AddInfo(ListBoxItem l)
    {
        var path = PathTextBox.Text;

        var rating = RateBox.Text;
        if (rating == "") rating = "0";

        var authorNick = AuthorBox.Text;
        if (authorNick == "") authorNick = "default";

        var authorName = "";
        var authorSurname = "";

        if (await FindPhByNick(authorNick))
        {
            var ph = await _client.QuerySingleAsync<Photographer?>(
                $"select Photographer {{name, surname}} filter .nick = '{authorNick}' limit 1;");
            authorName = ph?.name;
            authorSurname = ph?.surname;
        }
        else
        {
            var aP = new addPerson(authorNick);
            aP.ShowDialog();
            if (aP.DialogResult == true)
            {
                authorName = aP.name;
                authorSurname = aP.surname;
            }
        }

        var nameF = l.Content.ToString()?.Replace("📁", "");


        if (EventBox.Text == "") EventBox.Text = "Default Event";
        if (DateBox.Text == "") DateBox.Text = "22.12.2022";

        var eventText = EventBox.Text;
        var eventDateText = DateBox.Text;
        const string format = "yyyy-MM-ddTHH:mm:ssZ";
        var date = DateTime.ParseExact(eventDateText, "dd.MM.yyyy", CultureInfo.InvariantCulture);
        var clearEventDateText = date.ToString(format);
        var eventQuery = $" _E_n := '{eventText}', _E_d :=  <datetime>'{clearEventDateText}', ";
        var insertEventQuery =
            "_E := (insert Event {title:= _E_n, date := _E_d }unless conflict on (.title, .date) else (select Event) ), ";

        string cameraQuery;
        if (CameraBox.Text != "")
        {
            var c = CameraBox.Text.Split(" ");
            cameraQuery =
                $" _C := (insert Camera {{brand:= '{c[0]}', model := '{c[1]}', }}unless conflict on (.brand, .model) else (select Camera)),";
        }
        else
        {
            cameraQuery =
                " _C := (insert Camera {brand:= 'default', model := 'camera', }unless conflict on (.brand, .model) else (select Camera)),";
        }


        var faceQuery = "";
        var countFaceQuery = "{ ";

        if (FaceBox.Text != "")
        {
            if (FaceBox.Text.EndsWith(";"))
            {
                FaceBox.Text += " ";
            }

            if (!FaceBox.Text.EndsWith("; "))
            {
                FaceBox.Text += "; ";
            }

            var faces = FaceBox.Text.Split("; ");
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
            cameraQuery +
            $"_A := (insert Photographer {{name:= '{authorName}', surname := '{authorSurname}', nick := '{authorNick}', }}unless conflict on .nick else (select Photographer))," +
            eventQuery +
            insertEventQuery +
            faceQuery +
            $"Insert Photo {{name:= _n, directory := _d, rating := _r, author := _A, event := _E, camera := _C, face := {countFaceQuery} }} unless conflict on (.directory, .name)else (update Photo filter .full_path = (select(_d ++ _n))set {{rating:= _r, author := _A, event := _E, camera := _C, face := {countFaceQuery} }} );";

        // var res = ShowQuery(bigQuery);
        // if (res)
        // {
        await AskDb(bigQuery);
        // }
        await ShowInfoPhoto(PathTextBox.Text + l.Content.ToString()?.Replace("📁", ""));
        await AskCountPhoto();
    }

    private async Task<bool> FindPhByNick(string authorNick)
    {
        return (await _client.QueryAsync<string>("select( select Photographer {nick}).nick;")).Any(ph => ph == authorNick);
    }

    private async Task AskDb(string bigQuery)
    {
        await _client.QueryAsync<Photo>(bigQuery);
    }

    private async Task AskCountPhoto()
    {
        LAllIndexImg.Content = await _client.QuerySingleAsync<string>("select count(Photo);");
        LCountIndexImg.Content = await _client.QuerySingleAsync<string>($"select count(Photo filter .directory = '{ClearPath(PathTextBox.Text)}');");
        LCountImg.Content = CountImgInDir();
    }

    private string CountImgInDir()
    {
        int count =0;
        if (!PathTextBox.Text.EndsWith(@"\")) PathTextBox.Text += @"\";
        var directoryInfo = new DirectoryInfo(PathTextBox.Text);
        foreach (var file in directoryInfo.GetFiles())
        {
            if (Path.GetExtension(file.FullName) == ".jpg" || Path.GetExtension(file.FullName) == ".png")
            {
                count++;
            }
        }
        
        return count.ToString();
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

    private async void IndexAllInDir(object sender, RoutedEventArgs e)
    {
        foreach (var t in ListOfFiles.Items)
        {
            var l = (ListBoxItem)t;
            if (l.Content.ToString().StartsWith("📁"))
            {
                Debug.Write("folder");
            }
            else if(l.Content.ToString().EndsWith(".png") || l.Content.ToString().EndsWith(".jpg"))
            {
                await AddInfo(l);
            }
        }

        await AskCountPhoto();
    }
}