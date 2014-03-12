using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SquareHook.Membership.Data.Models
{
    public class UsersInRoles
    {
        public UsersInRoles()
        {
        }

        [BsonId]
        public ObjectId UserRoleID { get; set; }

        public string UserRoleIDToString { get { return UserRoleID.ToString(); } }

        [BsonRequired]
        public string ApplicationName { get; set; }

        [BsonRequired]
        public string Role { get; set; }

        [BsonRequired]
        public string Username { get; set; }

        public bool Validate()
        {
            return !(String.IsNullOrEmpty(ApplicationName) || String.IsNullOrEmpty(Role) || String.IsNullOrEmpty(Username));
        }
    }
}
