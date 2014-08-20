using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;

namespace HomeAutomation.WebPortal.Helpers {
    public class AuthProvider : SimpleMembershipProvider {
        public override MembershipUser GetUser(string username, bool userIsOnline) {
            return base.GetUser(username, userIsOnline);
        }

        public override bool ValidateUser(string username, string password) {
            using(var db = new Models.EntitiesContainer()){
                return db.Users.SingleOrDefault(u=> u.Username == username && u.Password == password)!= null; // base.ValidateUser(username, password);
            }
        }
    }
}