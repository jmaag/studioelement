using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SquareHook.Membership.Models;
using SquareHook.Membership.Data.Models;
using System.Net.Mail;

namespace SquareHook.Membership.Controllers
{
    public class ApplicationController : Controller
    {
        public IRoleService RoleService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            if (RoleService == null) { RoleService = new AccountRoleService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            ViewBag.IsAdmin = IsAdmin;
            ViewBag.IsRestaurant = IsRestaurant;
            base.OnResultExecuting(filterContext);
        }

        /// <summary>
        /// Constructor for the application wide controller that configures default values for the entire site.
        /// </summary>
        public ApplicationController()
        {
            // retrieve the values
            Build = ConfigurationManager.AppSettings["Build"];

            // attempt to retrieve the build date, if fail, just use the current date
            string date = ConfigurationManager.AppSettings["BuildDate"];
            try { BuildDate = Convert.ToDateTime(date); }
            catch { BuildDate = DateTime.MinValue; }
        }

        /// <summary>
        /// Build version for the current release of the application.  Used to tell us which version we are looking at.
        /// </summary>
        public string Build { get; set; }

        /// <summary>
        /// Date of the current build
        /// </summary>
        public DateTime BuildDate { get; set; }


        /// <summary>
        /// String that summarizes the build and build date for the current application.
        /// </summary>
        public string CurrentBuild
        {
            get
            {
                return ViewData["CurrentBuild"].ToString() ?? "";
            }
        }

        private SquareHook.Membership.Data.DataContext _context;

        /// <summary>
        /// Data context that is used to communicate with and retrieve the data from the database.
        /// </summary>
        public SquareHook.Membership.Data.DataContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new SquareHook.Membership.Data.DataContext(ConfigurationManager.ConnectionStrings["MongoConnection"].ConnectionString);
                }
                return _context;
            }
        }

        #region permissions names

        public const string kAdministratorRoles = "admin, developer";

        #endregion

        #region application settings

        /// <summary>
        /// Config field that states if the application is currently in production or not.
        /// </summary>
        public bool IsProduction
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["Production"]);
            }
        }

        #endregion

        #region permissions

        /// <summary>
        /// returns whether the current user is an administrator
        /// </summary>
        public bool IsAdmin
        {
            get
            {
                return IsInRole("admin");
            }
        }

        public bool IsRestaurant
        {
            get
            {
                return IsInRole("restaurant");
            }
        }

        /// <summary>
        /// checks to see if the current logged in user is in the specified role.
        /// </summary>
        /// <param name="role">role to check with</param>
        /// <returns>returns true if logged in user is in the given role.</returns>
        private bool IsInRole(string role)
        {
            return RoleService.IsUserInRole(User.Identity.Name, role);
        }

        #endregion

        #region site management

        public MembershipUser SiteUser
        {
            get
            {
                return MembershipService.GetUser();
            }
        }

        public ObjectId SiteUserId
        {
            get
            {
                if (SiteUser != null)
                {
                    return new ObjectId(SiteUser.ProviderUserKey.ToString());
                }
                return ObjectId.Empty;
            }
        }

        private User _currentUser = null;
        public User CurrentUser
        {
            get
            {
                if (_currentUser == null) { _currentUser = Context.Users.GetUser(this.SiteUserId); }
                return _currentUser;
            }
        }
        #endregion

        public Client getClient()
        {
            var user = Context.Users.GetUser(SiteUserId);
            Client customer = new Client();
            if (user.Clients != null && user.Clients.Count > 0)
            {
                customer = Context.Clients.GetClient(user.Clients.First());
            }

            return customer;
        }

        #region email

        /// <summary>
        /// Used to send email messages out
        /// </summary>
        /// <param name="tos">The list of users to send the message to.</param>
        /// <param name="ccs">List of users to carbon copy</param>
        /// <param name="subject">The subject of the message</param>
        /// <param name="body">The body of the message</param>
        public void SendMail(List<string> tos, List<string> ccs, string subject, string body)
        {
            SendMail(tos, ccs, subject, body, "Hunter@tapsterNFC.com");
        }

        /// <summary>
        /// Used to send email messages out
        /// </summary>
        /// <param name="tos">The list of users to send the message to.</param>
        /// <param name="ccs">List of users to carbon copy</param>
        /// <param name="subject">The subject of the message</param>
        /// <param name="body">The body of the message</param>
        /// <param name="from">The sender of the message</param>
        public void SendMail(List<string> tos, List<string> ccs, string subject, string body, string from)
        {
            using (SmtpClient client = new SmtpClient())
            {
                client.EnableSsl = true;
                MailMessage message = new MailMessage();

                if (tos != null)
                {
                    foreach (string email in tos)
                    {
                        message.To.Add(email);
                    }
                }

                if (ccs != null)
                {
                    foreach (string email in ccs)
                    {
                        message.Bcc.Add(email);
                    }
                }

                message.From = new MailAddress(from);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                client.Send(message);
            }
        }

        public string ToRelativeDate(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan <= TimeSpan.FromSeconds(60))
                return string.Format("{0} seconds ago", timeSpan.Seconds);

            if (timeSpan <= TimeSpan.FromMinutes(60))
                return timeSpan.Minutes > 1 ? String.Format("{0} minutes ago", timeSpan.Minutes) : "a minute ago";

            if (timeSpan <= TimeSpan.FromHours(24))
                return timeSpan.Hours > 1 ? String.Format("{0} hours ago", timeSpan.Hours) : "an hour ago";

            if (timeSpan <= TimeSpan.FromDays(30))
                return timeSpan.Days > 1 ? String.Format("{0} days ago", timeSpan.Days) : "yesterday";

            if (timeSpan <= TimeSpan.FromDays(365))
                return timeSpan.Days > 30 ? String.Format("{0} months ago", timeSpan.Days / 30) : "a month ago";

            return timeSpan.Days > 365 ? String.Format("{0} years ago", timeSpan.Days / 365) : "a year ago";
        }

        #endregion

        public string ClientIP
        {
            get
            {
                return Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? Request.UserHostAddress;
            }
        }
    }
}
