using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;

    private void OnDestroy()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }
}
