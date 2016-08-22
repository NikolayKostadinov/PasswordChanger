//-----------------------------------------------------------------------
// <copyright file="ChangePasswordService" company="">
//     Copyright &year& (c) . All rights reserved.
// </copyright>
// <author>Nikolay.Kostadinov</author>
//-----------------------------------------------------------------------

namespace PasswordChanger.Application.Services
{
    using Common;
    using Contracts;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;

    /// <summary>
    /// Summary description for $classname$
    /// </summary>
    public class ChangePasswordService : IChangePasswordService
    {
        private readonly string hostAddress;
        public readonly string hostPort;

        public ChangePasswordService(ISettingsProvider settingsProvider)
        {
            var settings = settingsProvider.GetSettings();
            this.hostAddress = settings["hostAddress"] as string;
            this.hostPort = settings["hostPort"] as string;
        }

        public IOperationStatus ChangePassword(IUsersDataDto user)
        {
            var result = new OperationStatus();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string url = $"http://{this.hostAddress}:{this.hostPort}/api/PasswordChanger/ChangePassword";

            HttpResponseMessage response = null;
            try
            {
                response = client.PostAsJsonAsync<IUsersDataDto>(url, user).Result;
                string returnValue = response.Content.ReadAsAsync<string>().Result;
                if (response.IsSuccessStatusCode)
                {
                    return result.SetSuccessMessages(new string[] { returnValue });
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.RequestedRangeNotSatisfiable)
                    {
                        return result.SetErrors(new List<ValidationResult>() { new ValidationResult(returnValue, new string[] { nameof(user.UserName) }) });
                    }
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return result.SetErrors(new List<ValidationResult>() { new ValidationResult(returnValue, new string[] { nameof(user.CurrentPassword) }) });
                    }
                    return result.SetErrors(new List<ValidationResult>() { new ValidationResult(returnValue) });
                }
            }
            catch (Exception ex)
            {
                return result.SetErrors(new List<ValidationResult>() { new ValidationResult("There was a problem during password changing process!!!\n Please contact service desk.") });
                throw ex;
            }
        }
    }
}
