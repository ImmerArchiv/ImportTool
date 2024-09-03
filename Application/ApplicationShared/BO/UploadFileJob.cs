using Archiv10.Application.Shared.BO;
using Archiv10.Domain.Shared.BO;

namespace Archiv10.Application.Shared.BO
{
    public class UploadFileJob : IJob
    {
        public BagId bagid { get; set; }
        public UIFile file { get; set; }

        public UploadFileJob(BagId bagid, UIFile file)
        {
            this.bagid = bagid;
            this.file = file;
        }

        public override string ToString()
        {
            return string.Format("Upload File={0}", file.SourceFile.data.Name);
        }
    }
}