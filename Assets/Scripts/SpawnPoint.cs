using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private static List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    private void OnEnable()
    {
        spawnPoints.Add(this);
    }
    public static Vector3 GetRandomSpawnPos()
    {

        Debug.LogError("THERE is spawnpoints:" + spawnPoints.Count);
        if (spawnPoints.Count == 0) return Vector3.zero;

        return spawnPoints[Random.Range(0, spawnPoints.Count - 1)].transform.position;
    }

    private void OnDisable()
    {
        spawnPoints.Remove(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 1);
    }
}
