using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeAutomation.DataCollector.Helpers {
    internal class HostNumericSensorInfo {
        public string Name { get; set; }

        public int UnitModifier { get; set; }

        public string BaseUnits { get; set; }

        public string SensorType { get; set; }

        public int CurrentReading { get; set; }
    }
}
