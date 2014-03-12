using System.Collections.Generic;
using System.Linq;
using SquareHook.Membership.Data.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace SquareHook.Membership.Data.Controllers
{
    /// <summary>
    /// Conceals the methods needed by the main application to access the WebEvents in the database
    /// </summary>
    public class WebEventsCollection
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="webEvents">WebEvents in the db</param>
        public WebEventsCollection(MongoCollection<WebEvent> webEvents)
        {
            this._webEvents = webEvents;
        }

        /// <summary>
        /// WebEvents objects accessible for class using this one as base
        /// </summary>
        private MongoCollection<WebEvent> _webEvents = null;

        protected MongoCollection<WebEvent> WebEvents
        {
            get
            {
                return this._webEvents;
            }
        }

        /// <summary>
        /// Inserts a new webEvent data in the db
        /// </summary>
        /// <param name="webEvent">WebEvent info. to be added</param>
        /// <returns>Result of the operation, true if was ok</returns>
        public bool Insert(WebEvent webEvent)
        {
            SafeModeResult result = WebEvents.Insert(webEvent, SafeMode.True);
            return result.Ok;
        }

        /// <summary>
        /// Updates the info of a webEvent...like a chnage of name, but isn't being used in the app...
        /// </summary>
        /// <param name="webEvent">WebEvent to be updated</param>
        /// <returns>Result of the operation, true if was ok</returns>
        public bool Update(WebEvent webEvent)
        {
            if (webEvent.Validate())
            {
                SafeModeResult result = WebEvents.Save(webEvent, SafeMode.True);
                return result.Ok;
            }
            else return false;
        }

        /// <summary>
        /// Deletes the webEvent with the specified id
        /// </summary>
        /// <param name="webEventId">WebEvent to be deleted id</param>
        /// <returns>Result of the operation, true if was ok</returns>
        public bool Delete(ObjectId webEventId)
        {
            FindAndModifyResult result = WebEvents.FindAndRemove(Query.EQ("_id", webEventId), SortBy.Ascending("_id"));
            return result.Ok;
        }

        public bool DeleteAll()
        {
            SafeModeResult result = WebEvents.RemoveAll();
            return true;
        }

        /// <summary>
        /// Searches for all webEvents
        /// </summary>
        /// <returns>List of found webEvents</returns>
        public List<T> GetWebEvents<T>()
        {
            MongoCursor<T> result = WebEvents.FindAllAs<T>();
            if (result != null)
                return result.ToList();
            else
                return null;
        }

        /// <summary> 
        /// Searches for the webEvent with the specified Id
        /// </summary>
        /// <param name="webEventId">Id of the webEvent to be searched</param>
        /// <returns>What was found</returns>
        public T GetWebEvent<T>(ObjectId webEventId)
        {
            return WebEvents.FindOneByIdAs<T>(webEventId);
        }
    }
}
