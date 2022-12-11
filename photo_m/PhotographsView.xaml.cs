using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using EdgeDB;


namespace photo_m;


public partial class Photographs : Window
{
    private String result;
    public EdgeDBClient _client = new();
    public Photographs()
    {
        InitializeComponent();
        var i = 0;
        //Query();
        NormalQuery();

    }



    async void NormalQuery()
    {
        foreach (var ph in await _client.QueryAsync<Tuple<string, long>>(
                     "SELECT (Photographer.full_name, (select count(Photographer.<author[is Photo])));"))
        {
            ListBoxItem itm = new()
            {
                Content = ph.Item1 + "------------" + ph.Item2
            };
            ListOfPhotographs.Items.Add(itm);
        }

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
            ListOfPhotographs.Items.Add(itm);
        }

    }
}

