using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeAutomation.DataAccess.Entity {
    public class ComponentValueHistory {

        //[MongoDB.Bson.Serialization.Attributes.BsonId]
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public ObjectId Id { get; set; }

        public MongoDB.Driver.MongoDBRef ComponentId { get; set; }

        //[MongoDB.Bson.Serialization.Attributes.BsonId]
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime TimeStamp { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public Entity.Component Component {
            get {
                return DatabaseFacade.DatabaseManager.Database.FetchDBRefAs<Component>(ComponentId);
            }
            set {
                ComponentId = new MongoDB.Driver.MongoDBRef("Component", value.Id);
            }
        }

        public Object Value { get; set; }
    }
}
