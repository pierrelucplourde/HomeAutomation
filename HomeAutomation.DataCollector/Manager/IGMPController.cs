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
            try { 
            var component = (DataAccess.Entity.Component)e.Argument;

            if (component.Type.Mode != null) {
                switch (component.Type.Mode.ToLower()) {
                        case "hostalive" :
                            PingHostAlive(component);
                            break;
                        case "delay" :
                            PingDelay(component);
                            
                            break;
                        default:
                            PingHostAlive(component);
                            break;
                    }
               
            } else {
                PingHostAlive(component);
            }

            e.Result = component;
            } catch (Exception ex) {
                var logFile = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "error.log", true);
                logFile.WriteLine(ex.ToString());
                logFile.Close();
            }
        }

        private void PingDelay(DataAccess.Entity.Component component) {
            var systemPing = new Ping();
            var reply = systemPing.Send(component.Device.IPAddress);
            var oldValue = component.CurrentValue;

            if (reply.Status == IPStatus.Success) {
                
                component.SetNewValue( reply.RoundtripTime);
                component.LastContact = DateTime.Now;
            } else {
                component.SetNewValue( -1);
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
                component.SetNewValue(1);
                component.LastContact = DateTime.Now;
            } else {
                component.SetNewValue(0);
                //component.LastContact = DateTime.Now;
            }

            CompressionManager.Instance.CompressStandard(component);

            DataAccess.DatabaseFacade.DatabaseManager.Components.Save(component);

        }

    }
}
