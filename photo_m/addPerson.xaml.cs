using System.Windows;

namespace photo_m;

public partial class addPerson : Window
{
    public string _nick = "";
    public string name = "";
    public string surname = "";

    public addPerson(string nick)
    {
        InitializeComponent();
        _nick = nick;
        NickBox.Text = _nick;
    }

    private void Click_yes(object sender, RoutedEventArgs e)
    {
        _nick = NickBox.Text;
        name = NameBox.Text;
        surname = SurnameBox.Text;
        DialogResult = true;
        Close();
    }
}