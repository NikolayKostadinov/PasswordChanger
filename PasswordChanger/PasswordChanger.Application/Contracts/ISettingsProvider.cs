namespace PasswordChanger.Application.Contracts
{
    using System.Collections.Generic;

    public interface ISettingsProvider
    {
        IDictionary<string, object> GetSettings();
    }
}