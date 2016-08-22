using System.DirectoryServices.AccountManagement;

namespace PasswordChanger.Application.Contracts
{
    public interface IAdAccountManagementService
    {
        void SetUserPassword(string userName, string newPassword, out string message);
        bool ValidateCredentials(string userName, string sPassword);

        UserPrincipal GetUser(string userName);
    }
}