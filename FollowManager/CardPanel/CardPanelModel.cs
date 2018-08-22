using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FollowManager.CardPanel
{
    public class CardPanelModel
    {
        public void OpenProfile(string screenName)
        {
            // プロフィールページをブラウザで開く
            var url = "https://twitter.com/" + screenName;
            System.Diagnostics.Process.Start(url);
        }
    }
}
