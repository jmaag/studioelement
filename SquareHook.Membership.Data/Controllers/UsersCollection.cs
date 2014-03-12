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
    /// <summary>
    /// Conceals the methods needed by the main application to access the Users in the database
    /// </summary>
    public class UsersCollection
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="users">Users in the db</param>
        public UsersCollection(MongoCollection<User> users, MongoCollection<UsersInRoles> userRoles)
        {
            this._users = users;
            this._userRoles = userRoles;
        }

        /// <summary>
        /// Users objects accessible for class using this one as base
        /// </summary>
        private MongoCollection<User> _users = null;

        protected MongoCollection<User> Users
        {
            get
            {
                return this._users;
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
        /// Inserts a new user data in the db....isn't being used
        /// </summary>
        /// <param name="user">User info. to be added</param>
        /// <returns>Result of the operation, true if was ok</returns>
        public bool Insert(User user)
        {
            if (user.Validate())
            {
                SafeModeResult result = Users.Insert(user, SafeMode.True);
                return result.Ok;
            }
            else return false;
        }

        /// <summary>
        /// Updates the info of a user...like a change of name, but isn't being used in the app...
        /// </summary>
        /// <param name="user">User to be updated</param>
        /// <returns>Result of the operation, true if was ok</returns>
        public bool Update(User user)
        {
            if (user.Validate())
            {
                user.LastActivityDate = DateTime.UtcNow;
                var result = Users.Save(user, WriteConcern.Acknowledged);
                return result.Ok;
            }
            else return false;
        }

        /// <summary>
        /// Deletes the user with the specified id
        /// </summary>
        /// <param name="userId">User to be deleted id</param>
        /// <returns>Result of the operation, true if was ok</returns>
        public bool Delete(ObjectId userId)
        {
            User user = Users.FindOneById(userId);

            FindAndModifyResult userRoleResult = UserRoles.FindAndRemove(Query.EQ("Username", user.Username), SortBy.Ascending("_id"));
            FindAndModifyResult result = Users.FindAndRemove(Query.EQ("_id", userId), SortBy.Ascending("_id"));
            return result.Ok && userRoleResult.Ok;
        }

        /// <summary>
        /// Searches for all users
        /// </summary>
        /// <returns>List of found users</returns>
        public List<T> GetUsers<T>()
        {
            MongoCursor<T> result = Users.FindAllAs<T>();
            if (result != null)
                return result.ToList();
            else
                return null;
        }

        /// <summary>
        /// Gets a list of users based on paging and searching
        /// </summary>
        /// <param name="take">the number of users to return</param>
        /// <param name="page">the page of users to display</param>
        /// <param name="search">a search term to filter on</param>
        /// <param name="total">returns a total of available users based on the search term</param>
        /// <returns>list of users that are found from the search</returns>        
        public List<User> GetUsers(int take, int page, string search, ref int total)
        {
            if (page < 1) { page = 1; }

            if (search == null) { search = ""; }
            else { search = search.ToLower(); }

            var users = (from u in Users.AsQueryable()
                         where u.Email.Contains(search) || u.Username.Contains(search)
                         orderby u.Email
                         select u);
            total = users.Count();
            return users.Skip((page - 1) * take).Take(take).ToList();
        }

        /// <summary>
        /// Gets a list of users based on paging and searching
        /// </summary>
        /// <param name="take">the number of users to return</param>
        /// <param name="page">the page of users to display</param>
        /// <param name="search">a search term to filter on</param>
        /// <param name="total">returns a total of available users based on the search term</param>
        /// <returns>list of users that are found from the search</returns>        
        public List<User> GetAllUsers()
        {
            return (from u in Users.AsQueryable() orderby u.Name select u).ToList();
        }

        /// <summary>
        /// Searches for the user with the specified Id
        /// </summary>
        /// <param name="userId">Id of the user to be searched</param>
        /// <returns>What was found</returns>
        public T GetUser<T>(ObjectId userId)
        {
            return Users.FindOneByIdAs<T>(userId);
        }

        /// <summary>
        /// returns the user by their user id
        /// </summary>
        /// <param name="userId">the user id to search</param>
        /// <returns>the user information found</returns>
        public User GetUser(ObjectId userId)
        {
            return Users.FindOneById(userId);
        }

        /// <summary>
        /// returns the user by their user name
        /// </summary>
        /// <param name="username">the user name to search for.</param>
        /// <returns>the user information found</returns>
        public User GetUser(string username)
        {
            username = username.ToLower();
            var results = (from u in Users.AsQueryable() where u.Username == username select u);

            return (results.Count() > 0) ? results.First() : null;
        }

        /// <summary>
        /// returns the user by their email address
        /// </summary>
        /// <param name="email">the email to search</param>
        /// <returns>the user information found</returns>
        public User GetUserByEmail(string email)
        {
            var users = (from u in Users.AsQueryable() where u.Email == email select u);
            return users.Count() == 0 ? null : users.First();
        }

        /// <summary>
        /// Searches users by their email and returns a list
        /// </summary>
        /// <param name="search">The search string to search by</param>
        /// <returns>list of users that match the search parameter</returns>
        public List<User> SearchUsersByEmail(string search)
        {
            search = search.ToLower();
            var users = (from u in Users.AsQueryable() where u.Email.StartsWith(search) select u);
            return users.ToList();
        }

        /// <summary>
        /// Checks to see if the username or email exists in the database
        /// </summary>
        /// <param name="search">the user name or email to search for.</param>
        /// <returns>true if found, false if not found.</returns>
        public bool Exists(string search)
        {
            search = search.ToLower();
            return Users.AsQueryable().Any(u => u.Username == search || u.Email == search);
        }

        /// <summary>
        /// Checks to see if the email already exists in the database
        /// </summary>
        /// <param name="email">the email to search for.</param>
        /// <returns>true if found, false if not found.</returns>
        public bool ExistsByEmail(string email)
        {
            email = email.ToLower();
            return Users.AsQueryable().Any(u => u.Email == email);
        }

        /// <summary>
        /// Initializes all the needed indexes for the pages to optimize search on them.
        /// </summary>
        public void InitializeIndexes()
        {
            // key options
            var options = new IndexOptionsBuilder().SetUnique(true);

            // page search key
            var keys = new IndexKeysBuilder().Ascending("Email");
            Users.EnsureIndex(keys, options);

            // general page search
            keys = new IndexKeysBuilder().Ascending("_id");
            Users.EnsureIndex(keys, options);

            // username search
            keys = new IndexKeysBuilder().Ascending("Username");
            Users.EnsureIndex(keys, options);
        }
    }
}
