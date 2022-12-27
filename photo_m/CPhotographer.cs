namespace photo_m;

public class Photographer : Human{
    public Camera? camera { get; set; }
    public string? nick { get; set; }
    public Photo? photos { get; }
}