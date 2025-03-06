using Unity.Netcode;
using UnityEngine;

public class CoinRotate : NetworkBehaviour
{
    private float rotationSpeed = 50f;

    // Network variable to synchronize rotation
    private NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>();

    void Update()
    {
        // Only the server should update the rotation
        if (IsServer)
        {
            // Rotate the GameObject around its Y-axis
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

            // Update the network variable with the new rotation
            networkRotation.Value = transform.rotation;
        }
        else
        {
            // Clients should update their rotation based on the network variable
            transform.rotation = networkRotation.Value;
        }
    }

    public override void OnNetworkSpawn()
    {        
        // Initialize the rotation when the object spawns
        if (IsServer)
        {
            networkRotation.Value = transform.rotation;
        }
    }
}