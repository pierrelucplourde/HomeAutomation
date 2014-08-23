using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeAutomation.DataCollector.Manager {
    internal class CompressionManager {

        private static CompressionManager _Instance;
        public static CompressionManager Instance {
            get {
                if (_Instance == null) {
                    _Instance = new CompressionManager();
                }

                return _Instance;
            }
            private set {
                _Instance = value;
            }
        }

        private CompressionManager() {

        }



        internal void CompressStandard(DataAccess.Entity.Component component) {
            //If something change let's archive it
            if (component.CurrentValue != component.LastHistoricalValue) {
                if (component.LastHistoricalValue != null) {
                    var delta = Math.Abs(Convert.ToDouble(component.CurrentValue) - Convert.ToDouble(component.LastHistoricalValue));
                    if (delta >= Convert.ToDouble(component.Compression)) {
                        var nHistory = new DataAccess.Entity.ComponentValueHistory() {
                            Component = component,
                            TimeStamp = DateTime.Now,
                            Value = component.CurrentValue
                        };
                        DataAccess.DatabaseFacade.DatabaseManager.ComponentValueHistory.Insert(nHistory);
                        component.LastHistoricalValue = nHistory.Value;
                        component.LastHistoricalContact = nHistory.TimeStamp;
                    }
                } else {
                    var nHistory = new DataAccess.Entity.ComponentValueHistory() {
                        Component = component,
                        TimeStamp = DateTime.Now,
                        Value = component.CurrentValue
                    };
                    DataAccess.DatabaseFacade.DatabaseManager.ComponentValueHistory.Insert(nHistory);
                    component.LastHistoricalValue = nHistory.Value;
                    component.LastHistoricalContact = nHistory.TimeStamp;
                }
            }
        }

        internal void CompressPingDelay(DataAccess.Entity.Component component) {
            //If something change let's archive it
            if (component.CurrentValue != component.LastHistoricalValue) {
                if (component.LastHistoricalValue != null) {
                    var delta = Math.Abs(Convert.ToDouble(component.CurrentValue) - Convert.ToDouble(component.LastHistoricalValue));
                    if (delta >= Convert.ToDouble(component.Compression) | Convert.ToDouble(component.CurrentValue) == -1 | Convert.ToDouble(component.LastHistoricalValue) == -1) {
                        var nHistory = new DataAccess.Entity.ComponentValueHistory() {
                            Component = component,
                            TimeStamp = DateTime.Now,
                            Value = component.CurrentValue
                        };
                        DataAccess.DatabaseFacade.DatabaseManager.ComponentValueHistory.Insert(nHistory);
                        component.LastHistoricalValue = nHistory.Value;
                        component.LastHistoricalContact = nHistory.TimeStamp;
                    }
                } else {
                    var nHistory = new DataAccess.Entity.ComponentValueHistory() {
                        Component = component,
                        TimeStamp = DateTime.Now,
                        Value = component.CurrentValue
                    };
                    DataAccess.DatabaseFacade.DatabaseManager.ComponentValueHistory.Insert(nHistory);
                    component.LastHistoricalValue = nHistory.Value;
                    component.LastHistoricalContact = nHistory.TimeStamp;
                }
            }
        }

    }
}
