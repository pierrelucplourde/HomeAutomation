using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver.Linq;

namespace HomeAutomation.WebPortal.Controllers {
    public class ComponentController : Controller {
        //
        // GET: /Component/

        public ActionResult Index() {
            return View();
        }

        //
        // GET: /Component/Details/5

        public ActionResult Details(String id) {
            var bSonId = new MongoDB.Bson.ObjectId(id);
            var query = Query<HomeAutomation.DataAccess.Entity.Component>.EQ(e => e.Id, bSonId);
            var model = DataAccess.DatabaseFacade.DatabaseManager.Components.FindOne(query);
            return View(model);

        }

        //
        // GET: /Component/Create

        public ActionResult Create(String id) {
            ViewData["DeviceId"] = id;
            ViewData["Type"] = DataAccess.DatabaseFacade.DatabaseManager.GetPollingSelectTypes().Select(u => new SelectListItem() { Text = u, Value = u }).ToList();

            return View();
        }

        [HttpPost]
        public JsonResult GetSubType(String type) {
            List<SelectListItem> subtype = DataAccess.DatabaseFacade.DatabaseManager.GetPollingSelectSubTypesByType(type).Select(u => new SelectListItem() { Text = u, Value = u }).ToList();

            //subtype.Insert(0,new SelectListItem())

            return Json(new SelectList(subtype, "Value", "Text"));
        }

        //
        // POST: /Component/Create

        [HttpPost]
        public ActionResult Create(String id, FormCollection collection) {
            try {
                // TODO: Add insert logic here
                DataAccess.Entity.Component nComponent = new DataAccess.Entity.Component();

                nComponent.Name = "New";

                //Get the associated device
                var bSonId = new MongoDB.Bson.ObjectId(id);
                var query = Query<HomeAutomation.DataAccess.Entity.Device>.EQ(e => e.Id, bSonId);
                nComponent.Device = DataAccess.DatabaseFacade.DatabaseManager.Devices.FindOne(query);

                nComponent.Type = DataAccess.DatabaseFacade.DatabaseManager.ComponentTypes.AsQueryable().Single(u => u.Category == collection["Type"] & u.Mode == collection["SubType"]);
                nComponent.Options = new Dictionary<string,string>(nComponent.Type.TemplateOptions);

                return View("Edit", nComponent);
            } catch {
                return View();
            }
        }

        //
        // GET: /Component/Edit/5

        public ActionResult Edit(String id) {
            var bSonId = new MongoDB.Bson.ObjectId(id);
            var query = Query<HomeAutomation.DataAccess.Entity.Component>.EQ(e => e.Id, bSonId);
            var model = DataAccess.DatabaseFacade.DatabaseManager.Components.FindOne(query);
            return View(model);
        }

        //
        // POST: /Component/Edit/5

        [HttpPost]
        public ActionResult Edit(String id, FormCollection collection) {
            try {
                // TODO: Add update logic here

                return RedirectToAction("Details", new { id = id });
            } catch {
                return View();
            }
        }

        //
        // GET: /Component/Delete/5

        public ActionResult Delete(int id) {
            return View();
        }

        //
        // POST: /Component/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection) {
            try {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            } catch {
                return View();
            }
        }


    }
}
