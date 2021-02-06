using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLA_Workshop_Assistant
{
    public class NewSteamWorkshopItemEventArgs : EventArgs
    {
        public NewSteamWorkshopItemEventArgs(SteamWorkshopItem item)
        {
            WorkshopItem = item;
        }
        public SteamWorkshopItem WorkshopItem { get; private set; }
    }
}
