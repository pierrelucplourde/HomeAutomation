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
                nComponent.IsActive = false;

                DataAccess.DatabaseFacade.DatabaseManager.Components.Insert(nComponent);

                var device = nComponent.Device;
                device.ComponentIds.Add(new MongoDB.Driver.MongoDBRef("Component", nComponent.Id));

                DataAccess.DatabaseFacade.DatabaseManager.Devices.Save(device);

                return RedirectToAction("Edit", new { id = nComponent.Id });
                //return View("Edit", nComponent);
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
                var modComponent = DataAccess.DatabaseFacade.DatabaseManager.Components.AsQueryable().SingleOrDefault(u => u.Id == new MongoDB.Bson.ObjectId(id));
                
                // TODO: Add update logic here
                if (modComponent != null) {
                    foreach (var key in collection.Keys) {
                        var keyStr = key.ToString();
                        switch (keyStr) {
                            case "Name" :
                                modComponent.Name = collection[keyStr];
                                break;
                                case "Description" :
                                modComponent.Description = collection[keyStr];
                                break;
                                case "Compression" :
                                try {
                                    modComponent.Compression = Convert.ToDecimal(collection[keyStr], System.Globalization.CultureInfo.CurrentCulture);
                                } catch {
                                    try {
                                        modComponent.Compression = Convert.ToDecimal(collection[keyStr], System.Globalization.CultureInfo.InvariantCulture);
                                    } catch {
                                        modComponent.Compression = 0;
                                    }
                                }
                                break;
                                case "Interval" :
                                modComponent.Interval = Convert.ToInt32(collection[keyStr]);
                                break;
                                case "IsActive" :
                                modComponent.IsActive = Convert.ToBoolean(collection[keyStr].Split(',').First());
                                break;
                            default:
                                if (modComponent.Options == null) {
                                    modComponent.Options = new Dictionary<string, string>();
                                }
                                if (modComponent.Options.ContainsKey(keyStr)) {
                                    modComponent.Options[keyStr] = collection[keyStr];
                                } else { 
                                modComponent.Options.Add(keyStr, collection[keyStr]);
                                }
                                break;
                        }

                    }
                    DataAccess.DatabaseFacade.DatabaseManager.Components.Save(modComponent);
                }

                return RedirectToAction("Details", new { id = id });
                
            } catch(Exception ex){
                return RedirectToAction("Edit", new { id = id });
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
