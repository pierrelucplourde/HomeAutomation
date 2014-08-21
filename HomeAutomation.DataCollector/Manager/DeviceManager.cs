using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MongoDB.Driver.Linq;
using System.Threading;

namespace HomeAutomation.DataCollector.Manager {
    class DeviceManager {
        int MaxThread = 10;

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
            threadlock = new Semaphore(0, MaxThread);
            worker.RunWorkerAsync();
        }

        public void StopDataCollecting() {
            worker.CancelAsync();
            while (worker.IsBusy) {
                
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e) {
            //Configure database connection
            DataAccess.DatabaseFacade.DatabaseManager.InitializeDatabaseConnection(System.Configuration.ConfigurationManager.AppSettings["ConnectionString"], System.Configuration.ConfigurationManager.AppSettings["DbName"]);
            while (worker.CancellationPending) {
                //Manage Timer and Query thread
                var devices = DataAccess.DatabaseFacade.DatabaseManager.Devices.AsQueryable().ToList();

                foreach (var device in devices) {
                    foreach (var component in device.Components) {
                        switch (component.Type.Category) {
                            case "ping":

                                break;
                            default:
                                break;
                        }
                    }
                }

            }

            //Close database connection
            DataAccess.DatabaseFacade.DatabaseManager.Database.Server.Disconnect();
        }


    }
}
