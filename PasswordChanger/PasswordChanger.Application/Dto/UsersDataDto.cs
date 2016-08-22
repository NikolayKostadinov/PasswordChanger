namespace PasswordChanger.Application.Dto
{
    using Contracts;
    using System.ComponentModel.DataAnnotations;

    public class UsersDataDto : IUsersDataDto
    {
        [Required]
        [Display(Name="User Name")]
        [MinLength(2)]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        [Required]
        [RegularExpression(@"((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).{8,20})", ErrorMessage = "The entered password does not meet complexity rules!")]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}