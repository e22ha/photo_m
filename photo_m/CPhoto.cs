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
        
        public Photographer? author;

        public Person? face;
        public long? Rating { get; set; }
        public Event? event_;
    }
}