using Archiv10.Domain.Shared.Services;
using Archiv10.Infrastructure.Shared.Locator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Impl.Services
{
    class Md5SumService : ICheckSumService
    {
        public string CalcForFile(string filename)
        {
            using (var md5 = MD5.Create())
            {
                var fileService = InfrastructureLocator.GetFileService();

                using (var stream = fileService.OpenRead(filename))
                {
                    var buffer = md5.ComputeHash(stream);
                    var sb = new StringBuilder();
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        sb.Append(buffer[i].ToString("x2"));
                    }
                    return sb.ToString();
                }

            }
        }
    }
}
