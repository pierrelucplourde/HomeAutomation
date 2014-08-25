using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace HomeAutomation.DataCollector {
    public partial class Service1 : ServiceBase {

        private bool stopping;
        private ManualResetEvent stoppedEvent;
        private StreamWriter logFile;

        public Service1() {
            InitializeComponent();
            logFile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "service.log", true);
            logFile.WriteLine(DateTime.Now + " - " + "Constructor");
            try {
                this.stopping = false;
                this.stoppedEvent = new ManualResetEvent(false);
            } catch (Exception) {
                logFile.WriteLine(DateTime.Now + " - " + "Constructor Catch");
                logFile.Close();
            }
        }

        protected override void OnStart(string[] args) {

            try {
                logFile.WriteLine(DateTime.Now + " - " + "OnStart Begin");

                // Log a service start message to the Application log.
                this.eventLog1.WriteEntry("HomeAutomation in OnStart.");

                // Queue the main service function for execution in a worker thread.
                ThreadPool.QueueUserWorkItem(new WaitCallback(ServiceWorkerThread));
                logFile.WriteLine(DateTime.Now + " - " + "OnStart End");
            } catch (Exception ex) {
                logFile.WriteLine(DateTime.Now + " - " + "OnStart Catch");
                logFile.WriteLine(DateTime.Now + " - " + ex.Message);
                logFile.WriteLine(DateTime.Now + " - " + ex.StackTrace);
                logFile.WriteLine(DateTime.Now + " - " + ex.InnerException);
                logFile.Close();

            }

        }


        /// <summary>
        /// The method performs the main function of the service. It runs on 
        /// a thread pool worker thread.
        /// </summary>
        /// <param name="state"></param>
        private void ServiceWorkerThread(object state) {
            try {
                logFile.WriteLine(DateTime.Now + " - " + "WorkerThread Start");

                try {
                    this.eventLog1.WriteEntry("ConfigFile :" + AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                    this.eventLog1.Close();
                    
                } catch (Exception Ex) {
                    this.eventLog1.WriteEntry("Erreur " + Ex.Message);
                }
                // Periodically check if the service is stopping.
                while (!this.stopping) {
                    // Perform main service function here...
                    try {
                        
                    } catch (Exception ex) {
                        this.eventLog1.WriteEntry("Erreur " + ex.Message);
                        logFile.WriteLine(DateTime.Now + " - " + "StartProgram Catch");
                        logFile.WriteLine(DateTime.Now + " - " + ex.Message);
                        logFile.WriteLine(DateTime.Now + " - " + ex.StackTrace);
                        logFile.WriteLine(DateTime.Now + " - " + ex.InnerException);
                        //logFile.Close();
                    }
                    Thread.Sleep(2000);  // Simulate some lengthy operations.
                }

                // Signal the stopped event.
                this.stoppedEvent.Set();
                logFile.WriteLine(DateTime.Now + " - " + "WorkerThread Stopped Event Set");
            } catch (Exception) {
                logFile.WriteLine(DateTime.Now + " - " + "WorkerThread MegaCatch");

                logFile.Close();

            }

        }


        /// <summary>
        /// The function is executed when a Stop command is sent to the 
        /// service by SCM. It specifies actions to take when a service stops 
        /// running. In this code sample, OnStop logs a service-stop message 
        /// to the Application log, and waits for the finish of the main 
        /// service function.
        /// </summary>
        protected override void OnStop() {
            try {
                // Log a service stop message to the Application log.
                logFile.WriteLine(DateTime.Now + " - " + "OnStop Begin");

                this.eventLog1.WriteEntry("HomeAutomation in OnStop.");

                // Indicate that the service is stopping and wait for the finish 
                // of the main service function (ServiceWorkerThread).
                this.stopping = true;
                this.stoppedEvent.WaitOne();

                
                logFile.WriteLine(DateTime.Now + " - " + "OnStop End");
                logFile.Close();
            } catch (Exception) {
                logFile.WriteLine(DateTime.Now + " - " + "OnStop MegaCatch");

                logFile.Close();

            }
        }


    }
}
