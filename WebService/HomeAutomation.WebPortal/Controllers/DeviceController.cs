using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Builders;

namespace HomeAutomation.WebPortal.Controllers
{
    public class DeviceController : Controller
    {
        //
        // GET: /Device/

        public ActionResult Index()
        {
            var data = DataAccess.DatabaseFacade.DatabaseManager.Devices.AsQueryable().ToList();


            return View(data);
        }

        //
        // GET: /Device/ListComponent/5

        public ActionResult ListComponent(String id)
        {
            //var model = DataAccess.DatabaseFacade.DatabaseManager.Devices.AsQueryable().SingleOrDefault(u => u.Id.Equals(id));
            //var model = DataAccess.DatabaseFacade.DatabaseManager.Devices.FindOneAs<DataAccess.Entity.Device>(new MongoDB.Driver.FindOneArgs(){F})
            var bSonId = new MongoDB.Bson.ObjectId(id);
            var query = Query<HomeAutomation.DataAccess.Entity.Device>.EQ(e => e.Id, bSonId);
            var model = DataAccess.DatabaseFacade.DatabaseManager.Devices.FindOne(query);
            return View(model);
        }

        //
        // GET: /Device/Create

        public ActionResult Create()
        {
            var nDevice = new DataAccess.Entity.Device();
            
            return View("Edit",nDevice);
        }

        //
        // POST: /Device/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                var nDevice = new DataAccess.Entity.Device();

                nDevice.Name = collection["Name"];
                nDevice.Description = collection["Description"];
                nDevice.Location = collection["Location"];
                nDevice.IPAddress = collection["IPAddress"];
                nDevice.OwnerEmail = collection["OwnerEmail"];

                DataAccess.DatabaseFacade.DatabaseManager.Devices.Insert(nDevice);

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /Device/Edit/5

        public ActionResult Edit(String id)
        {
            var bSonId = new MongoDB.Bson.ObjectId(id);
            var query = Query<HomeAutomation.DataAccess.Entity.Device>.EQ(e => e.Id, bSonId);
            var Device = DataAccess.DatabaseFacade.DatabaseManager.Devices.FindOne(query);

            return View(Device);
        }

        //
        // POST: /Device/Edit/5

        [HttpPost]
        public ActionResult Edit(String id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                var bSonId = new MongoDB.Bson.ObjectId(id);
                var query = Query<HomeAutomation.DataAccess.Entity.Device>.EQ(e => e.Id, bSonId);
                var nDevice = DataAccess.DatabaseFacade.DatabaseManager.Devices.FindOne(query);

                nDevice.Name = collection["Name"];
                nDevice.Description = collection["Description"];
                nDevice.Location = collection["Location"];
                nDevice.IPAddress = collection["IPAddress"];
                nDevice.OwnerEmail = collection["OwnerEmail"];

                DataAccess.DatabaseFacade.DatabaseManager.Devices.Save(nDevice);

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /Device/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Device/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
