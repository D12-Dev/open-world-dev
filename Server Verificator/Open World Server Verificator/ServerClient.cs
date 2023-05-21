using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Cache;

namespace OpenWorldServerVerificator
{
    [Serializable]
    public class ServerClient
    {
        [NonSerialized] public TcpClient tcp;
        [NonSerialized] public bool disconnectFlag;

        public string Username { get; set; }

        public string Token { get; set; }

        public string SavedID { get; set; }

        public string SavedIP { get; set; }

        public bool ToBeRenewed { get; set; } = true;

        [NonSerialized] public bool isBusy;

        public ServerClient(TcpClient tcp = null)
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
