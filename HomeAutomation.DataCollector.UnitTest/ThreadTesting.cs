using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HomeAutomation.DataCollector.UnitTest {
    [TestClass]
    public class ThreadTesting {
        [TestMethod]
        public void ThreadTest_Simple() {
            DataAccess.DatabaseFacade.DatabaseManager.InitializeDatabaseConnection("mongodb://localhost", "HomeAutomation");

            var DeviceMan = new DataCollector.Manager.DeviceManager();
            DeviceMan.StartDataCollecting();
            System.Threading.Thread.Sleep(120000);
            DeviceMan.StopDataCollecting();
        }

        [TestMethod]
        public void ThreadTest_Type() {
            var str = typeof(bool).ToString();
        }
    }
}
