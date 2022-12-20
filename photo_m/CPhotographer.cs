using System;

namespace photo_m;

public class Photographer : Human{
    public Camera? camera { get; set; }
    public string? nick { get; set; }
    public Photo? photos { get; }
}

public class Camera
{
    public Guid? id { get; set; }
    public string? brand { get; set; } 
    public string? model { get; set; } 
    public string? name { get; set; }
}