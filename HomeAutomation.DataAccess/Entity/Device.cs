using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeAutomation.DataAccess.Entity {
    public class Device {

        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public String Description { get; set; }

        public String Location { get; set; }

        public DateTime LastModified { get; set; }

        public List<Component> Components { get; set; }
    }
}
