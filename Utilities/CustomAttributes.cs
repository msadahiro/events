using System;
using System.ComponentModel.DataAnnotations;
namespace events
{
    // this is a custom class called InThePast. It inherits from ValidationAttribute
    public class InThePast : ValidationAttribute
    {

        // Creating a new variable called CurrentDate which a DateTime object
        private DateTime CurrentDate;

        // For constructor function: setting CurrentDate to DateTime.Now (so no dates will be in future)
        public InThePast()
        {
            CurrentDate = DateTime.Now;
        }

        // We are overriding the IsValid method to return a ValidationResult object. 
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Casting the incoming "value" as a DateTime object and comparing it below
            DateTime setVal = (DateTime)value;
            // Checking to see if setVal is before CurrentDate
            if (setVal < CurrentDate)
            {
                // Returns ValidationResult of success if it's before Today's Date and Time.
                return ValidationResult.Success;
            }
            // If setVal is invalid, we return error message
            return new ValidationResult("Date of visit must be in the past!");
        }
    }
}