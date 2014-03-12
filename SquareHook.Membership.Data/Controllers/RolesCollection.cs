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
    public class RolesCollection
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="roles">Roles in the db</param>
        /// <param name="userRoles">User/Role association in the db</param>
        public RolesCollection(MongoCollection<Role> roles, MongoCollection<UsersInRoles> userRoles)
        {
            this._roles = roles;
            this._userRoles = userRoles;
        }

        /// <summary>
        /// Roles objects accessible from the database.
        /// </summary>
        private MongoCollection<Role> _roles = null;
        protected MongoCollection<Role> Roles
        {
            get
            {
                return this._roles;
            }
        }

        /// <summary>
        /// User/Role association objects accessible from the database.
        /// </summary>
        private MongoCollection<UsersInRoles> _userRoles = null;
        protected MongoCollection<UsersInRoles> UserRoles
        {
            get
            {
                return this._userRoles;
            }
        }

        /// <summary>
        /// Inserts a new role into the db.
        /// </summary>
        /// <param name="role">The role to add.</param>
        /// <returns>true if the role is valid and successfully added.</returns>
        public bool Insert(Role role)
        {
            if (role.Validate())
            {
                role.RoleID = ObjectId.GenerateNewId();
                SafeModeResult result = Roles.Insert(role);
                return true;
            }
            else return false;
        }

        // <summary>
        /// Deletes the role with the specified id
        /// </summary>
        /// <param name="roleId">Id of the role to delete</param>
        /// <returns>Result of the operation, true if was ok</returns>
        public bool Delete(ObjectId roleId)
        {
            Role role = Roles.FindOneById(roleId);

            FindAndModifyResult userRoleResult = UserRoles.FindAndRemove(Query.EQ("Role", role.Name), SortBy.Ascending("_id"));
            FindAndModifyResult roleResult = Roles.FindAndRemove(Query.EQ("_id", roleId), SortBy.Ascending("_id"));

            return roleResult.Ok && userRoleResult.Ok;
        }

        /// <summary>
        /// Returns all roles from the database
        /// </summary>
        /// <returns>Returns all roles from the database</returns>
        public List<Role> GetAllRoles()
        {
            return Roles.FindAll().ToList();
        }

        /// <summary>
        /// Returns the role by their role id
        /// </summary>
        /// <param name="roleId">The id of the role to retrieve</param>
        /// <returns>the role found or null</returns>
        public Role GetRole(ObjectId roleId)
        {
            return Roles.FindOneById(roleId);
        }

        /// <summary>
        /// Returns the role by their name
        /// </summary>
        /// <param name="name">The name of the role to search on</param>
        /// <returns>the role found or null</returns>
        public Role GetRoleByName(string name)
        {
            var roles = (from r in Roles.AsQueryable() where r.Name == name.ToLower() select r);
            return roles.Count() == 0 ? null : roles.First();
        }

        /// <summary>
        /// Searches roles by their name and returns a list
        /// </summary>
        /// <param name="search">The search string to search by</param>
        /// <returns>list of roles that match the search parameter</returns>
        public List<Role> SearchRoleByName(string search)
        {
            search = search.ToLower();
            var roles = (from r in Roles.AsQueryable() where r.Name.StartsWith(search) select r);
            return roles.ToList();
        }

        /// <summary>
        /// Gets a list of roles based on paging and searching
        /// </summary>
        /// <param name="take">the number of roles to return</param>
        /// <param name="page">the page of roles to display</param>
        /// <param name="search">a search term to filter on</param>
        /// <param name="total">returns a total of available roles based on the search term</param>
        /// <returns>list of roles that are found from the search</returns>        
        public List<Role> GetRoles(int take, int page, string search, ref int total)
        {
            if (page < 1) { page = 1; }

            if (search == null) { search = ""; }
            else { search = search.ToLower(); }

            var roles = (from r in Roles.AsQueryable()
                         where r.Name.Contains(search)
                         orderby r.Name
                         select r);
            total = roles.Count();
            return roles.Skip((page - 1) * take).Take(take).ToList();
        }

        /// <summary>
        /// Checks to see if the role exists in the database
        /// </summary>
        /// <param name="role">the role to search for.</param>
        /// <returns>true if found, false if not found.</returns>
        public bool Exists(string role)
        {
            role = role.ToLower();
            return Roles.AsQueryable().Any(u => u.Name == role);
        }

        /// <summary>
        /// Initializes all the needed indexes for the pages to optimize search on them.
        /// </summary>
        public void InitializeIndexes()
        {
            // key options
            var options = new IndexOptionsBuilder().SetUnique(true);

            // page search key
            var keys = new IndexKeysBuilder().Ascending("Role");
            Roles.EnsureIndex(keys, options);

            // general page search
            keys = new IndexKeysBuilder().Ascending("_id");
            Roles.EnsureIndex(keys, options);
        }
    }
}
