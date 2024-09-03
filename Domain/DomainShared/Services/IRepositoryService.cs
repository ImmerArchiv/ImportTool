using Archiv10.Domain.Shared.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Shared.Services
{
    public interface IRepositoryService
    {

        bool Init(Repository repository);

        void Status(Repository repository);

        BagId Create(Repository repository, BagId bagId, BagInfo info);

        void AppendFile(Repository repository, BagId bagId , BagFile file);

        IList<BagSnippet> ListAll(Repository repository);

        Bag ListOne(Repository repository, BagId bagId);

        BagFilePart GetFilePart(Repository repository, BagId bagId,string fileName,long offset,int maxSize);
        void UploadFilePart(Repository repository, BagFilePart filepart);
        void DownloadFile(Repository repository, BagId sourceBag, string name, Func<byte[], int, object> callBackWrite);
    }
}
