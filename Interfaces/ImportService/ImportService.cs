using Archiv10.Interfaces.ImportService;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImportService
{
    public partial class ImportService : ServiceBase
    {
        private Scheduler scheduler = null;
        private Thread thread = null;
        private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ImportService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            scheduler = new Scheduler(); 
            // Create the thread object, passing in the Scheduler.Run method
            // via a ThreadStart delegate. This does not start the thread.
            thread = new Thread(new ThreadStart(scheduler.Run));

            // Start the thread
            thread.Start();

            log.Info("WindowsService gestartet");
        }

        protected override void OnStop()
        {
            //STop the Scheduler
            scheduler.ShouldRun = false;

            // Wait until oThread finishes. Join also has overloads
            // that take a millisecond interval or a TimeSpan object.
            if (!thread.Join(TimeSpan.FromSeconds(10)))
            {
                log.Error("Could not Stop Thread");
                thread.Abort();
            }

            thread = null;
            scheduler = null;
            log.Info("WindowsService gestoppt");
        }
    }
}
