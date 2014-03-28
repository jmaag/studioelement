using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SquareHook.Membership.Areas.Settings.Models
{
    public class UserModel
    {
        public Guid UserID { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastActivityDate { get; set; }
    }
}