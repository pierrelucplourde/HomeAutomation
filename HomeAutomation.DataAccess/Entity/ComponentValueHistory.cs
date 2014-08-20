using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeAutomation.DataAccess.Entity {
    public class ComponentValueHistory {

        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public ObjectId ComponentId { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public DateTime TimeStamp { get; set; }

        public Object Value { get; set; }
    }
}
