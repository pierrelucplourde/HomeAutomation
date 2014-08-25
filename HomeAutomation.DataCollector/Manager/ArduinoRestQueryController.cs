using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace HomeAutomation.DataCollector.Manager {
    internal class ArduinoRestQueryController {

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

        internal ArduinoRestQueryController() {

        }

        internal void StartQuery(RunWorkerCompletedEventHandler EndPingEventHandler, DataAccess.Entity.Component component) {
            worker = new BackgroundWorker();
            worker.RunWorkerCompleted += EndPingEventHandler;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync(component);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e) {
            var component = (DataAccess.Entity.Component)e.Argument;

            //Process the Arduino query


            e.Result = component;
        }
    }
}
