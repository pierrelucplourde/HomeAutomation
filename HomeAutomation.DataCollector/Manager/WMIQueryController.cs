﻿using System;
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
            if (component.Type.Mode != null) {
                switch (component.Type.Mode.ToLower()) {
                    case "diskspaceleft":
                        GetDiskSpaceLeft(component);
                        break;
                    case "diskpercentageleft":
                        GetPercentageLeft(component);
                        break;
                    case "memspaceleft":
                        GetMemSpaceLeft(component);
                        break;
                    case "mempercentageleft":
                        GetMemPercentageLeft(component);
                        break;
                    case "cpuusage":
                        GetCPUUsage(component);
                        break;
                    case "custom":
                        GetCustomProp(component);
                        break;
                    default:
                        break;
                }
            }
            e.Result = component;
        }

        private void GetCustomProp(DataAccess.Entity.Component component) {
            throw new NotImplementedException();
        }

        private void GetCPUUsage(DataAccess.Entity.Component component) {
            var oldValue = component.CurrentValue;

            ConnectionOptions options = new ConnectionOptions();
            options.Username = component.Options["User"];
            options.Password = component.Options["Password"]; //TODO Encrypt Password
            ManagementScope scope = new ManagementScope(@"\\" + component.Device.IPAddress + @"\root\cimv2", options);
            SelectQuery query = new SelectQuery();
            query.QueryString = "SELECT * FROM Win32_Processor";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection queryCollection = searcher.Get();
            foreach (ManagementObject m in queryCollection) {
                component.CurrentValue = Convert.ToDouble(m.Properties["LoadPercentage"].Value);
                component.LastContact = DateTime.Now;
                break;
            }
            CompressionManager.Instance.CompressStandard(component);
            DataAccess.DatabaseFacade.DatabaseManager.Components.Save(component);
        }

        private void GetMemPercentageLeft(DataAccess.Entity.Component component) {

            var oldValue = component.CurrentValue;

            ConnectionOptions options = new ConnectionOptions();
            options.Username = component.Options["User"];
            options.Password = component.Options["Password"]; //TODO Encrypt Password
            ManagementScope scope = new ManagementScope(@"\\" + component.Device.IPAddress + @"\root\cimv2", options);
            SelectQuery query = new SelectQuery();
            query.QueryString = "SELECT * FROM Win32_OperatingSystem";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection queryCollection = searcher.Get();
            foreach (ManagementObject m in queryCollection) {
                component.CurrentValue = Convert.ToDouble(m.Properties["FreePhysicalMemory"].Value) / Convert.ToDouble(m.Properties["TotalVisibleMemorySize"].Value) * 100;
                component.LastContact = DateTime.Now;
                break;
            }
            CompressionManager.Instance.CompressStandard(component);
            DataAccess.DatabaseFacade.DatabaseManager.Components.Save(component);
        }

        private void GetMemSpaceLeft(DataAccess.Entity.Component component) {
            var oldValue = component.CurrentValue;

            ConnectionOptions options = new ConnectionOptions();
            options.Username = component.Options["User"];
            options.Password = component.Options["Password"]; //TODO Encrypt Password
            ManagementScope scope = new ManagementScope(@"\\" + component.Device.IPAddress + @"\root\cimv2", options);
            SelectQuery query = new SelectQuery();
            query.QueryString = "SELECT * FROM Win32_OperatingSystem";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection queryCollection = searcher.Get();
            foreach (ManagementObject m in queryCollection) {

                component.CurrentValue = Convert.ToDouble(m.Properties["FreePhysicalMemory"].Value);
                component.LastContact = DateTime.Now;
                if (component.Options.ContainsKey("Unit")) {
                    switch (component.Options["Unit"]) {
                        case "KB":
                            component.CurrentValue = Convert.ToDouble(component.CurrentValue);
                            break;
                        case "MB":
                            component.CurrentValue = Convert.ToDouble(component.CurrentValue) / 1024;
                            break;
                        case "GB":
                            component.CurrentValue = Convert.ToDouble(component.CurrentValue) / 1024 / 1024;
                            break;
                        case "TB":
                            component.CurrentValue = Convert.ToDouble(component.CurrentValue) / 1024 / 1024 / 1024;
                            break;
                        default:
                            break;
                    }
                }
                break;
            }
            CompressionManager.Instance.CompressStandard(component);
            DataAccess.DatabaseFacade.DatabaseManager.Components.Save(component);
        }

        private void GetPercentageLeft(DataAccess.Entity.Component component) {
            if (component.Options.ContainsKey("Disk")) {
                var DiskToSearch = component.Options["Disk"];
                var oldValue = component.CurrentValue;
                ConnectionOptions options = null;

                if (component.Options.ContainsKey("User")) {
                    if (!String.IsNullOrEmpty( component.Options["User"])) {
                        options = new ConnectionOptions();
                        options.Username = component.Options["User"];
                        options.Password = component.Options["Password"]; //TODO Encrypt Password 
                    }
                }
                ManagementScope scope;
                if (options != null) {
                    scope = new ManagementScope(@"\\" + component.Device.IPAddress + @"\root\cimv2", options);
                } else {
                    scope = new ManagementScope(@"\\" + component.Device.IPAddress + @"\root\cimv2");

                }
                SelectQuery query = new SelectQuery();
                query.QueryString = "SELECT * FROM Win32_LogicalDisk";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection queryCollection = searcher.Get();
                foreach (ManagementObject m in queryCollection) {
                    if ((string)m.Properties["DeviceID"].Value == DiskToSearch) {
                        component.CurrentValue = Convert.ToDouble(m.Properties["FreeSpace"].Value) / Convert.ToDouble(m.Properties["Size"].Value) * 100;
                        component.LastContact = DateTime.Now;
                        break;
                    }
                }
                CompressionManager.Instance.CompressStandard(component);
                DataAccess.DatabaseFacade.DatabaseManager.Components.Save(component);
            }
        }

        private void GetDiskSpaceLeft(DataAccess.Entity.Component component) {
            if (component.Options.ContainsKey("Disk")) {
                var DiskToSearch = component.Options["Disk"];
                var oldValue = component.CurrentValue;

                ConnectionOptions options = new ConnectionOptions();
                options.Username = component.Options["User"];
                options.Password = component.Options["Password"]; //TODO Encrypt Password
                ManagementScope scope = new ManagementScope(@"\\" + component.Device.IPAddress + @"\root\cimv2");
                SelectQuery query = new SelectQuery();
                query.QueryString = "SELECT * FROM Win32_LogicalDisk";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection queryCollection = searcher.Get();
                foreach (ManagementObject m in queryCollection) {
                    if ((string)m.Properties["DeviceID"].Value == DiskToSearch) {
                        component.CurrentValue = Convert.ToDouble(m.Properties["FreeSpace"].Value);
                        component.LastContact = DateTime.Now;
                        if (component.Options.ContainsKey("Unit")) {
                            switch (component.Options["Unit"]) {
                                case "KB":
                                    component.CurrentValue = Convert.ToDouble(component.CurrentValue) / 1024;
                                    break;
                                case "MB":
                                    component.CurrentValue = Convert.ToDouble(component.CurrentValue) / 1024 / 1024;
                                    break;
                                case "GB":
                                    component.CurrentValue = Convert.ToDouble(component.CurrentValue) / 1024 / 1024 / 1024;
                                    break;
                                case "TB":
                                    component.CurrentValue = Convert.ToDouble(component.CurrentValue) / 1024 / 1024 / 1024 / 1024;
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    }
                }
                CompressionManager.Instance.CompressStandard(component);
                DataAccess.DatabaseFacade.DatabaseManager.Components.Save(component);
            }
        }
    }
}
