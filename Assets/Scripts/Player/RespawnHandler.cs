using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private PlayerTank playerPrefab;
    [SerializeField] private float keptCoinPercentage;
    public override void OnNetworkSpawn()
    {
        if(!IsServer) { return; }

        PlayerTank[] players = FindObjectsByType<PlayerTank>(FindObjectsSortMode.None);

        foreach (PlayerTank player in players)
        {
            HandlePlayerSpawned(player);
        }
        PlayerTank.OnPlayerSpawned += HandlePlayerSpawned;
        PlayerTank.OnPlayerDespawned += HandlePlayerDespawned;
    }


    private void HandlePlayerSpawned(PlayerTank player)
    {
        player.Health.OnDie += (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDie(PlayerTank player)
    {
        int keptCoins = (int)(player.Wallet.TotalCoins.Value * (keptCoinPercentage / 100));

        Destroy(player.gameObject);
        StartCoroutine(RespawnPlayer(player.OwnerClientId, keptCoins));
    }

    private void HandlePlayerDespawned(PlayerTank player)
    {
        player.Health.OnDie -= (health) => HandlePlayerDie(player);
    }

    private IEnumerator RespawnPlayer(ulong ownerClientId, int keptCoins)
    {
        yield return new WaitForSeconds(1);

        PlayerTank playerInstance = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);

        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientId);

        playerInstance.Wallet.TotalCoins.Value += keptCoins;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer){ return; }
        PlayerTank.OnPlayerSpawned -= HandlePlayerSpawned;
        PlayerTank.OnPlayerDespawned -= HandlePlayerDespawned;
    }
}
