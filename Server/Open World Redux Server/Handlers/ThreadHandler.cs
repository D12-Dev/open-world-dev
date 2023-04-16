using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace OpenWorldReduxServer
{
    public static class ThreadHandler
    {
        public static void GenerateServerThread(int threadID, ServerClient client=null)
        {
            try
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
                    Thread StructuresThread = new Thread(() => FactionHandler.StructureClock());
                    StructuresThread.IsBackground = true;
                    StructuresThread.Name = "Structures Thread";
                    StructuresThread.Start();
                }

                else if (threadID == 3)
                {
                    Thread AuthChannelThread = new Thread(() => AuthNetwork.TryConnectToServer());
                    AuthChannelThread.IsBackground = true;
                    AuthChannelThread.Name = "Auth Channel Thread";
                    AuthChannelThread.Start();
                }
                else if (threadID == 4)
                {
                    Thread BackupThread = new Thread(() => BackupHandler.CreateServerBackupHeartbeat());
                    BackupThread.IsBackground = true;
                    BackupThread.Name = "Backup Heart Beat Thread";
                    BackupThread.Start();
                }
                else if (threadID == 5)
                {
                    Thread KickThread = new Thread(() => Network.SaveBeforeKick(client));
                    KickThread.IsBackground = true;
                    KickThread.Name = "Save Kick Thread";
                    KickThread.Start();
                }
                else return;
            }
            catch (Exception ex) { 
                Console.WriteLine(ex.ToString());
            
            }
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
