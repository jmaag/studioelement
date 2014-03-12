using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SquareHook.Membership.Data.Models
{
    public class Role
    {
        public Role()
        {
            ApplicationName = "/";
        }

        [BsonId]
        public ObjectId RoleID { get; set; }

        [BsonIgnore]
        public string RoleIDToString { get { return RoleID.ToString(); } }

        [BsonRequired]
        public string ApplicationName { get; set; }

        [BsonRequired]
        [BsonElement("Role")]
        public string Name { get; set; }

        public bool Validate()
        {
            return !(String.IsNullOrEmpty(ApplicationName) || String.IsNullOrEmpty(Name));
        }
    }
}
