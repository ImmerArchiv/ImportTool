﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Shared.BO
{
    public interface IBag
    {
        IBagId   Id { get; set; }
        IBagInfo Info { get; set; }
        IBagData Data { get; set; }

    }
}
