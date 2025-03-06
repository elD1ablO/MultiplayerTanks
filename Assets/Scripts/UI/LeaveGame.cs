using Unity.Netcode;
using UnityEngine;

public class LeaveGame : MonoBehaviour
{
    public void DisconnectFromGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.GameManager.ShutDown();
        }

        ClientSingleton.Instance.GameManager.Disconnect();
    }
}
