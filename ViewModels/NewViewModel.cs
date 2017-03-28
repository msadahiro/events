using System;
using System.ComponentModel.DataAnnotations;

namespace events.Models{
    public class NewViewModel{
        [Required]
        [MinLengthAttribute(2)]
        public string Title {get;set;}

        [Required]
        [InTheFuture]
        public DateTime EventDate {get;set;}

        [Required]
        public string Duration {get;set;}
        [Required]
        public string Length {get;set;}

        [Required]
        [MinLengthAttribute(10)]
        public string Description {get;set;}

    }
}