using System;
using photo_m;

namespace photo_m
{
    public class Photo
    {
        public Guid? id { get; set; }
        
        public string? name { get; set; }
        public string? directory { get; set; }
        public string? full_path { get; set; }
        
        public Photographer? author { get; set; }

        public Person[]? face { get; set; }
        public long? rating { get; set; }
        public Event? @event { get; set; }
        public Camera? camera { get; set; }
    }
}