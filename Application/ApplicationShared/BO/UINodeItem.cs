using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archiv10.Domain.Shared.BO;

namespace Archiv10.Application.Shared.BO
{
    public class UINodeItem
    {
        public UINodeItem()
        {
            this.Items = new ObservableCollection<UINodeItem>();
        }

        public string Title { get; set; }

        public UICanvasIcon Icon { get; set; }

        public ObservableCollection<UINodeItem> Items { get; set; }
        public LocalFileState State { get; set; }
    }
}
