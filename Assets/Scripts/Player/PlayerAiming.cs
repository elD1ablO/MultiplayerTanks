using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour

{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform turretTransform;
    private void LateUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        // Convert mouse position to a world position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Ground plane at y = 0

        if (groundPlane.Raycast(ray, out float enter))
        {
            // Get the point on the ground where the ray intersects
            Vector3 mouseWorldPosition = ray.GetPoint(enter);

            // Calculate direction from turret to mouse position
            Vector3 direction = mouseWorldPosition - turretTransform.position;

            // Lock the y-axis direction to ensure the turret rotates horizontally
            direction.y = 0;

            // Rotate the turret to face the direction
            turretTransform.rotation = Quaternion.LookRotation(direction);
        }
    }

}
