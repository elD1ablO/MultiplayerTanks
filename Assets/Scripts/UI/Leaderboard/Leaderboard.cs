using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform leaderboardEntityHolder;
    [SerializeField] private LeaderboardEntity leaderboardEntityPrefab;
    [SerializeField] private int entitiesToDisplay = 8;

    private NetworkList<LeaderboardEntityState> leaderboardEntities;

    private List<LeaderboardEntity> entityDisplays = new List<LeaderboardEntity>();
    private void Awake()
    {
        leaderboardEntities = new NetworkList<LeaderboardEntityState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged += HandleLeaderboardChanged;
            foreach (LeaderboardEntityState entity in leaderboardEntities)
            {
                HandleLeaderboardChanged(new NetworkListEvent<LeaderboardEntityState>
                {
                    Type = NetworkListEvent<LeaderboardEntityState>.EventType.Add,
                    Value = entity
                });
            }
        }
        if (IsServer)
        {
            PlayerTank[] players = FindObjectsByType<PlayerTank>(FindObjectsSortMode.None);

            foreach (PlayerTank player in players)
            {
                HandlePlayerSpawned(player);
            }
            PlayerTank.OnPlayerSpawned += HandlePlayerSpawned;
            PlayerTank.OnPlayerDespawned += HandlePlayerDespawned;
        }
    }


    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged -= HandleLeaderboardChanged;
        }

        if (IsServer)
        {
            PlayerTank.OnPlayerSpawned -= HandlePlayerSpawned;
            PlayerTank.OnPlayerDespawned -= HandlePlayerDespawned;
        }
    }
    private void HandleLeaderboardChanged(NetworkListEvent<LeaderboardEntityState> changeEvent)
    {
        switch (changeEvent.Type)
        {
            case NetworkListEvent<LeaderboardEntityState>.EventType.Add:
                if(!entityDisplays.Any(x => x.ClientID == changeEvent.Value.ClientID))
                {
                    LeaderboardEntity leaderboardEntity = Instantiate(leaderboardEntityPrefab, leaderboardEntityHolder);
                    leaderboardEntity.Initialize(changeEvent.Value.ClientID, changeEvent.Value.PlayerName, changeEvent.Value.Coins);
                    entityDisplays.Add(leaderboardEntity);
                }
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                LeaderboardEntity displayToRemove = entityDisplays.FirstOrDefault(x => x.ClientID == changeEvent.Value.ClientID);
                if (displayToRemove != null)
                {
                    displayToRemove.transform.SetParent(null);
                    Destroy(displayToRemove.gameObject);
                    entityDisplays.Remove(displayToRemove);
                }
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Value:
                LeaderboardEntity displayToUpdate = entityDisplays.FirstOrDefault(x => x.ClientID == changeEvent.Value.ClientID);
                if (displayToUpdate != null)
                {
                    displayToUpdate.UpdateCoins(changeEvent.Value.Coins);
                }
                break;
        }

        entityDisplays.Sort((x, y) => y.Coins.CompareTo(x.Coins));

        for (int i = 0; i < entityDisplays.Count; i++)
        {
            entityDisplays[i].transform.SetSiblingIndex(i);
            entityDisplays[i].UpdateText();

            bool shouldShow = i <= entitiesToDisplay - 1;
            entityDisplays[i].gameObject.SetActive(shouldShow);
        }

        LeaderboardEntity myDisplay = entityDisplays.FirstOrDefault(x => x.ClientID == NetworkManager.Singleton.LocalClientId);
        if (myDisplay != null)
        {
            if(myDisplay.transform.GetSiblingIndex() >= entitiesToDisplay)
            {
                leaderboardEntityHolder.GetChild(entitiesToDisplay - 1).gameObject.SetActive(false);
                myDisplay.gameObject.SetActive(true);
            }
        }
    }
    
    private void HandlePlayerSpawned(PlayerTank player)
    {
        leaderboardEntities.Add(new LeaderboardEntityState
        {
            ClientID = player.OwnerClientId,
            PlayerName = player.PlayerName.Value,
            Coins = 0
        });

        player.Wallet.TotalCoins.OnValueChanged += (oldCoins, newCoins) => HandleCoinsChanged(player.OwnerClientId, newCoins);
    }
    private void HandlePlayerDespawned(PlayerTank player)
    {
        if (leaderboardEntities == null) return;

        foreach (LeaderboardEntityState entity in leaderboardEntities)
        {
            if(entity.ClientID != player.OwnerClientId) continue; 

            leaderboardEntities.Remove(entity);
            break;
        }
        player.Wallet.TotalCoins.OnValueChanged -= (oldCoins, newCoins) => HandleCoinsChanged(player.OwnerClientId, newCoins);
    }

    private void HandleCoinsChanged(ulong clientID, int newCoins)
    {
        for (int i = 0; i < leaderboardEntities.Count; i++)
        {
            if (leaderboardEntities[i].ClientID != clientID) continue;
            leaderboardEntities[i] = new LeaderboardEntityState
            {
                ClientID = leaderboardEntities[i].ClientID,
                PlayerName = leaderboardEntities[i].PlayerName.Value,
                Coins = newCoins
            };
            return;
        }
    }
}
