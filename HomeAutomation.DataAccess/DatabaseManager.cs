using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeAutomation.DataAccess {
    public class DatabaseFacade {

        MongoDB.Driver.MongoClient client;
        MongoDB.Driver.MongoServer server;

        private static DatabaseFacade _DatabaseManager;
        public static DatabaseFacade DatabaseManager {
            get {
                if (_DatabaseManager == null) {
                    _DatabaseManager = new DatabaseFacade();
                }

                return _DatabaseManager;
            }
            private set {
                _DatabaseManager = value;
            }
        }

        private MongoDB.Driver.MongoDatabase _Database;
        public MongoDB.Driver.MongoDatabase Database {
            get {
                return _Database;
            }
            private set {
                _Database = value;
            }
        }

        

        public MongoDB.Driver.MongoCollection<Entity.User> Users {
            get {
                if (Database != null) {
                    return Database.GetCollection<Entity.User>("User");
                }
                return null; 
            }
        }

        public MongoDB.Driver.MongoCollection<Entity.Device> Devices {
            get {
                if (Database != null) {
                    return Database.GetCollection<Entity.Device>("Device");
                }
                return null;
            }
        }

        public MongoDB.Driver.MongoCollection<Entity.Component> Components {
            get {
                if (Database != null) {
                    return Database.GetCollection<Entity.Component>("Component");
                }
                return null;
            }
        }

        public MongoDB.Driver.MongoCollection<Entity.ComponentType> ComponentTypes {
            get {
                if (Database != null) {
                    return Database.GetCollection<Entity.ComponentType>("ComponentType");
                }
                return null;
            }
        }

        public MongoDB.Driver.MongoCollection<Entity.ComponentValueHistory> ComponentValueHistory {
            get {
                if (Database != null) {
                    return Database.GetCollection<Entity.ComponentValueHistory>("ComponentValueHistory");
                }
                return null;
            }
        }

        public bool IsUserCollectionExist {
            get {
                if (Database != null) {
                    return Database.CollectionExists("User");
                }
                return false;
            }
        }

        public DatabaseFacade() {
            //InitializeDatabaseConnection("mongodb://localhost", "HomeAutomation");
        }

        public void InitializeDatabaseConnection(String connectionString) {
            InitializeDatabaseConnection(connectionString, "HomeAutomation");
        }

        public void InitializeDatabaseConnection(String connectionString, String dbname) {
            client = new MongoDB.Driver.MongoClient(connectionString);
            server = client.GetServer();
            Database = server.GetDatabase(dbname);
        }




    }
}
