using OpenWorldRedux.RTSE;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Verse;
using System.Threading;

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
        public static string LastserverIP;
        public static string LastserverPORT;
        public static string LastserverUSER;
        public static string LastserverPASS;
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
                    LastserverIP = ip;
                    LastserverPORT = port;
                    LastserverUSER = username;
                    LastserverPASS = password;
                    BooleanCache.isConnectedToServer = true;
                    BooleanCache.isTryingToConnect = false;
                    FocusCache.waitWindowInstance.Close();
                    Thread newThread = new Thread(new ThreadStart(Checkforplayers));
                    newThread.Start();
                    ListenToServer();
                }




            catch (Exception ex)
            {
                Log.Message($"[Openworld] Failed to connect to server. Full Stack Error:\n{ex.ToString()}");
                DisconnectFromServer();
                FocusCache.waitWindowInstance.Close();
            }
        }
        public static bool oneplayerjoined = false;
        public static void ListenToServer()
        {
            GeneralHandler.SendAuthFile();

            while (true)
            {
                if (!BooleanCache.isConnectedToServer) return;

                string data = sr.ReadLine();

                if (data == null)
                {
                    Log.Message("[Openworld] Received null data packet, disconnecting from server!");
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

            catch (Exception ex)
            {

                Log.Message($"[Openworld] Client couldnt send data to server. Full stack error:\n{ex}");
                DisconnectFromServer();

            }
        }

        public static void DisconnectFromServer()
        {
            Log.Message("[Openworld] Client disposed server connection!");
            if (connection != null) connection.Dispose();

            ErrorHandler.ForceDisconnectCountermeasures();
        }

        public static void Checkforplayers()
        {
            while (true)
            {
                    //Log.Message("Called first step of restart");
                    if (Current.ProgramState == ProgramState.Playing && BooleanCache.isConnectedToServer && BooleanCache.ishostingrtseserver && Multiplayer.Client.Multiplayer.session != null && Multiplayer.Client.Multiplayer.session.players.Count > 1 && oneplayerjoined == false)
                    {
                        Log.Message("Player joined.");
                        oneplayerjoined = true;
                    }
                    if (Current.ProgramState == ProgramState.Playing && BooleanCache.isConnectedToServer && BooleanCache.ishostingrtseserver && Multiplayer.Client.Multiplayer.session != null && Multiplayer.Client.Multiplayer.session.players.Count <= 1 && oneplayerjoined)
                    {
                        Log.Message("calling remove taken");
                        OnSendVisitRequest.removetakenitems(OW_SendVisitConfirmation.savedpeerstring);
                    }
            }
        }
    }
}