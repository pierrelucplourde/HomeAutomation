using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Linq;

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

        public bool IsDeviceCollectionExist {
            get {
                if (Database != null) {
                    return Database.CollectionExists("Device");
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

        public void InitializeDatabaseStructure() {
            if (!DataAccess.DatabaseFacade.DatabaseManager.IsDeviceCollectionExist) {
                var nDevice = new Entity.Device() {
                    Name = "Localhost",
                    Description = "LocalComputer",
                    LastModified = DateTime.Now,
                    IPAddress = "127.0.0.1",
                    Components = new List<Entity.Component>() { new Entity.Component() { 
                    Type = new Entity.ComponentType() { Category = "ping", TemplateOptions = new Dictionary<string,string>(){{"Mode","HostAlive"}} },
                    Compression = 0,
                    IsActive = true,
                    ValueType = typeof(bool).ToString(),
                    CurrentValue = 0,
                    Interval = 5,
                    Name = "Host Alive"
                    }
                }
                };

                Devices.Insert(nDevice);
                foreach (var item in nDevice.Components) {
                    item.Device = nDevice;
                    Components.Save(item);
                }
            }

            //Check and create other module
            //Each new query module has to be inserted here to be registered in the database
            if (!ComponentTypes.AsQueryable().Any(u=> u.CheckOptionsExist("Mode","Delay") & u.Category == "ping")) {
                var nType = new Entity.ComponentType() { Category = "ping", TemplateOptions = new Dictionary<string, string>() { { "Mode", "Delay" } } };

                ComponentTypes.Insert(nType);
            }

        }


    }
}
