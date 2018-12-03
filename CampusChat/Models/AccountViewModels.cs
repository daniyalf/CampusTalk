using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CampusChat.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set;}

        [Required]
        [EmailAddress]
        [EduEmail]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set;}

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set;}

        [Required]
        [Display(Name = "State")]
        public string State { get; set;}

        [Required]
        [Display(Name = "University")]
        public string University { get; set;}

        [Required]
        [Display(Name = "Major")]
        public string Major { get; set;}

        [Required]
        [Display(Name = "Graduation Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        public DateTime ExpectedGraduationDate{ get; set; }

        [Required]
        [Display(Name = "By checking this box, you are agreeing to the terms of use.")]
        public bool AgreedToTerms { get; set;}
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class EduEmailAttribute : ValidationAttribute
    {
        string email = "";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            RegisterViewModel regViewModel = (RegisterViewModel)validationContext.ObjectInstance;
            email = regViewModel.Email;
            if((email.Substring(email.Length - 4, 4)).Equals(".edu"))
                return ValidationResult.Success;
            return new ValidationResult(GetErrorMessage());
        }

        private string GetErrorMessage()
        {
            return $"School email addresses must end in '.edu'.";
        }
    }
}
