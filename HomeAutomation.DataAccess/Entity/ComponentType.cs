using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeAutomation.DataAccess.Entity {
    public class ComponentType {

        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public ObjectId Id { get; set; }

        public String Name { get; set; }

        public String Category { get; set; }

        public Dictionary<String, String> TemplateOptions { get; set; }
    }
}
