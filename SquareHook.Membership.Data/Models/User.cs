using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace SquareHook.Membership.Data.Models
{
    public class User
    {
        public User()
        {
            Clients = new List<ObjectId>();
        }

        [BsonExtraElements]
        public BsonDocument ExtraElements { get; set; }

        [BsonId]
        public ObjectId UserID { get; set; }
        [BsonIgnore]
        public string UserIDToString { get { return UserID.ToString(); } }

        public string ApplicationName { get; set; }

        public DateTime CreationDate { get; set; }
        [BsonRequired]
        public string Email { get; set; }

        public int FailedPasswordAnswerAttemptCount { get; set; }

        public DateTime FailedPasswordAnswerAttemptWindowStart { get; set; }

        public int FailedPasswordAttemptCount { get; set; }

        public DateTime FailedPasswordAttemptWindowStart { get; set; }

        public bool IsApproved { get; set; }

        public bool IsLockedOut { get; set; }

        public DateTime LastActivityDate { get; set; }

        public DateTime LastLockoutDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        public DateTime LastPasswordChangedDate { get; set; }
        [BsonRequired]
        public string Password { get; set; }
        [BsonRequired]
        public string Salt { get; set; }
        [BsonRequired]
        public string Username { get; set; }

        // profile information
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public List<ObjectId> Clients { get; set; }

        public bool Validate()
        {
            return (Password.Length >= 4) && (Salt.Length > 0) && (Email.Length > 0) && (Username.Length > 0);
        }
    }
}
