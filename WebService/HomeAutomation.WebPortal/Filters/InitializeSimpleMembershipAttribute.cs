using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using WebMatrix.WebData;
using HomeAutomation.WebPortal.Models;

namespace HomeAutomation.WebPortal.Filters {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            // Ensure ASP.NET Simple Membership is initialized only once per app start
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        private class SimpleMembershipInitializer {
            public SimpleMembershipInitializer() {
                

                try {

                    if (DataAccess.DatabaseFacade.DatabaseManager.Database == null) {
                        DataAccess.DatabaseFacade.DatabaseManager.InitializeDatabaseConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["HomeAutomationMongoDB"].ConnectionString, System.Web.Configuration.WebConfigurationManager.AppSettings["DatabaseName"]); 
                    }
                    
                    if (!DataAccess.DatabaseFacade.DatabaseManager.IsUserCollectionExist) {
                        DataAccess.DatabaseFacade.DatabaseManager.Users.Insert(new DataAccess.Entity.User() { Name = "Administrator", UserName = "admin", Password = "admin" });
                    }

                    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
                } catch (Exception ex) {
                    throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
            }
        }
    }
}
