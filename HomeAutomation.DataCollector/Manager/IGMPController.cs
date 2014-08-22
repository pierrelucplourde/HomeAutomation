using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace HomeAutomation.DataCollector.Manager {
    class IGMPController {

        BackgroundWorker worker;

        public event RunWorkerCompletedEventHandler RunWorkerCompleted {
            add {
                if (worker != null) {
                    lock (worker) {
                        worker.RunWorkerCompleted += value;
                    }
                }
            }
            remove {
                if (worker != null) {
                    lock (worker) {
                        worker.RunWorkerCompleted -= value;
                    }
                }
            }
        }

        public IGMPController() {

        }

        public void StartPing(RunWorkerCompletedEventHandler EndPingEventHandler, DataAccess.Entity.Component component) {
            worker = new BackgroundWorker();
            worker.RunWorkerCompleted += EndPingEventHandler;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync(component);
        }

        void worker_DoWork(object sender, DoWorkEventArgs e) {
            var component = (DataAccess.Entity.Component)e.Argument;

            var systemPing = new Ping();
            var reply = systemPing.Send(component.Device.IPAddress);
            var oldValue = component.CurrentValue;

            if (reply.Status == IPStatus.Success) {
                component.CurrentValue = 1;
                component.LastContact = DateTime.Now;
            } else {
                component.CurrentValue = 0;
                //component.LastContact = DateTime.Now;
            }

            //If something change let's archive it
            if (component.CurrentValue != oldValue) {
                var nHistory = new DataAccess.Entity.ComponentValueHistory() {
                    Component = component,
                    TimeStamp = DateTime.Now,
                    Value = component.CurrentValue
                };
                DataAccess.DatabaseFacade.DatabaseManager.ComponentValueHistory.Insert(nHistory);
            }

            DataAccess.DatabaseFacade.DatabaseManager.Components.Save(component);

            e.Result = component;
        }


    }
}
