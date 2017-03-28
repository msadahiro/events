using System;

namespace events.Models
{
    public class Event
    {
        public int id { get; set; }
        public string Title { get; set; }
        public DateTime EventDate {get;set;}
        public string Length {get;set;}

        public DateTime CreatedAt {get;set;}

        public DateTime UpdatedAt {get;set;}
        public Event (){
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}