using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Text;

namespace HomeAutomation.DataCollector.Manager {
    internal class WMIQueryController {

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

        internal WMIQueryController() {

        }

        internal void StartQuery(RunWorkerCompletedEventHandler EndPingEventHandler, DataAccess.Entity.Component component) {
            worker = new BackgroundWorker();
            worker.RunWorkerCompleted += EndPingEventHandler;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync(component);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e) {
            var component = (DataAccess.Entity.Component)e.Argument;

            //Process the WMI query
            if (component.Options != null) {
                if (component.Options.ContainsKey("Mode")) {
                    switch (component.Options["Mode"]) {
                        case "DiskSpaceLeft":
                            GetDiskSpaceLeft(component);
                            break;
                        case "DiskPercentageLeft":
                            GetPercentageLeft(component);
                            break;
                        case "MemSpaceLeft":
                            GetMemSpaceLeft(component);
                            break;
                        case "MemPercentageLeft":
                            GetMemPercentageLeft(component);
                            break;
                        case "CpuUsage":
                            GetCPUUsage(component);
                            break;
                        case "Custom":
                            GetCustomProp(component);
                            break;
                        default:
                            break;
                    }
                }
            }

            e.Result = component;
        }

        private void GetCustomProp(DataAccess.Entity.Component component) {
            throw new NotImplementedException();
        }

        private void GetCPUUsage(DataAccess.Entity.Component component) {
            throw new NotImplementedException();
        }

        private void GetMemPercentageLeft(DataAccess.Entity.Component component) {
            throw new NotImplementedException();
        }

        private void GetMemSpaceLeft(DataAccess.Entity.Component component) {
            throw new NotImplementedException();
        }

        private void GetPercentageLeft(DataAccess.Entity.Component component) {
            throw new NotImplementedException();
        }

        private void GetDiskSpaceLeft(DataAccess.Entity.Component component) {
            if (component.Options.ContainsKey("Disk")) {
                var DiskToSearch = component.Options["Disk"];
                var oldValue = component.CurrentValue;

                ConnectionOptions options = new ConnectionOptions();
                options.Username = component.Options["User"];
                options.Password = component.Options["Password"]; //TODO Encrypt Password
                ManagementScope scope = new ManagementScope(@"\\" + component.Device.IPAddress + @"\root\cimv2", options);
                SelectQuery query = new SelectQuery();
                query.QueryString = "SELECT * FROM Win32_LogicalDisk";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection queryCollection = searcher.Get();
                foreach (ManagementObject m in queryCollection) {
                    if (m.Properties["DeviceID"].Value == DiskToSearch) {
                        component.CurrentValue = Convert.ToDouble(m.Properties["FreeSpace"]);
                        component.LastContact = DateTime.Now;
                        break;
                    }
                }

                //TODO - PLP - Compress
            }
        }
    }
}
