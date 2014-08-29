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

        public String OwnerEmail { get; set; }

        public String Location { get; set; }

        public String IPAddress { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastModified { get; set; }

        public List<MongoDB.Driver.MongoDBRef> ComponentIds { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public List<Component> Components {
            get {
                if (ComponentIds == null) {
                    return new List<Component>();
                }
                return ComponentIds.Select(u => DatabaseFacade.DatabaseManager.Database.FetchDBRefAs<Component>(u)).ToList();
            }
            set {
                foreach (var component in value) {
                    if (component.Id.Pid == 0) {
                        DatabaseFacade.DatabaseManager.Components.Insert(component);
                    }
                }
                ComponentIds = value.Select(u => new MongoDB.Driver.MongoDBRef("Component", u.Id)).ToList();
            }
        }

        public Device() {
            ComponentIds = new List<MongoDB.Driver.MongoDBRef>();
        }

    }
}
