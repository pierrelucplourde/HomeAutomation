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

        public String ValueType { get; set; }

        public Object CurrentValue { get; set; }
        
        public DateTime LastContact { get; set; }
        
        public Object LastHistoricalValue { get; set; }

        public DateTime LastHistoricalContact { get; set; }

        public bool IsActive { get; set; }

        public Dictionary<String,String> Options { get; set; }

        public MongoDB.Driver.MongoDBRef TypeId { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public ComponentType Type {
            get {
                return DatabaseFacade.DatabaseManager.Database.FetchDBRefAs<ComponentType>(TypeId);
            }
            set {
                if (value.Id.Pid == 0) {
                    DatabaseFacade.DatabaseManager.ComponentTypes.Insert(value);
                }
                
                TypeId = new MongoDB.Driver.MongoDBRef("ComponentType", value.Id);
            }
        }

        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public DateTime NextContact { get; set; }

        public MongoDB.Driver.MongoDBRef DeviceId { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public Device Device {
            get {
                return DatabaseFacade.DatabaseManager.Database.FetchDBRefAs<Device>(DeviceId);
            }
            set {
                if (value.Id.Pid == 0) {
                    DatabaseFacade.DatabaseManager.Devices.Insert(value);
                }

                DeviceId = new MongoDB.Driver.MongoDBRef("Device", value.Id);
            }
        }

    }
}
