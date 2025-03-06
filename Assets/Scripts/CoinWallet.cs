using System;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    [SerializeField] private CoinBounty coinBountyPrefab;
    [SerializeField] private Health health;

    [SerializeField] private int bountyCoinCount = 10;
    [SerializeField] private int minBountyCoinValue = 5;
    [SerializeField] private float coinSpread = 3f;
    [SerializeField] private float bountyPercent = 50;

    [SerializeField] private LayerMask layerMask;
    private Collider coinRadius;

    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();


    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;
        coinRadius = coinBountyPrefab.GetComponent<Collider>();
        health.OnDie += HandleDie;
    }
    public override void OnNetworkDespawn()
    {
        health.OnDie -= HandleDie;
    }

    private void HandleDie(Health health)
    {
        int bountyValue = (int) (TotalCoins.Value * (bountyPercent / 100f));
        int bountyCoinValue = bountyValue / bountyCoinCount;

        if (bountyCoinValue < minBountyCoinValue) return;
        for (int i = 0; i < bountyCoinCount; i++)
        {
            CoinBounty coinInstance = Instantiate(coinBountyPrefab, GetSpawnPoint(), Quaternion.identity);
            coinInstance.SetValue(bountyCoinValue);
            coinInstance.NetworkObject.Spawn();
        }

    }
    
    private Vector3 GetSpawnPoint()
    {
        float radius = 3f;
        bool validPosition = false;
        while (true)
        {
            Vector3 randomPoint = UnityEngine.Random.insideUnitSphere * radius;

            // Add the random point to the GameObject's position to get the spawn point
            Vector3 spawnPoint = transform.position + randomPoint;
            spawnPoint.y = 1;
        
            return spawnPoint;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Coin>(out Coin coin)) return;
        
        int collectedCoin = coin.Collect();
        
        if (!IsServer) return;

        TotalCoins.Value += collectedCoin;
    }

    public void SpendCoins(int coins)
    {
        TotalCoins.Value -= coins;
    }
}
