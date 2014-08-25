using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeAutomation.DataAccess.Entity {
    public class User {

        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

    }
}
