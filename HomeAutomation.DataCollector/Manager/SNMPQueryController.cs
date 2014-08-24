using SnmpSharpNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;

namespace HomeAutomation.DataCollector.Manager {
    internal class SNMPQueryController {

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

        internal SNMPQueryController() {

        }

        internal void StartQuery(RunWorkerCompletedEventHandler EndPingEventHandler, DataAccess.Entity.Component component) {
            worker = new BackgroundWorker();
            worker.RunWorkerCompleted += EndPingEventHandler;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync(component);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e) {
            var component = (DataAccess.Entity.Component)e.Argument;
            var communityString = "public";
            
            if (component.Options.Keys.Contains("CommunityString")) {
                communityString = component.Options["CommunityString"];
            }
            if (!component.Options.Keys.Contains("OID")) {
                return;
            }
            var OIDString = component.Options["OID"];

            //Process the snmp query
            // SNMP community name
            OctetString community = new OctetString(communityString);

            // Define agent parameters class
            AgentParameters param = new AgentParameters(community);
            // Set SNMP version to 1 (or 2)
            param.Version = SnmpVersion.Ver2;
            // Construct the agent address object
            // IpAddress class is easy to use here because
            //  it will try to resolve constructor parameter if it doesn't
            //  parse to an IP address
            IpAddress agent = new IpAddress(component.Device.IPAddress);

            // Construct target
            UdpTarget target = new UdpTarget((IPAddress)agent, 161, 2000, 1);

            // Pdu class used for all requests
            Pdu pdu = new Pdu(PduType.Get);
            pdu.VbList.Add(OIDString); //sysDescr
            

            // Make SNMP request
            SnmpV2Packet result = (SnmpV2Packet)target.Request(pdu, param);

            // If result is null then agent didn't reply or we couldn't parse the reply.
            if (result != null) {
                // ErrorStatus other then 0 is an error returned by 
                // the Agent - see SnmpConstants for error definitions
                if (result.Pdu.ErrorStatus == 0) {
                    // Reply variables are returned in the same order as they were added
                    //  to the VbList
                    //Console.WriteLine("sysDescr({0}) ({1}): {2}",
                    //    result.Pdu.VbList[0].Oid.ToString(),
                    //    SnmpConstants.GetTypeName(result.Pdu.VbList[0].Value.Type),
                    //    result.Pdu.VbList[0].Value.ToString());

                    component.CurrentValue = result.Pdu.VbList[0].Value.ToString();
                    Double val;
                    if (Double.TryParse(component.CurrentValue.ToString(), out val)) {
                        component.CurrentValue = val;
                        CompressionManager.Instance.CompressStandard(component);

                    }

                    DataAccess.DatabaseFacade.DatabaseManager.Components.Save(component);
                }
            } 
            target.Close();

            e.Result = component;
        }

    }
}
