using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FollowManager.Tab
{
    public class TabManager
    {
        public ObservableCollection<TabItemData> TabItemDatas { get; set; } = new ObservableCollection<TabItemData>();
    }
}
