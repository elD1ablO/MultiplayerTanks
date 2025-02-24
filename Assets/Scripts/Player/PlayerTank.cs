using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerTank : NetworkBehaviour
{
    [Header("Camera")]
    [SerializeField] CinemachineCamera virtualCamera;
    private byte ownerPriority = 15;
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            virtualCamera.Priority = ownerPriority;
        }
    }
}
