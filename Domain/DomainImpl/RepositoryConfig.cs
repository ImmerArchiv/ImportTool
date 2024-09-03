using Archiv10.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Impl
{
    class RepositoryConfig : IRepositoryConfig
    {
        private int _transferBlocks;

        public RepositoryConfig()
        {
            var blocks = ConfigurationManager.AppSettings["TransferBlocks"];
            _transferBlocks = Int32.Parse(blocks);
        }

        public int DataSize
        {
            get
            {
                return 1024 * _transferBlocks; //1KB
            }
        }
    }
}
