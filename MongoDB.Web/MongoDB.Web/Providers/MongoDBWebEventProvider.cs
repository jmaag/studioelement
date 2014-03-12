using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web.Management;
using MongoDB.Driver;

namespace MongoDB.Web.Providers
{
    public class MongoDBWebEventProvider : BufferedWebEventProvider
    {
        private MongoCollection mongoCollection;

        public override void Initialize(string name, NameValueCollection config)
        {
            this.mongoCollection = MongoDatabase.Create(
                ConfigurationManager.ConnectionStrings[config["connectionStringName"] ?? "MongoConnection"].ConnectionString ?? "mongodb://localhost")
                .GetCollection(config["collection"] ?? "WebEvents");

            config.Remove("collection");
            config.Remove("connectionStringName");

            base.Initialize(name, config);
        }

        public override void ProcessEventFlush(WebEventBufferFlushInfo flushInfo)
        {
            this.mongoCollection.InsertBatch<WebEvent>(flushInfo.Events.Cast<WebBaseEvent>().ToList().ConvertAll<WebEvent>(WebEvent.FromWebBaseEvent));
        }
    }
}