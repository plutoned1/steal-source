using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ark.ModHandler;
using static ark.Core.UI.UIProcess;
using static ark.Core.Menu.Main;

namespace ark.Mods
{
    internal class Settings
    {
        public static void ClearNotifications()
        {
            notif.clearAll();
            Disable("Clear Notifications");
        }
    }
}
