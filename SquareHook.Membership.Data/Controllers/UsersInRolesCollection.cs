using System;
using System.Collections.Generic;
using System.Linq;
using SquareHook.Membership.Data.Models;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace SquareHook.Membership.Data.Controllers
{
    public class UsersInRolesCollection
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="userRoles">User/Role association in the db</param>
        public UsersInRolesCollection(MongoCollection<UsersInRoles> userRoles)
        {
            this._userRoles = userRoles;
        }

        /// <summary>
        /// User/Role association objects accessible from the database.
        /// </summary>
        private MongoCollection<UsersInRoles> _userRoles = null;
        public MongoCollection<UsersInRoles> UserRoles
        {
            get
            {
                return this._userRoles;
            }
        }

        /// <summary>
        /// Inserts a new user/role association into the db.
        /// </summary>
        /// <param name="userRole">The user/role association to add.</param>
        /// <returns>true if the user/role is valid and successfully added.</returns>
        public bool Insert(UsersInRoles userRole)
        {
            if (userRole.Validate())
            {
                userRole.Username = userRole.Username.ToLower();
                userRole.Role = userRole.Role.ToLower();

                // search for a pre-exisiting association
                bool found = (from ur in UserRoles.AsQueryable<UsersInRoles>()
                              where ur.Username == userRole.Username && ur.Role == userRole.Role
                              select ur).Any();

                if (!found)
                {
                    userRole.UserRoleID = ObjectId.GenerateNewId();
                    SafeModeResult result = UserRoles.Insert(userRole, SafeMode.True);
                    return result.Ok;
                }
                else return true;
            }
            else return false;
        }

        // <summary>
        /// Deletes the user/role association with the specified id
        /// </summary>
        /// <param name="userRoleId">Id of the user/role to delete</param>
        /// <returns>Result of the operation, true if was ok</returns>
        public bool Delete(ObjectId userRoleId)
        {
            FindAndModifyResult result = UserRoles.FindAndRemove(Query.EQ("_id", userRoleId), SortBy.Ascending("_id"));
            return result.Ok;
        }

        /// <summary>
        /// Deletes the user/role association by role and username
        /// </summary>
        /// <param name="role">The role to search on</param>
        /// <param name="username">The username to search on</param>
        /// <returns>true if deleted</returns>
        public bool DeleteByRoleAndUserName(string role, string username)
        {
            role = role.ToLower();
            username = username.ToLower();
            var query = Query.And(Query<UsersInRoles>.EQ(ur => ur.Username, username), Query<UsersInRoles>.EQ(ur => ur.Role, role));
            return UserRoles.Remove(query, SafeMode.True).Ok;
        }

        /// <summary>
        /// Returns list of usernames associated to a role
        /// </summary>
        /// <param name="role">The name of the role to search on</param>
        /// <returns>the list of usernames associated to the role</returns>
        public List<UsersInRoles> GetUsersByRole(string role)
        {
            return (from r in UserRoles.AsQueryable() where r.Role == role.ToLower() select r).ToList();
        }

        /// <summary>
        /// Returns list of roles associated to a username
        /// </summary>
        /// <param name="role">The username to search on</param>
        /// <returns>the list of roles associated to the username</returns>
        public List<UsersInRoles> GetRolesByUsername(string username)
        {
            return (from r in UserRoles.AsQueryable() where r.Username == username.ToLower() select r).ToList();
        }

        /// <summary>
        /// Checks to see if the association already exists.
        /// </summary>
        /// <param name="role">the role to check on</param>
        /// <param name="username">the user name to check on</param>
        /// <returns>true if an association is found, false if otherwise.</returns>
        public bool IsAssociated(string role, string username)
        {
            role = role.ToLower();
            username = username.ToLower();
            return UserRoles.AsQueryable().Any(ur => ur.Role == role && ur.Username == username);
        }
    }
}
