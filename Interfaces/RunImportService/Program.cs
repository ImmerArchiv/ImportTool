using Archiv10.Interfaces.ImportService;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RunImportService
{
    class Program
    {
        static void Main(string[] args)
        {
            var program = new Program();
            program.OnStart(args);

            Console.WriteLine("Service started; Press key for exit service");
            Console.ReadKey();

            program.OnStop();

            Console.WriteLine("Service stoped");
            Console.ReadKey();
        }



        private Scheduler scheduler = null;
        private Thread thread = null;
        private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    
        protected void OnStart(string[] args)
        {
            scheduler = new Scheduler();
            // Create the thread object, passing in the Scheduler.Run method
            // via a ThreadStart delegate. This does not start the thread.
            thread = new Thread(new ThreadStart(scheduler.Run));

            // Start the thread
            thread.Start();

            log.Info("WindowsService gestartet");
        }

        protected void OnStop()
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
