using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HomeAutomation.DataAccess.UnitTest {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestDouble() {

            DataAccess.DatabaseFacade.DatabaseManager.InitializeDatabaseConnection("mongodb://localhost", "HomeAutomation");

            var refDocument = new MongoDB.Bson.ObjectId("53fc96c0f59e1008ace5649d");
            var query = MongoDB.Driver.Builders.Query<Entity.Component>.EQ(e=>e.Id, refDocument);
            var component = DataAccess.DatabaseFacade.DatabaseManager.Components.FindOne(query);

            component.MaxThreshold = 40;

            component.SetNewValue(50);
        }

        [TestMethod]
        public void TestInt() {

            DataAccess.DatabaseFacade.DatabaseManager.InitializeDatabaseConnection("mongodb://localhost", "HomeAutomation");

            var refDocument = new MongoDB.Bson.ObjectId("53ff1d3e55ed340d04e73091");
            var query = MongoDB.Driver.Builders.Query<Entity.Component>.EQ(e=>e.Id, refDocument);
            var component = DataAccess.DatabaseFacade.DatabaseManager.Components.FindOne(query);

            component.MinThreshold = 0;

            component.SetNewValue(0);
        }

        
    }
}
