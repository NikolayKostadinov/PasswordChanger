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
    using log4net;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;

    /// <summary>
    /// Summary description for $classname$
    /// </summary>
    public class ChangePasswordService : IChangePasswordService
    {
        private readonly string hostAddress;
        private readonly string hostPort;
        private readonly ILog logger;

        public ChangePasswordService(ISettingsProvider settingsProvider, ILog loggerParam)
        {
            var settings = settingsProvider.GetSettings();
            this.hostAddress = settings["hostAddress"] as string;
            this.hostPort = settings["hostPort"] as string;
            this.logger = loggerParam;
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
            catch (System.AggregateException ex)
            {
                StringBuilder message = new StringBuilder();
                GetErrorMessage(ex, message);
                logger.Error($"{ message.ToString()}\n{ex.StackTrace}");
                return result.SetErrors(new List<ValidationResult>() { new ValidationResult("There was a problem during password changing process!!!\n Please contact service desk.") });
            }
            catch (Exception ex)
            {
                logger.Error($"{ex.Message}\n{ex.StackTrace}", ex);
                return result.SetErrors(new List<ValidationResult>() { new ValidationResult("There was a problem during password changing process!!!\n Please contact service desk.") });
            }
        }

        private void GetErrorMessage(AggregateException ex, StringBuilder message)
        {
            foreach (var error in ex.InnerExceptions)
            {
                message.AppendLine(ex.Message);
                if (ex.InnerException != null)
                {
                    message.AppendLine($"\t\t\t\t\t\t\t{ex.Message}");
                    GetSubErrorMessage(ex.InnerException, message);
                }
                else
                {
                    return;
                }
            }

        }

        private void GetSubErrorMessage(Exception ex, StringBuilder message)
        {
            message.AppendLine($"\t\t\t\t\t\t\t{ex.Message}");
            if (ex.InnerException != null)
            {
                GetSubErrorMessage(ex.InnerException, message);
            }
            else
            {
                return;
            }
        }
    }
}
