//-----------------------------------------------------------------------
// <copyright file="PasswordChangerController" company="Business Management Systems Ltd.">
//     Copyright &year& (c) . All rights reserved.
// </copyright>
// <author>Nikolay.Kostadinov</author>
//-----------------------------------------------------------------------

namespace PasswordChanger.WebApiService.Controllers
{
    using PasswordChanger.Application.Contracts;
    using log4net;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Application.Dto;


    /// <summary>
    /// Summary description for PasswordChangerController
    /// </summary>
    public class PasswordChangerController:ApiController
    {
        private readonly IAdAccountManagementService accountService;
        private readonly ILog logger;

        public PasswordChangerController(IAdAccountManagementService accountServiceParam, ILog loggerParam)
        {
            this.accountService = accountServiceParam;
            this.logger = loggerParam;
        }


        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="currentPassword">The current password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns></returns>
        [Route("api/PasswordChanger/ChangePassword")]
        [HttpPost]
        public HttpResponseMessage ChangePassword([FromBody] UsersDataDto user)
        {
            try
            {
                string message = "";

                if (accountService.GetUser(user.UserName) == null)
                {
                    message = "Invalid User Name";
                    this.logger.Error($"{user.UserName} - {message}");
                    return Request.CreateResponse(HttpStatusCode.RequestedRangeNotSatisfiable, message);
                }

                if (this.accountService.ValidateCredentials(user.UserName, user.CurrentPassword))
                {
                    this.accountService.SetUserPassword(user.UserName, user.NewPassword, out message);

                    if (string.IsNullOrEmpty(message))
                    {
                        this.logger.Info($"{user.UserName} - Password was changed successfully (OldPasswordLength) [{user.CurrentPassword.Length}] to (NewPasswordLength) [{user.NewPassword.Length}]");
                        return Request.CreateResponse( HttpStatusCode.OK, $"User's {user.UserName} password was changed successfully!!!");
                    }
                    else
                    {
                        this.logger.Error($"{user.UserName} - {message}");
                        return Request.CreateResponse(HttpStatusCode.BadRequest, $"There was a problem during password changing process!!!\n Please contact service desk.");
                    }
                }
                else
                {
                    this.logger.Error($"{user.UserName} - Invalid User's credentials!");
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, $"Invalid password");
                }

            }
            catch (Exception exception)
            {
                this.logger.Fatal($"{exception.Message} \n{exception.StackTrace}");
                return Request.CreateResponse(HttpStatusCode.BadRequest, $"There was a problem during password changing process!!!\n Please contact service desk.");
            }
        }
    }
}
