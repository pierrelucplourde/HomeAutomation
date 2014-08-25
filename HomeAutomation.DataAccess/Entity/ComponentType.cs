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

        public String Mode { get; set; }

        public Dictionary<String, String> TemplateOptions { get; set; }

        internal bool CheckOptionsExist(string key, string value) {
            if (TemplateOptions != null) {
                if (TemplateOptions.ContainsKey(key)) {
                    return TemplateOptions[key] == value;
                }
            }

            return false;
        }
    }
}
