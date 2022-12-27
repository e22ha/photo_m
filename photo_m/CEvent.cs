using System;

namespace photo_m;

public class Event
{
    public Guid? id { get; set; }
    public string? title;
    public DateTimeOffset? date;
}