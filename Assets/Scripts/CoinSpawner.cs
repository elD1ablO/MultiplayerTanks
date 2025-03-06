using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    [SerializeField] private CoinRespawning coinPrefab;
    [SerializeField] private int maxCoins;
    [SerializeField] private int coinValue;
    [SerializeField] private Vector2 xSpawnRange;
    [SerializeField] private Vector2 zSpawnRange;

    [SerializeField] private LayerMask layerMask;

    private Collider coinRadius;

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;
        coinRadius = coinPrefab.GetComponent<Collider>();

        for (int i = 0; i < maxCoins; i++)
        {
            SpawnCoin();
        }
    }
    
    private void SpawnCoin()
    {
        CoinRespawning coinInstance =  Instantiate(coinPrefab, GetSpawnPoint(), Quaternion.identity);

        coinInstance.SetValue(coinValue);
        coinInstance.GetComponent<NetworkObject>().Spawn();

        coinInstance.OnCollected += HandleCoinCollected;
    }

    private void HandleCoinCollected(CoinRespawning coin)
    {
        coin.transform.position = GetSpawnPoint();
        coin.Reset();
    }

    private Vector3 GetSpawnPoint()
    {
        float x = 0;
        float z = 0;
        Vector3 spawnPoint = Vector3.zero;
        bool validPosition = false;

        while (!validPosition)
        {
            x = Random.Range(xSpawnRange.x, xSpawnRange.y);
            z = Random.Range(zSpawnRange.x, zSpawnRange.y);
            spawnPoint = new Vector3(x, 1, z);

            // Check if the spawn point is free of collisions
            if (!Physics.CheckSphere(spawnPoint, coinRadius.bounds.extents.magnitude, layerMask))
            {
                validPosition = true;
            }
        }
        return spawnPoint;
    }
}

