using Archiv10.Application.Shared.BO;
using Archiv10.Application.Shared.Locator;
using Archiv10.Domain.Shared.BO;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Archiv10.Interfaces.ImportService
{
    public class Scheduler
    {
        private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Scheduler()
        {
            ShouldRun = true;
        }
        public bool ShouldRun { get; set; }
        public bool LastRunWithError { get; set; }

        public void Run()
        {
            ApplicationLocator.GetApplication().WriteServiceState(false, "STARTING", "Start Scheduler");
            try
            {
                while (ShouldRun)
                {
                    Step(DateTime.Now);
                    //Wait before next step
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }



            }
            catch(Exception e)
            {
                //Catch all Exception
                log.Error(e);
                ApplicationLocator.GetApplication().WriteServiceState(false, "ERROR", e.Message);
                return;
            }
            ApplicationLocator.GetApplication().WriteServiceState(false, "STOPPED", "Scheduler stopped");
        }

        private int lastHour = -1;
        private int lastMinute = -1;
        private IList<IJob> jobs = new List<IJob>();

        private DateTime lastSynchronize = DateTime.MinValue;

        private void Step(DateTime current)
        {

            if (current == new DateTime(0))
                throw new ArgumentOutOfRangeException("current");

            int hour = current.Hour;
            int minute = current.Minute;


            if (lastHour == hour && lastMinute == minute) return; //Nur einmal pro Zyklus ausführen

            log.DebugFormat("Cycle {0:D2}:{1:D2} Uhr", hour, minute);

            lastHour = hour;
            lastMinute = minute;


            ApplicationLocator.GetApplication().ReadConfig(); //Update Config, Application is Singleton

            //Load/Init Repositories 
            try
            {

                ApplicationLocator.GetApplication().ReadRepositories();  //Read all Bagit's without files

                ApplicationLocator.GetApplication().ReadStatus();

            }
            catch (Exception e)
            {
                log.Error(e);
                ApplicationLocator.GetApplication().WriteServiceState(false, "WAITING", "{0}", e.Message);

                return; //try again later
            }

            var cntDisconnected = ApplicationLocator.GetApplication().RepositoryList.Count(rp => rp.Active && rp.State == RepositoryState.Disconnected);
            var cntConnected = ApplicationLocator.GetApplication().RepositoryList.Count(rp => rp.Active && rp.State == RepositoryState.Connected);

            if (cntConnected == 0 || cntDisconnected > 0)
            {
                ApplicationLocator.GetApplication().WriteServiceState(false, "WAITING" , "Not all repositories are connected: connected={0} disconnected={1}", cntConnected, cntDisconnected);
                return;
            }

            //Load/Init Rootfolders
            ApplicationLocator.GetApplication().ReadRootFolders(); //Read all Local Folders without files


            var errors = new Dictionary<string, int>();
            foreach (var rf in ApplicationLocator.GetApplication().RootFolderList)
                errors[rf.Source.Path] = 0;


            //makeJob or Synchronized
            if (jobs.Count == 0)
            {


                //check for Modification
                log.DebugFormat("lastSynchronize={0}", lastSynchronize);

                // LastModified of Repositories (status)
                var lastModifiedRepositories = ApplicationLocator.GetApplication().GetLastmodifiedForRepositories();
                log.DebugFormat("lastModifiedRepositories={0}", lastModifiedRepositories);

                // LastModified of Local files (FileWatcher)
                var lastModifiedLocalFiles = ApplicationLocator.GetApplication().GetLastmodifiedForLocalFiles();
                log.DebugFormat("lastModifiedLocalFiles={0}", lastModifiedLocalFiles);

                // LastModified of Configuration (FileInfo)
                var lastModifiedConfiguration = ApplicationLocator.GetApplication().GetLastmodifiedForConfiguration();
                log.DebugFormat("lastModifiedConfiguration={0}", lastModifiedConfiguration);



                if ((lastSynchronize > lastModifiedRepositories)
                    && (lastSynchronize > lastModifiedLocalFiles)
                    && (lastSynchronize > lastModifiedConfiguration))
                {
                    //no changes 
                    log.DebugFormat("No changes, nothing to do");
                    ApplicationLocator.GetApplication().WriteServiceState(false, "WAITING", "No changes, nothing to do");
                    return;
                }

                ApplicationLocator.GetApplication().WriteServiceState(true, "CHECK", "Checking Repositories and local Folder");
                ApplicationLocator.GetApplication().UpdateFileWatcher(); //Watching files



                SyncFolderState(RootFolderSyncStatus.StateInit, jobs, errors);

                try
                {
                    ApplicationLocator.GetJobCreatorService().Create(jobs,() => ShouldRun);
                }
                catch (Exception e)
                {
                    log.Error(e);
                    ApplicationLocator.GetApplication().WriteServiceState(false, "WAITING", "{0}", e.Message);

                    return; //try again later
                }

                lastSynchronize = DateTime.Now;

            }


            //RootFolderState
            SyncFolderState(RootFolderSyncStatus.StateUploading, jobs, errors);
         
            while (ShouldRun && jobs.Count > 0)
            {

                IJob job = jobs[0];

                ApplicationLocator.GetApplication().WriteServiceState(true, "WORKING", "Do Job {0}, remaining {1}", job,  jobs.Count - 1);

                log.Info(job);

                try
                {
                    DoJob(job);
                }
                catch(Exception e)
                {
                    //register error if is an uploadjob
                    var uj = job as UploadFileJob;
                    if (uj != null)
                        errors[uj.file.RootSourceFolder.Path]++;

                    log.Error(e);
                }
                jobs.Remove(job);
            }

            //Update Time after jobs
            current = DateTime.Now;
            lastHour = current.Hour;
            lastMinute = current.Minute;
            SyncFolderState(RootFolderSyncStatus.StateSynchronized, jobs,errors);
            ApplicationLocator.GetApplication().WriteServiceState(false, "WAITING", "Cycle successful finished");
        }

        private void SyncFolderState(string state, IList<IJob> jobs,IDictionary<string,int> errors)
        {
            var syncstatelist = new List<RootFolderSyncStatus>();
            var upLoadJobs = jobs.Where(j => j.GetType() == typeof(UploadFileJob)).Cast<UploadFileJob>();
            foreach (var rf in ApplicationLocator.GetApplication().RootFolderList)
            {
                RootFolderSyncStatus syncstate = new RootFolderSyncStatus() { path = rf.Source.Path , state = state };
                syncstate.jobCnt = upLoadJobs.Count(j => j.file.RootSourceFolder.Path == rf.Source.Path);

                //ready if has no jobs
                if(state == RootFolderSyncStatus.StateUploading && syncstate.jobCnt == 0)
                {
                    //nothing to do
                    syncstate.state = RootFolderSyncStatus.StateSynchronized;
                }
              
                if(errors[rf.Source.Path] > 0)
                {
                    syncstate.state = RootFolderSyncStatus.StateError;
                }


                log.DebugFormat("RootFolder {0} has {1} UploadJobs", rf.Source.Path, syncstate.jobCnt);
                syncstatelist.Add(syncstate);
            }
            ApplicationLocator.GetApplication().CommitSyncFolderStatus(syncstatelist);
        }

      

     
        private void DoJob(IJob job)
        {
            if (job.GetType() == typeof(CreateBagitJob))
            {
                var createBagitJob = job as CreateBagitJob;
                var description = ApplicationLocator.GetApplication().CreateDescription(createBagitJob.sourceFolder);
                ApplicationLocator.GetApplication().CreateBag(createBagitJob.bagid, description);
                ApplicationLocator.GetApplication().ReadRepositories();
            }
            if (job.GetType() == typeof(UploadFileJob))
            {
                var uploadFileJob = job as UploadFileJob;

                var bag = ApplicationLocator.GetApplication().BagList.Where(b => b.SourceId.ToString() == uploadFileJob.bagid.ToString()).Single();
                var index = ApplicationLocator.GetApplication().BagList.IndexOf(bag);


                var filename = Path.Combine(uploadFileJob.file.SourceFolder.ToString(),uploadFileJob.file.SourceFile.data.Name);

                var parts = ApplicationLocator.GetApplication().CountRepositories(index) * ApplicationLocator.GetApplication().CountPartsOfFile(filename);
                var p = 0;
                ApplicationLocator.GetApplication().AppendFile(index, filename, () => {
                    p++;
                    ApplicationLocator.GetApplication().WriteServiceState(true, "WORKING", "Do Job {0}, remaining {1}, upload={2}/{3}", job, jobs.Count - 1,p,parts);
                    return null;
                });

            }

        }


    }
}
