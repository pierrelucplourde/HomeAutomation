using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeAutomation.WebPortal {
    public static class DBConfig {

        public static void DatabaseConfigure() {
            DataAccess.DatabaseFacade.DatabaseManager.InitializeDatabaseConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["HomeAutomationMongoDB"].ConnectionString, System.Web.Configuration.WebConfigurationManager.AppSettings["DatabaseName"]);            
        }
    }
}