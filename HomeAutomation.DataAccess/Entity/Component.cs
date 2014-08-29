using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeAutomation.DataAccess.Entity {
    public class Component {

        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Compression { get; set; }

        public decimal Interval { get; set; }

        public String ValueType { get; set; }

        public Object CurrentValue { get; set; }

        public Object MinThreshold { get; set; }

        public Object MaxThreshold { get; set; }

        public Boolean ThresholdReach { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastContact { get; set; }

        public Object LastHistoricalValue { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastHistoricalContact { get; set; }

        public bool IsActive { get; set; }

        public Dictionary<String, String> Options { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public DateTime NextContact { get; set; }

        public MongoDB.Driver.MongoDBRef TypeId { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public ComponentType Type {
            get {
                return DatabaseFacade.DatabaseManager.Database.FetchDBRefAs<ComponentType>(TypeId);
            }
            set {
                if (value.Id.Pid == 0) {
                    DatabaseFacade.DatabaseManager.ComponentTypes.Insert(value);
                }

                TypeId = new MongoDB.Driver.MongoDBRef("ComponentType", value.Id);
            }
        }



        public MongoDB.Driver.MongoDBRef DeviceId { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public Device Device {
            get {
                return DatabaseFacade.DatabaseManager.Database.FetchDBRefAs<Device>(DeviceId);
            }
            set {
                if (value.Id.Pid == 0) {
                    DatabaseFacade.DatabaseManager.Devices.Insert(value);
                }

                DeviceId = new MongoDB.Driver.MongoDBRef("Device", value.Id);
            }
        }

        public void SetNewValue(object value) {
            if (value != null) {

                if (IsNumber(value)) {
                    bool OnThreshold = false;
                    double diff =0;
                    String threshold = "";
                    if (MaxThreshold != null) {
                        if (Convert.ToDouble(value) >= Convert.ToDouble(MaxThreshold)) {
                            OnThreshold = true;
                            threshold = "Maximum";
                            diff= Convert.ToDouble(value) - Convert.ToDouble(MaxThreshold);
                        }
                    }
                    if (MinThreshold != null) {
                        if (Convert.ToDouble(value) <= Convert.ToDouble(MinThreshold)) {
                            OnThreshold = true;
                            threshold = "Minimum";
                            diff= Convert.ToDouble(value) - Convert.ToDouble(MinThreshold);
                        }
                    }

                    if (!ThresholdReach & OnThreshold) {
                        //Send Alert Email
                        ThresholdReach = true;
                        
                        var to = Device.OwnerEmail;
                        var subject = "[Alert] - " +Name+ " has reach "+threshold.ToLower()+ " value on "+ Device.Name;
                        var message = "Current value of "+ Name + " on "+Device.Name +" is "+value+" at "+DateTime.Now+".\n\rThat Exceed the "+ threshold+ " value of "+ diff.ToString()+ ".\n\rPrevious value was "+CurrentValue+ " at "+LastContact;
                        EmailManager.SendEmail(to, subject, message);
                    } else if (ThresholdReach & !OnThreshold) {
                        //Send Recovered Email
                        ThresholdReach = false;

                        var to = Device.OwnerEmail;
                        var subject = "[Alert] - Recovered " + Name + " on " + Device.Name;
                        var message = "Current value of " + Name + " on " + Device.Name + " is " + value + " at " + DateTime.Now + ".\n\rPrevious value was " + CurrentValue + " at " + LastContact;
                        EmailManager.SendEmail(to, subject, message);
                    }
                }
            }
            CurrentValue = value;
        }

        private bool IsNumber(object value) {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }

    }
}
