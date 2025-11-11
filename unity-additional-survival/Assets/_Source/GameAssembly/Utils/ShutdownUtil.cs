using System.Linq;
using Mirror;

namespace Utils
{
    public static class ShutdownUtil
    {
        [Server]
        public static void Shutdown()
        {
            if(!NetworkServer.active)
                return;
            
            foreach (var conn in NetworkServer.connections.Values.Where(conn => conn is { isReady: true }).ToList())
                conn.Disconnect();
            
            if(NetworkServer.active && NetworkClient.active)
                NetworkManager.singleton.StopHost();
            else
                NetworkManager.singleton.StopServer();
        }
    }
}