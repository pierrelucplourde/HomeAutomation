using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace HomeAutomation.DataCollector.Manager {
    internal class VMwareController {
        BackgroundWorker worker;
        Dictionary<String, String> SensorsWebResponse;

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

        public VMwareController() {
            SensorsWebResponse = new Dictionary<string, string>();
        }

        public void StartQuery(RunWorkerCompletedEventHandler EndPingEventHandler, DataAccess.Entity.Component component) {
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
                        case "esxi sensor":
                            GetSensorValue(component);
                            break;
                        default:

                            break;
                    }

                }

                e.Result = component;
            } catch (Exception ex) {
                var logFile = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "error.log", true);
                logFile.WriteLine(ex.ToString());
                logFile.Close();
            }
        }

        private void GetSensorValue(DataAccess.Entity.Component component) {

            String substringXML;
            try {
                if (component.Options.ContainsKey("SensorName")) {

                    if (SensorsWebResponse.ContainsKey(component.Device.IPAddress)) {
                        substringXML = SensorsWebResponse[component.Device.IPAddress];
                    } else {
                        WebClient wc = new WebClient();

                        if (component.Options.ContainsKey("User")) {
                            if (!String.IsNullOrEmpty(component.Options["User"])) {

                                wc.Credentials = new NetworkCredential(component.Options["User"], component.Options["Password"]);//TODO - Encrypt Password
                            }
                        }

                        String str = wc.DownloadString(new Uri("https://" + component.Device.IPAddress + "/mob/?moid=ha-host&doPath=runtime.healthSystemRuntime.systemHealthInfo.numericSensorInfo"));

                        substringXML = str.Substring(str.IndexOf("<xml"), str.IndexOf("</xml>") - str.IndexOf("<xml") + 6);
                        SensorsWebResponse.Add(component.Device.IPAddress, substringXML);
                    }

                    XDocument xdoc = XDocument.Parse(substringXML);

                    var SensorsInfos = from lv1 in xdoc.Root.Descendants("{urn:vim25}HostNumericSensorInfo")
                                       select new Helpers.HostNumericSensorInfo() {
                                           Name = lv1.Descendants("{urn:vim25}name").FirstOrDefault().Value,
                                           //UnitModifier = Convert.ToInt32(lv1.Descendants("{urn:vim25}unitModifier").FirstOrDefault().Value),
                                           //BaseUnits = lv1.Descendants("{urn:vim25}baseUnits").FirstOrDefault().Value,
                                           //SensorType = lv1.Descendants("{urn:vim25}sensorType").FirstOrDefault().Value,
                                           CurrentReading = Convert.ToInt32(lv1.Descendants("{urn:vim25}currentReading").FirstOrDefault().Value)
                                       };

                    var SensorInfo = SensorsInfos.Where(u => u.Name.Contains(component.Options["SensorName"])).FirstOrDefault();
                    if (SensorInfo != null) {
                        component.SetNewValue(Math.Abs(SensorInfo.CurrentReading / 100));
                        component.LastContact = DateTime.Now;

                        CompressionManager.Instance.CompressStandard(component);
                        DataAccess.DatabaseFacade.DatabaseManager.Components.Save(component);

                    }
                }
            } catch (Exception ex) {
                var logFile = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "error.log", true);
                logFile.WriteLine(ex.ToString());
                logFile.WriteLine("Adress: '" + "https://" + component.Device.IPAddress + "/mob/?moid=ha-host&doPath=runtime.healthSystemRuntime.systemHealthInfo.numericSensorInfo" + "'");
                logFile.Close();
            }
        }

    }
}
