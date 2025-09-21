using Mirror;

namespace Core.Network
{
    public static class GlobalMessages
    {
        public struct CreateLobbyPlayerMessage : NetworkMessage{}
        public struct CreateGamePlayerMessage : NetworkMessage{}
    }
}