using log4net;
using PasswordChanger.Application.Contracts;
using PasswordChanger.Application.Dto;
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

        public HomeController(IChangePasswordService serviceParam, ILog loggerParam)
        {
            this.logger = loggerParam;
            this.service = serviceParam;
        }

        public ActionResult Index()
        {
            return View();
        }

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
                    this.logger.Info($"{user.UserName} - Password was changed successfully (OldPasswordLength) [{user.CurrentPassword.Length}] to (NewPasswordLength) [{user.NewPassword.Length}]");
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

                        this.ModelState.AddModelError(error.MemberNames.FirstOrDefault(),error.ErrorMessage);

                        string memberName = (!string.IsNullOrEmpty(error.MemberNames.FirstOrDefault() ?? string.Empty)) ? $"FieldName: {error.MemberNames.FirstOrDefault()}; Error: " : string.Empty;
                        logger.Error($"{user.UserName} - {memberName}{error.ErrorMessage}");
                    }
                }
            }

            return View(user);

        }
    }
}