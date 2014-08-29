using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver.Linq;

namespace HomeAutomation.WebPortal.Controllers
{
    public class ConfigurationController : Controller
    {
        //
        // GET: /Configuration/

        public ActionResult Index()
        {
            var model = DataAccess.DatabaseFacade.DatabaseManager.Configurations.AsQueryable().ToList();
            return View(model);
        }

        //
        // POST: /Configuration/

        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                foreach(var key in collection.Keys){
                    var conf = DataAccess.DatabaseFacade.DatabaseManager.Configurations.AsQueryable().Single(u => u.Name == key.ToString());
                    conf.Value = collection[key.ToString()];

                    DataAccess.DatabaseFacade.DatabaseManager.Configurations.Save(conf);

                }
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        
    }
}
