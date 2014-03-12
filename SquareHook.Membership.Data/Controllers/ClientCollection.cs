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
    public class ClientCollection
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="roles">Roles in the db</param>
        /// <param name="userRoles">User/Role association in the db</param>
        public ClientCollection(MongoCollection<Client> clients)
        {
            this._clients = clients;
        }

        /// <summary>
        /// Roles objects accessible from the database.
        /// </summary>
        private MongoCollection<Client> _clients = null;
        protected MongoCollection<Client> Clients
        {
            get
            {
                return this._clients;
            }
        }

        /// <summary>
        /// Inserts a new role into the db.
        /// </summary>
        /// <param name="role">The role to add.</param>
        /// <returns>true if the role is valid and successfully added.</returns>
        public bool Insert(Client client)
        {
            client.ClientID = ObjectId.GenerateNewId();
            var result = Clients.Insert(client, WriteConcern.Acknowledged);
            return result.Ok;
        }

        /// <summary>
        /// Inserts a new role into the db.
        /// </summary>
        /// <param name="role">The role to add.</param>
        /// <returns>true if the role is valid and successfully added.</returns>
        public bool Save(Client client)
        {
            client.Updated = DateTime.UtcNow;
            var result = Clients.Save(client, WriteConcern.Acknowledged);
            return result.Ok;
        }

        /// <summary>
        /// Inserts a new role into the db.
        /// </summary>
        /// <param name="role">The role to add.</param>
        /// <returns>true if the role is valid and successfully added.</returns>
        public void InsertBatch(List<Client> clients)
        {
            foreach (var client in clients)
            {
                client.ClientID = ObjectId.GenerateNewId();
            }
            Clients.InsertBatch(clients, WriteConcern.Unacknowledged);
        }

        public bool HasClient(int id)
        {
            return (from c in Clients.AsQueryable() where c.ID == id select c).Any();
        }

        public ObjectId GetClientID(int id)
        {
            var client = (from c in Clients.AsQueryable() where c.ID == id select c);

            if (client.Count() <= 0)
            {
                return ObjectId.Empty;
            }
            else
            {
                return client.First().ClientID;
            }
        }

        // <summary>
        /// Deletes the role with the specified id
        /// </summary>
        /// <param name="roleId">Id of the role to delete</param>
        /// <returns>Result of the operation, true if was ok</returns>
        public bool Delete(ObjectId clientID)
        {
            Client client = Clients.FindOneById(clientID);
            client.Deleted = DateTime.UtcNow;

            var result = Clients.Save(client, WriteConcern.Acknowledged);

            return result.Ok;
        }

        /// <summary>
        /// Returns the role by their role id
        /// </summary>
        /// <param name="roleId">The id of the role to retrieve</param>
        /// <returns>the role found or null</returns>
        public Client GetClient(ObjectId clientID)
        {
            return Clients.FindOneById(clientID);
        }

        public Client GetClientByName(string name)
        {
            Client client = new Client();
            try
            {
                client = (from c in Clients.AsQueryable() where c.Name == name.ToLower() select c).First();
            }
            catch { }

            return client;
        }

        /// <summary>
        /// Searches roles by their name and returns a list
        /// </summary>
        /// <param name="search">The search string to search by</param>
        /// <returns>list of roles that match the search parameter</returns>
        public List<Client> Search(string search)
        {
            search = search.ToLower();
            var roles = (from r in Clients.AsQueryable() where r.Name.ToLower().Contains(search) select r);
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
        public List<Client> GetClients(int take, int page, string search, ref int total)
        {
            if (page < 1) { page = 1; }

            if (search == null) { search = ""; }
            else { search = search.ToLower(); }

            var roles = (from r in Clients.AsQueryable()
                         where r.Name.ToLower().Contains(search)
                         orderby r.Name
                         select r);
            total = roles.Count();
            return roles.Skip((page - 1) * take).Take(take).ToList();
        }


        public List<Client> GetAllClients()
        {
            return (from c in Clients.AsQueryable() where c.Deleted <= DateTime.MinValue orderby c.Name select c).ToList();
        }

        /// <summary>
        /// Initializes all the needed indexes for the pages to optimize search on them.
        /// </summary>
        public void InitializeIndexes()
        {
            // key options
            var options = new IndexOptionsBuilder().SetUnique(true);

            // page search key
            var keys = new IndexKeysBuilder().Ascending("Name");
            Clients.EnsureIndex(keys, options);

            // general page search
            keys = new IndexKeysBuilder().Ascending("_id");
            Clients.EnsureIndex(keys, options);
        }
    }
}
