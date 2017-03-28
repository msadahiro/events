using System.ComponentModel.DataAnnotations;

namespace events.Models{
    public class RegisterViewModel{


        [Required]
        [MinLengthAttribute(2)]
        [RegularExpression(@"^[a-zA-Z]+$")]
        public string FirstName {get;set;}
        
        [Required]
        [MinLengthAttribute(2)]
        [RegularExpression(@"^[a-zA-Z]+$")]
        public string LastName {get;set;}

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=(.*[a-zA-Z].*){2,})(?=.*\d.*)(?=.*\W.*)[a-zA-Z0-9\S]{8,15}$", ErrorMessage = "Password must contain at least 2 letters, one special character, one number and no spaces. Password must be between 8-15 characters")]
        public string Password { get; set; }
 
        [Compare("Password", ErrorMessage = "Password and confirmation must match.")]
        public string PasswordConfirmation { get; set; }
    }
}