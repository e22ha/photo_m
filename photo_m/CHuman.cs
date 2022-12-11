using System;

namespace photo_m;

public abstract class Human {
    public Guid? id { get; set; }
    public string? name { get; set; } 
    public string? surname { get; set; } 
    public string? full_name { get; set; }
}