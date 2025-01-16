using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] float turningRate = 180f;

    private Vector3 previousMovementInput;
    public override void OnNetworkSpawn() // use instead of Start()
    {
        if(!IsOwner) { return; }
        inputReader.MoveEvent += HandleMove;
        
    }
    public override void OnNetworkDespawn() //use instead of OnDestroy()
    {
        if (!IsOwner) { return; }
        inputReader.MoveEvent -= HandleMove;
    }

    void Update()
    {
        if (!IsOwner)
        { return; }
        float yRotation = previousMovementInput.x * turningRate * Time.deltaTime;
        bodyTransform.Rotate(0f, yRotation, 0f);
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
}
