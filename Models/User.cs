using System;
using System.Collections.Generic;

namespace events.Models
{
    public class User
    {
        public int id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public DateTime CreatedAt {get;set;}

        public DateTime UpdatedAt {get;set;}

        public List <Reserve> Reserves {get;set;}
        public User (){
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}