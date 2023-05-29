using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventManager1.Models
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
        [Display(Name = "Email")]        
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
    public class LoginphoneModel2
    {
        public string CountryId { get; set; }            
        public string Phone { get; set; }        
        public string EmailId { get; set; }
    }
    public class LoginphoneModel
    {
        public int CountryId { get; set; }
        public List<Dropdownlist> Country { get; set; }
        //[Required]
        //[DataType(DataType.PhoneNumber)]
        //[MinLength(9,ErrorMessage ="Enter valid mobile number")]
        public string Phone { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        public string Password { get; set; }

        //[Required]
        public int OTP { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        public string Phone_CountryCode { get; set; }
        public string EmailId { get; set; }
    }
    public class LoginViewModel1
    {        
        public string Email { get; set; }
        
        public string Password { get; set; }
        
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
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
    public class User_
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UserType { get; set; }
        public int UserStatus { get; set; }
        public decimal CreditBalanceTotal { get; set; }
        public decimal CreditBalanceDaily { get; set; }
        public decimal CreditBalanceMonthly { get; set; }
        public int CountryId { get; set; }
        public int CurrencyId { get; set; }
        public int NativeCurrencyId { get; set; }
        public decimal ExchangeRate { get; set; }             
        public string Password { get; set; }        
        public string ProfilePic { get; set; }
        public System.DateTime LastModified { get; set; }
        public bool IsEmailEnable { get; set; }
        public bool IsNotiEnable { get; set; }
        public bool IsMessageEnable { get; set; }
        public System.DateTime CreatedDate { get; set; }
    }
    public class RegisterUser_
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        //[Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone No is required.")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(10,ErrorMessage ="Phone number should not be more than 10")]
        public string PhoneNo { get; set; }
        public string phoneNumber { get; set; }
        
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }
        public int UserType { get; set; }
        public int UserStatus { get; set; }        
        public int CountryId { get; set; }
        public List<Dropdownlist> Country { get; set; }
        public decimal ExchangeRate { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirmation Password is required.")]
        [Compare("Password", ErrorMessage = "Password and Confirmation Password must match.")]
        public string ConfPassword { get; set; }
        public bool IsEmailEnable { get; set; }
        public bool IsNotiEnable { get; set; }
        public bool IsMessageEnable { get; set; }
        public string Phone_CountryCode { get; set; }        
        public ApiResponse Error { get; set; }
        
    }
    public class Users_Heave
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        //[Required(ErrorMessage = "Email is required.")]
        public string Firsttimejoin { get; set; }
        [Required(ErrorMessage = "Phone No is required.")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(10, ErrorMessage = "Phone number should not be more than 10")]
        public string PhoneNo { get; set; }
        public int id { get; set; }
        public string WhichEvent { get; set; }
        public string Package { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string HearSource { get; set; }
        public string Emergency_Contact { get; set; }
        public Nullable<bool> Medical_condition { get; set; }
        public string MedicalCondition { get; set; }
        public string Above_condition { get; set; }
        public Nullable<bool> REFUND_POLICY { get; set; }
        public Nullable<bool> Release_of_Liability { get; set; }
        public Nullable<bool> Social_Media { get; set; }
        public Nullable<bool> Payment { get; set; }
        public Nullable<int> user_id { get; set; }

    }
    public class RegisterOrganization
    {       
        [Required(ErrorMessage = "User Id is required.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        public string ContactNo { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirmation Password is required.")]
        [Compare("Password", ErrorMessage = "Password and Confirmation Password must match.")]
        public string BusinessName { get; set; }        
        public int CountryId { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }               
        public IList<CompOwner> compOwners { get; set; }
    }
    public class CompOwner
    {
        //public int Id { get; set; }
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Phone No is required.")]
        public string PhoneNo { get; set; }
    }
    public  class CompRegister
    {
        public string Email { get; set; }
    }
    public class PaymentSupportModel
    {
        //public int Id { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }        
        public string TrxId { get; set; }
    }
}
