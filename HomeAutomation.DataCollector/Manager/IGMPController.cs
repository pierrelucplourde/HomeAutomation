using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace HomeAutomation.DataCollector.Manager {
    internal class IGMPController {

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

            if (component.Options != null) {
                if (component.Options.ContainsKey("Mode")) {
                    switch (component.Options["Mode"]) {
                        case "HostAlive" :
                            PingHostAlive(component);
                            break;
                        case "Delay" :
                            PingDelay(component);
                            
                            break;
                        default:
                            PingHostAlive(component);
                            break;
                    }
                } else {
                    PingHostAlive(component);
                }
            } else {
                PingHostAlive(component);
            }

            e.Result = component;
        }

        private void PingDelay(DataAccess.Entity.Component component) {
            var systemPing = new Ping();
            var reply = systemPing.Send(component.Device.IPAddress);
            var oldValue = component.CurrentValue;

            if (reply.Status == IPStatus.Success) {
                component.CurrentValue = reply.RoundtripTime;
                component.LastContact = DateTime.Now;
            } else {
                component.CurrentValue = -1;
                //component.LastContact = DateTime.Now;
            }

            CompressionManager.Instance.CompressPingDelay(component);

            DataAccess.DatabaseFacade.DatabaseManager.Components.Save(component);
        }

        private void PingHostAlive(DataAccess.Entity.Component component) {
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

            CompressionManager.Instance.CompressStandard(component);

            DataAccess.DatabaseFacade.DatabaseManager.Components.Save(component);
        }

    }
}
