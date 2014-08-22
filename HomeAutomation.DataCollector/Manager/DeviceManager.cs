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

        IGMPController PingQuery = new IGMPController();
        SNMPQueryController SNMPQuery = new SNMPQueryController();
        WMIQueryController WMIQuery = new WMIQueryController();

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
            //DataAccess.DatabaseFacade.DatabaseManager.InitializeDatabaseConnection(System.Configuration.ConfigurationManager.AppSettings["ConnectionString"], System.Configuration.ConfigurationManager.AppSettings["DbName"]);

            if (!DataAccess.DatabaseFacade.DatabaseManager.IsDeviceCollectionExist) {
                DataAccess.DatabaseFacade.DatabaseManager.InitializeDatabaseStructure();
            }
            
            while (!worker.CancellationPending) {
                //Manage Timer and Query thread
                var devices = DataAccess.DatabaseFacade.DatabaseManager.Devices.AsQueryable().ToList();
                var MinNextInterval = DateTime.Now.AddMinutes(10);

                foreach (var device in devices) {
                    foreach (var component in device.Components) {
                        if (component.NextContact < DateTime.Now) {
                            switch (component.Type.Category) {
                                case "ping":
                                    IGMPController pingner = new IGMPController();
                                    threadlock.WaitOne();
                                    component.NextContact = DateTime.Now.AddMinutes(Convert.ToDouble(component.Interval));
                                    pingner.StartPing(worker_RunWorkerCompleted, component);

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
