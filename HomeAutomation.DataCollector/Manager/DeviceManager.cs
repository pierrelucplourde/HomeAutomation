using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MongoDB.Driver.Linq;
using System.Threading;

namespace HomeAutomation.DataCollector.Manager {
    public class DeviceManager {
        int MaxThread = 2;

        BackgroundWorker worker;
        Semaphore threadlock;

        public DeviceManager() {

        }

        public void StartDataCollecting() {
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.WorkerSupportsCancellation = true;
            threadlock = new Semaphore(1, MaxThread);
            worker.RunWorkerAsync();
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            threadlock.Release();
        }

        public void StopDataCollecting() {
            worker.CancelAsync();

            while (worker.IsBusy) {

            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e) {
            //Configure database connection
            DataAccess.DatabaseFacade.DatabaseManager.InitializeDatabaseConnection(System.Configuration.ConfigurationManager.AppSettings["ConnectionString"], System.Configuration.ConfigurationManager.AppSettings["DbName"]);


            DataAccess.DatabaseFacade.DatabaseManager.InitializeDatabaseStructure();


            while (!worker.CancellationPending) {
                //Manage Timer and Query thread
                var devices = DataAccess.DatabaseFacade.DatabaseManager.Devices.AsQueryable().ToList();
                var MinNextInterval = DateTime.Now.AddMinutes(10);

                foreach (var device in devices) {
                    if (device.Components != null) {
                        foreach (var component in device.Components) {
                            if (component.NextContact < DateTime.Now & component.IsActive) {
                                //Decide which poller to use
                                switch (component.Type.Category.ToLower()) {
                                    case "ping":
                                        IGMPController pingner = new IGMPController();
                                        threadlock.WaitOne();
                                        component.NextContact = DateTime.Now.AddMinutes(Convert.ToDouble(component.Interval));
                                        pingner.StartPing(worker_RunWorkerCompleted, component);

                                        break;

                                    case "wmi":
                                        WMIQueryController wmiCtl = new WMIQueryController();
                                        threadlock.WaitOne();
                                        component.NextContact = DateTime.Now.AddMinutes(Convert.ToDouble(component.Interval));
                                        wmiCtl.StartQuery(worker_RunWorkerCompleted, component);

                                        break;

                                    case "arduino":
                                        ArduinoRestQueryController ardCtl = new ArduinoRestQueryController();
                                        threadlock.WaitOne();
                                        component.NextContact = DateTime.Now.AddMinutes(Convert.ToDouble(component.Interval));
                                        ardCtl.StartQuery(worker_RunWorkerCompleted, component);

                                        break;

                                    case "snmp":
                                        SNMPQueryController snmpCtl = new SNMPQueryController();
                                        threadlock.WaitOne();
                                        component.NextContact = DateTime.Now.AddMinutes(Convert.ToDouble(component.Interval));
                                        snmpCtl.StartQuery(worker_RunWorkerCompleted, component);

                                        break;
                                    default:
                                        break;
                                }
                                if (component.NextContact < MinNextInterval) {
                                    MinNextInterval = component.NextContact;
                                }
                            }
                        }
                    }
                }

                var spanToWait = MinNextInterval - DateTime.Now;
                while (MinNextInterval > DateTime.Now & !worker.CancellationPending) {
                    Thread.Sleep(10000);
                }


            }

            //Close database connection
            DataAccess.DatabaseFacade.DatabaseManager.Database.Server.Disconnect();
        }


    }
}
