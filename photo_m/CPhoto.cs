using System;
using photo_m;

namespace photo_m
{
    public abstract class Photo
    {
        public Guid? id { get; set; }
        public string full_path
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

        public int? Rating;
      

        public Event event_;

        public Photographer author;

        public Human face;

    }

    public class Event
    {
        public Guid? id { get; set; }

        public string title;
    }
}