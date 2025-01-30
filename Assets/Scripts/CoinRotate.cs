using UnityEngine;

public class CoinRotate : MonoBehaviour
{
    private float rotationSpeed = 50f;
    
    void Update()
    {
        // Rotate the GameObject around its Y-axis
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
