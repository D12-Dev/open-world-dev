using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenWorldServerVerificator
{
    public static class ThreadHandler
    {
        public static void GenerateServerThread(int threadID)
        {
            if (threadID == 0)
            {
                Thread NetworkingThread = new Thread(new ThreadStart(Network.ReadyServer));
                NetworkingThread.IsBackground = true;
                NetworkingThread.Name = "Networking Thread";
                NetworkingThread.Start();
            }

            else if (threadID == 1)
            {
                Thread CheckThread = new Thread(() => Network.HearbeatClients());
                CheckThread.IsBackground = true;
                CheckThread.Name = "Heartbeat Thread";
                CheckThread.Start();
            }

            else if (threadID == 2)
            {
                Thread RenewalThread = new Thread(() => ClientHandler.CheckClientsRenewal());
                RenewalThread.IsBackground = true;
                RenewalThread.Name = "Renewal Thread";
                RenewalThread.Start();
            }

            else return;
        }

        public static void GenerateClientThread(ServerClient client)
        {
            Thread ClientThread = new Thread(() => Network.ListenToClient(client));
            ClientThread.IsBackground = true;
            ClientThread.Name = "User Thread " + client.SavedIP;
            ClientThread.Start();
        }
    }
}
