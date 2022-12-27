using System;

namespace photo_m;

public class Camera
{
    public Guid? id { get; set; }
    public string? brand { get; set; }
    public string? model { get; set; }
    public string? name { get; set; }

    public Photographer? photographers;
}