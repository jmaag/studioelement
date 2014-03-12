using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SquareHook.Membership.Data.Models
{
    public class Client
    {
        public Client()
        {
            Users = new List<ObjectId>();
        }

        [BsonId]
        public ObjectId ClientID { get; set; }
        public int ID { get; set; }

        public string Name { get; set; }
        public string Title { get; set; }
        public string LogoUrl { get; set; }
        public string AlertEmail { get; set; }
        public bool CaptureEmail { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime Deleted { get; set; }

        [BsonIgnore]
        public string ClientIDToString { get { return ClientID.ToString(); } }

        public List<ObjectId> Users { get; set; }
    }
}
