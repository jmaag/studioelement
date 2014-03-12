using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Bson;
using SquareHook.Membership.Data.Models;
using SquareHook.Membership.Data.Controllers;

namespace SquareHook.Membership.Data
{
    public class DataContext
    {
        #region constructors

        public DataContext(string connectionString)
        {
            ConnectionString = connectionString;

        }

        #endregion

        #region database setup

        private MongoDatabase _database = null;
        public MongoDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    _database = MongoDatabase.Create(ConnectionString);
                }
                return _database;
            }
            set
            {
                _database = value;
            }
        }

        public string ConnectionString { get; set; }

        #endregion

        #region collection names

        private const string kUsers = "Users";
        private const string kUsersInSites = "UsersInSites";
        private const string kRoles = "Roles";
        private const string kUsersInRoles = "UsersInRoles";
        private const string kWebEvents = "WebEvents";

        private const string kClients = "Client";
        private const string kMetrics = "Metric";
        private const string kSurveys = "Survey";
        private const string kReviews = "Review";

        #endregion

        #region collections

        private WebEventsCollection _webEvents = null;
        public WebEventsCollection WebEvents
        {
            get
            {
                if (_webEvents == null)
                {
                    _webEvents = new WebEventsCollection(Database.GetCollection<WebEvent>(kWebEvents));
                }
                return _webEvents;
            }
        }

        private UsersCollection _users = null;
        public UsersCollection Users
        {
            get
            {
                if (_users == null)
                {
                    _users = new UsersCollection(Database.GetCollection<User>(kUsers), Database.GetCollection<UsersInRoles>(kUsersInRoles));
                }
                return _users;
            }
        }

        private RolesCollection _roles = null;
        public RolesCollection Roles
        {
            get
            {
                if (_roles == null)
                {
                    _roles = new RolesCollection(Database.GetCollection<Role>(kRoles), Database.GetCollection<UsersInRoles>(kUsersInRoles));
                }
                return _roles;
            }
        }

        private UsersInRolesCollection _usersInRoles = null;
        public UsersInRolesCollection UsersInRoles
        {
            get
            {
                if (_usersInRoles == null)
                {
                    _usersInRoles = new UsersInRolesCollection(Database.GetCollection<UsersInRoles>(kUsersInRoles));
                }
                return _usersInRoles;
            }
        }

        private ClientCollection _clients = null;
        public ClientCollection Clients
        {
            get
            {
                if (_clients == null)
                {
                    _clients = new ClientCollection(Database.GetCollection<Client>(kClients));
                }
                return _clients;
            }
        }

        /*private MetricCollection _metrics = null;
        public MetricCollection Metrics
        {
            get
            {
                if (_metrics == null)
                {
                    _metrics = new MetricCollection(Database.GetCollection<Metric>(kMetrics));
                }
                return _metrics;
            }
        }

        private SurveyCollection _surveys = null;
        public SurveyCollection Surveys
        {
            get
            {
                if (_surveys == null)
                {
                    _surveys = new SurveyCollection(Database.GetCollection<Survey>(kSurveys));
                }
                return _surveys;
            }
        }

        private ReviewCollection _reviews = null;
        public ReviewCollection Reviews
        {
            get
            {
                if (_reviews == null)
                {
                    _reviews = new ReviewCollection(Database.GetCollection<Review>(kReviews));
                }
                return _reviews;
            }
        }*/

        #endregion

        public MongoCollection<T> GetCollection<T>()
        {
            return Database.GetCollection<T>(typeof(T).Name);
        }
    }
}
