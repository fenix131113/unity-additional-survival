using System.Collections;
using Mirror;
using UnityEngine;

namespace Core.Network
{
    public class GameStartedAuthenticator : NetworkAuthenticator
    {
        private const byte CODE_SUCCESS = 100;
        private const byte CODE_REJECT = 0;

        private bool _isGameStarted;

        public void StartGame() => _isGameStarted = true;

        #region Messages

        private struct AuthResponseMessage : NetworkMessage
        {
            public byte Code;
        }

        #endregion

        #region Server

        public override void OnServerAuthenticate(NetworkConnectionToClient conn)
        {
            if (_isGameStarted)
                StartCoroutine(DelayedReject(conn));
            else
            {
                conn.Send(new AuthResponseMessage { Code = CODE_SUCCESS });
                ServerAccept(conn);
            }
        }

        private IEnumerator DelayedReject(NetworkConnectionToClient conn)
        {
            conn.Send(new AuthResponseMessage { Code = CODE_REJECT });

            yield return new WaitForSeconds(1f);

            ServerReject(conn);
        }

        #endregion

        #region Client

        public override void OnStartClient() =>
            NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponse, false);

        public override void OnStopClient() => NetworkClient.UnregisterHandler<AuthResponseMessage>();

        private void OnAuthResponse(AuthResponseMessage message)
        {
            switch (message.Code)
            {
                case CODE_SUCCESS:
                    ClientAccept();
                    break;
                case CODE_REJECT:
#if UNITY_EDITOR
                    Debug.Log("Can't join, game is already started!");
#endif
                    ClientReject();
                    break;
            }
        }

        #endregion
    }
}