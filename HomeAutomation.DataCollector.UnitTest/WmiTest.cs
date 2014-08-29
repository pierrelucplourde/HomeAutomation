using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver.Builders;

namespace HomeAutomation.DataCollector.Manager {
    [TestClass]
    public class WmiTest {
        [TestMethod]
        public void TestWMIDiskSpace() {
            DataAccess.DatabaseFacade.DatabaseManager.InitializeDatabaseConnection("mongodb://localhost", "HomeAutomation");

            var refDocument = new MongoDB.Bson.BsonDocument { 
                {"$ref", "Component"}, 
                {"$id", new MongoDB.Bson.ObjectId("53fc96c0f59e1008ace5649d")} 
                };
            var query = Query.EQ("ComponentId", refDocument);
            var test = query.ToString();
            var values = DataAccess.DatabaseFacade.DatabaseManager.ComponentValueHistory.Find(query);
            foreach (var item in values) {
                Console.WriteLine(item.Value);
            }
        }
    }
}
