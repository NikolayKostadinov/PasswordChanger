namespace PasswordChanger.Application.Contracts
{
    public interface IUsersDataDto
    {
        string CurrentPassword { get; set; }
        string NewPassword { get; set; }
        string UserName { get; set; }
    }
}