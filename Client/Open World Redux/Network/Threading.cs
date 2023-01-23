using System.Threading;

namespace OpenWorldRedux
{
    public static class Threading
    {
        public static void GenerateThreads(int threadID)
        {
            if (threadID == 0)
            {
                Thread NetworkThread = new Thread(new ThreadStart(Network.TryConnectToServer));
                NetworkThread.IsBackground = true;
                NetworkThread.Name = "Connection Thread";
                NetworkThread.Start();
            }

            else return;
        }
    }
}
