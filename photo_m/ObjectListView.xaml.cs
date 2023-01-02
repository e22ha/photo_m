using System;
using System.Windows.Controls;
using EdgeDB;

namespace photo_m;

public partial class ObjectListView
{
    private readonly EdgeDBClient _client = new();

    private const string PersonQuery = "SELECT (Person.full_name, count(Person.photos));";
    private const string PhQuery = "SELECT (Photographer.full_name, count(Photographer.photos));";
    private const string EventQuery = "SELECT (Event.title, count(Photo filter Photo.event.title = Event.title));";
    public ObjectListView(string type)
    {
        InitializeComponent();
        switch (type)
        {
            case "p":
                Label.Content = "Люди";
                Query(PersonQuery);
                break;
            case "ph":
                Label.Content = "Фотографы";
                Query(PhQuery);
                break;
            case "e":
                Label.Content = "События";
                Query(EventQuery);
                break;
        }
    }



    private async void Query(string query)
    {
        foreach (var tuple in await _client.QueryAsync<Tuple<string, long>>(query))
        {
            if (tuple == null) continue;
            ListBoxItem itm = new()
            {
                Content = tuple.Item1 + "------------" + tuple.Item2
            };
            ListOfPerson.Items.Add(itm);
        }
    }

}

