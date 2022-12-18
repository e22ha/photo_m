using System;
using photo_m;

namespace photo_m
{
    public class Photo
    {
        public Guid? id { get; set; }
        public string? full_path
        {
            set
            {
                full_path = value;
            }
            get
            {
                return this.full_path; // = full_path.Replace(@"\",@"/");
            }
        }

        public long? Rating;
        
        public Event? event_;

        public Photographer? author;

        public Person? face;

    }

    public class Event
    {
        public Guid? id { get; set; }
        public string? title;
        public DateTimeOffset? date;
    }
}