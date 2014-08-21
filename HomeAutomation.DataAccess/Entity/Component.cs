using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeAutomation.DataAccess.Entity {
    public class Component {

        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public ObjectId Id { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Compression { get; set; }

        public decimal Interval { get; set; }

        public DateTime LastContact { get; set; }

        public Type ValueType { get; set; }

        public Object CurrentValue { get; set; }

        public bool IsActive { get; set; }

        public Dictionary<String,String> Options { get; set; }

        public ComponentType Type { get; set; }

    }
}
