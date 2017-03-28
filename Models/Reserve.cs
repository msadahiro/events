using System;

namespace events.Models
{
    public class Reserve
    {
        public int id { get; set; }
        public int UserId {get;set;}
        public User User {get;set;}

        public int EventId {get;set;}
        public Event Event {get;set;}
    }
}