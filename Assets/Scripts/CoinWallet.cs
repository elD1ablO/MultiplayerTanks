using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Coin>(out Coin coin)) return;
        
        int collectedCoin = coin.Collect();
        
        if (!IsServer) return;

        TotalCoins.Value += collectedCoin;
    }
}
