using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using EdgeDB;


namespace photo_m;


public partial class PhotographsWin : Window
{
    private String result;
    public EdgeDBClient _client = new();
    public PhotographsWin()
    {
        InitializeComponent();
        var i = 0;
        NormalQuery();

    }



    async void NormalQuery()
    {
        foreach (var ph in await _client.QueryAsync<Tuple<string, long>>(
                     "SELECT (Photographer.full_name, count(Photographer.photos));"))
        {
            ListBoxItem itm = new()
            {
                Content = ph.Item1 + "------------" + ph.Item2
            };
            ListOfPhotographs.Items.Add(itm);
        }

    }
}

