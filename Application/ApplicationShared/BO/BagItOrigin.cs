using Archiv10.Application.Shared.BO;

namespace Archiv10.Application.Shared.Services
{
    public class BagItOrigin 
    {
        public UIData Data { get; set;  }
      
        public override string ToString()
        {
            return "bagit=" + Data.SourceBag + " name=" + Data.SourceData.Name;
        }

    }
}