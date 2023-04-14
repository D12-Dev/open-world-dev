using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Cache;

namespace OpenWorldReduxServer
{
    [Serializable]
    public class ServerClient
    {
        [NonSerialized] public TcpClient tcp;
        [NonSerialized] public bool disconnectFlag; // This indicates to just disconnect client
        [NonSerialized] public bool disconnectsaveFlag;// This indicates to save current client before disconnect
        public string Username { get; set; }

        public string Password { get; set; }

        public string SavedID { get; set; }

        public string SavedIP { get; set; }

        public string SettlementTile { get; set; }

        public string FactionName { get; set; }

        public int selectedProductionItem { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsBanned { get; set; }

        [NonSerialized] public bool isEventProtected;
        [NonSerialized] public bool isBusy;

        public ServerClient(TcpClient tcp)
        {
            if (tcp == null) return;
            else
            {
                this.tcp = tcp;
                SavedIP = ((IPEndPoint)tcp.Client.RemoteEndPoint).Address.ToString();
            }
        }
    }
}
