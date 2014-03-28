using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using SquareHook.Membership.Areas.Settings.Models;
using SquareHook.Membership.Models;

namespace SquareHook.Membership.Areas.Settings.Controllers
{
    [Authorize]
    public class UsersController : SquareHook.Membership.Controllers.ApplicationController
    {
        //
        // GET: /Settings/Users/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetUsers(int take, int page, string search)
        {
            int total = 0;

            if (page < 1) { page = 1; }
            if (search == null) { search = ""; }
            else { search = search.ToLower(); }

            var providers = (from l in Context.aspnet_Users
                             where l.LoweredUserName.Contains(search) || l.aspnet_Membership.LoweredEmail.Contains(search)
                             orderby l.aspnet_Membership.LoweredEmail
                             select new
                             {
                                 l.UserId,
                                 l.UserName,
                                 LastActivityDate = l.LastActivityDate.ToString(),
                                 Email = l.aspnet_Membership.Email,
                                 l.aspnet_Membership.IsLockedOut, 
                                 LastLoginDate = l.aspnet_Membership.LastLoginDate.ToString(), 
                                 CreateDate = l.aspnet_Membership.CreateDate.ToString()
                             });
            total = providers.Count();

            var results = providers.Skip((page - 1) * take).Take(take).ToList();

            return Json(new
            {
                total = total,
                results = results
            });
        }

        public ActionResult Details(Guid id)
        {
            var user = (from u in Context.aspnet_Users where u.UserId == id select u).First();
            var model = new UserModel()
            {
                CreationDate = user.aspnet_Membership.CreateDate,
                Email = user.aspnet_Membership.LoweredEmail, 
                LastActivityDate = user.LastActivityDate, 
                UserID = user.UserId, 
                Username = user.UserName
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Details(UserModel model)
        {
            var user = (from u in Context.aspnet_Users where u.UserId == model.UserID select u).First();

            bool password = false;
            if (!String.IsNullOrEmpty(model.OldPassword) || !String.IsNullOrEmpty(model.NewPassword) || !String.IsNullOrEmpty(model.ConfirmPassword))
            {
                if (String.IsNullOrEmpty(model.OldPassword))
                {
                    ModelState.AddModelError("OldPassword", "Required");
                }

                if (String.IsNullOrEmpty(model.NewPassword))
                {
                    ModelState.AddModelError("NewPassword", "Required");
                }

                if (String.IsNullOrEmpty(model.ConfirmPassword))
                {
                    ModelState.AddModelError("ConfirmPassword", "Required");
                }

                if (model.NewPassword != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Must Match");
                }
                password = true;
            }

            if (ModelState.IsValid)
            {
                user.UserName = model.Username.ToLower();
                user.LoweredUserName = user.UserName;
                user.aspnet_Membership.Email = model.Email.ToLower();
                user.aspnet_Membership.LoweredEmail = model.Email.ToLower();

                Context.SubmitChanges();

                if (password)
                {
                    // ChangePassword will throw an exception rather
                    // than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        MembershipUser currentUser = System.Web.Security.Membership.GetUser(user.LoweredUserName, true /* userIsOnline */);
                        changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (!changePasswordSucceeded)
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult Delete(string id)
        {
            bool success = true;

            try
            {
                success = System.Web.Security.Membership.DeleteUser(id);
            }
            catch { success = false; }

            return Json(new { success = success });
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Guid id = Guid.NewGuid();
                System.Web.Security.Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, id, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    return RedirectToAction("Details", new { id = id });
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

    }
}
