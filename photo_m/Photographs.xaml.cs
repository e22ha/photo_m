using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using EdgeDB;


namespace photo_m;


public partial class Photographs : Window
{
    private readonly EdgeDBClient _client = new();
    private String result;
    public Photographs()
    {
        InitializeComponent();
        var i = 0;
        Query();

    }

    async void Query()
    {
        foreach (var ph in await _client.QueryAsync<Photographer>("SELECT Photographer {id, full_name};"))
        {

            var count = await _client.QuerySingleAsync<int>($"SELECT count_p_by_author((select <uuid>'{ph.id}'));");
            ListBoxItem itm = new()
            {
                Content = ph.full_name + " " + count
            };
            list_of_photographs.Items.Add(itm);
        }

    }
}