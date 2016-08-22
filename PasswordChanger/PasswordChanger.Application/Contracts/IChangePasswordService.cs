namespace PasswordChanger.Application.Contracts
{
    public interface IChangePasswordService
    {
        IOperationStatus ChangePassword(IUsersDataDto user);
    }
}