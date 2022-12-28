using System;

namespace photo_m;

public class Event
{
    public Guid? id { get; set; }
    public string? title { get; set; }
    public DateTimeOffset? date { get; set; }
}