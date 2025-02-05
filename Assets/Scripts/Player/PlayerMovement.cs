using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float turningRate = 180f;

    private Vector3 previousMovementInput;

    // NetworkVariable to synchronize rotation
    private NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>();

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        { return; }
        inputReader.MoveEvent += HandleMove;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
        { return; }
        inputReader.MoveEvent -= HandleMove;
    }

    void Update()
    {
        if (IsOwner)
        {
            // Only the owner calculates rotation
            float yRotation = previousMovementInput.x * turningRate * Time.deltaTime;
            bodyTransform.Rotate(0f, yRotation, 0f);

            // Update the NetworkVariable with the new rotation
            UpdateRotationServerRpc(bodyTransform.rotation);
        }
        else
        {
            // Other clients and host use the NetworkVariable value
            bodyTransform.rotation = networkRotation.Value;
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
        { return; }
        rb.linearVelocity = bodyTransform.forward * moveSpeed * previousMovementInput.z;
    }

    private void HandleMove(Vector3 movementInput)
    {
        previousMovementInput = movementInput;
    }

    [ServerRpc]
    private void UpdateRotationServerRpc(Quaternion newRotation)
    {
        // Update the NetworkVariable on the server
        networkRotation.Value = newRotation;
    }
}