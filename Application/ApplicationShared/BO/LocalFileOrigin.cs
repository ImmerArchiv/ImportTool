using Archiv10.Application.Shared.BO;
using Archiv10.Domain.Shared.BO;

namespace Archiv10.Application.Shared.Services
{
    public class LocalFileOrigin 
    {
        public UIFile File { get; set; }

        public override string ToString()
        {
            return "bagit=" + File.SourceFolder + " name=" + File.SourceFile.data.Name;
        }

    }
}