using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OpenWorldRedux
{
    public static class Network
    {
        public static TcpClient connection;
        public static Stream s;
        public static NetworkStream ns;
        public static StreamWriter sw;
        public static StreamReader sr;

        public static string ip = "";
        public static string port = "";
        public static string username = "";
        public static string password = "";

        public static void TryConnectToServer()
        {
            BooleanCache.isTryingToConnect = true;

            Find.WindowStack.Add(new OW_WaitingDialog());

            try
            {
                connection = new TcpClient(ip, int.Parse(port));
                s = connection.GetStream();
                sw = new StreamWriter(s);
                sr = new StreamReader(s);
                ns = connection.GetStream();

                BooleanCache.isConnectedToServer = true;
                BooleanCache.isTryingToConnect = false;
                FocusCache.waitWindowInstance.Close();

                ListenToServer();
            }

            catch(Exception ex)
            {
                Log.Message($"Failed to connect to server. Full Stack Error:\n{ex.ToString()}");
                DisconnectFromServer();
                FocusCache.waitWindowInstance.Close();
            }
        }
        public static void ServerConnectionHeartbeat()
        {


            try
            {

            }

            catch (Exception ex)
            {
                Log.Message($"Failed to connect to server. Full Stack Error:\n{ex.ToString()}");
                DisconnectFromServer();
                FocusCache.waitWindowInstance.Close();
            }
        }
        public static void ListenToServer()
        {
            GeneralHandler.SendAuthFile();

            while (true)
            {
                if (!BooleanCache.isConnectedToServer) return;

                string data = sr.ReadLine();

                if (data == null)
                {
                    Log.Message("Received null data packet, disconnecting from server!");
                    DisconnectFromServer();
                    return;
                }

                else PacketHandler.HandlePacket(data);
            }
        }

        public static void SendData(Packet toSend)
        {
            try
            {
                sw.WriteLine(Serializer.Serialize(toSend));
                sw.Flush();
            }

            catch { DisconnectFromServer(); }
        }

        public static void DisconnectFromServer()
        {
            Log.Message("Client disposed server connection!");
            if (connection != null) connection.Dispose();

            ErrorHandler.ForceDisconnectCountermeasures();
        }
        public static void HeartBeatPacket() { 
              
        
        
        
        
        }
    }
}