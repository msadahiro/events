using System;
using System.Collections.Generic;

namespace events.Models
{
    public class Event
    {
        public int id { get; set; }
        public string Title { get; set; }
        public DateTime EventDate {get;set;}
        public int Duration {get;set;}
        public string Length {get;set;}

        public DateTime CreatedAt {get;set;}
        
        public DateTime UpdatedAt {get;set;}
        public int CreatedBy {get;set;}
        public string Description {get;set;}
        public List <Reserve> Attendings {get;set;}
        public Event (){
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}