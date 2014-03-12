using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace SquareHook.Membership.Data.Models
{
    public class WebEvent
    {
        public WebEvent()
        {
        }

        [BsonExtraElements]
        public BsonDocument ExtraElements { get; set; }

        [BsonId]
        public ObjectId _id { get; set; }

        public virtual string ApplicationPath { get; set; }
        [BsonRequired]
        public virtual string ApplicationVirtualPath { get; set; }

        public virtual string Details { get; set; }

        public virtual DateTime EventTime { get; set; }
        [BsonRequired]
        public virtual string Message { get; set; }

        public int EventCode { get; set; }
        public int EventDetailCode { get; set; }
        public int EventSequence { get; set; }
        public int EventOccurrence { get; set; }
        public DateTime EventTimeUtc { get; set; }
        public string EventType { get; set; }
        public string ExceptionType { get; set; }
        public BsonValue EventID { get; set; }

        public bool Validate()
        {
            return (Message.Length > 0) && (ApplicationVirtualPath.Length > 0);
        }
    }
}
