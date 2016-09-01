using log4net;
using PasswordChanger.Application.Contracts;
using PasswordChanger.Application.Dto;
using PasswordChanger.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PasswordChanger.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IChangePasswordService service;
        private readonly ILog logger;
        private readonly IMailerService mailerService;

        public HomeController(IChangePasswordService serviceParam, ILog loggerParam, IMailerService mailerServiceParam)
        {
            this.logger = loggerParam;
            this.service = serviceParam;
            this.mailerService = mailerServiceParam;
        }

        [RequireHttps]
        public ActionResult Index()
        {
            return View();
        }

        [RequireHttps]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UsersDataDto user)
        {
            if (this.ModelState.IsValid)
            {
                var result = service.ChangePassword(user);
                if (result.IsValid)
                {
                    TempData["SuccessMessage"] = String.Join("<br/>", result.SuccessMessages);
                    string message = $"{user.UserName} - Password was changed successfully (OldPasswordLength) [{user.CurrentPassword.Length}] to (NewPasswordLength) [{user.NewPassword.Length}]";
                    this.logger.Info(message);
                    this.mailerService.SendMail(message, "Password changed successfully");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.OperationErrors)
                    {
                        if (string.IsNullOrEmpty(error.MemberNames.FirstOrDefault()))
                        {
                            this.TempData["ErrorMessage"] = error.ErrorMessage;
                        }

                        this.ModelState.AddModelError(string.Empty, error.ErrorMessage);

                        string memberName = (!string.IsNullOrEmpty(error.MemberNames.FirstOrDefault() ?? string.Empty)) ? $"FieldName: {error.MemberNames.FirstOrDefault()}; Error: " : string.Empty;
                        string message = $"{user.UserName} - {memberName}{error.ErrorMessage}";
                        logger.Error(message);
                        this.mailerService.SendMail(message, "Password change failed");

                    }
                }
            }

            return View(user);

        }


        [HttpGet]
        public ActionResult RedirectToLogin()
        {
            return Redirect("http://sdm.bmsys.eu/CAisd/pdmweb.exe");
        }
    }
}