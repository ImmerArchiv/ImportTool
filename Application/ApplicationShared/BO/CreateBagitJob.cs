using Archiv10.Application.Shared.BO;
using Archiv10.Domain.Shared.BO;

namespace Archiv10.Application.Shared.BO
{
    public class CreateBagitJob : IJob
    {
        public BagId bagid { get; set; }
        public FolderPath sourceFolder {  get; set; }

        public CreateBagitJob(BagId bagid, FolderPath sourceFolder)
        {
            this.bagid = bagid;
            this.sourceFolder = sourceFolder;
        }

        public override string ToString()
        {
            return string.Format("Create Bagit id={0} Name={1}", bagid.ToString(), sourceFolder.ToString());
        }
    }
}