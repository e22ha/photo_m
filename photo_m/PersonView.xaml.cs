using System;
using System.Windows;
using System.Windows.Controls;
using EdgeDB;

namespace photo_m;

public partial class PersonView : Window
{
    
    private String result;
    public EdgeDBClient _client = new();
    public PersonView()
    {
        InitializeComponent();
        NormalQuery();
    }



    async void NormalQuery()
    {
        foreach (var ph in await _client.QueryAsync<Tuple<string, long>>(
                     "SELECT (Person.full_name, count(Person.photos));"))
        {
            ListBoxItem itm = new()
            {
                Content = ph.Item1 + "------------" + ph.Item2
            };
            ListOfPerson.Items.Add(itm);
        }

    }

}

