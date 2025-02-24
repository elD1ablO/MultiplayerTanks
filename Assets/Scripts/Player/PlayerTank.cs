using Unity.Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerTank : NetworkBehaviour
{
    [Header("Camera")]
    [SerializeField] CinemachineCamera virtualCamera;
    private byte ownerPriority = 15;

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData = HostSingleton.Instance.GameManager.NetworkServer.GetUserByClientID(OwnerClientId);
            PlayerName.Value = userData.userName;
        }

        if (IsOwner)
        {
            virtualCamera.Priority = ownerPriority;
        }
    }
}
