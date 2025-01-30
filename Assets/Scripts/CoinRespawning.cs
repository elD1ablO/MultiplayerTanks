using System;
using UnityEngine;

public class CoinRespawning : Coin
{
    public event Action<CoinRespawning> OnCollected;

    private Vector3 previousPosition;

    private void Update()
    {
        if (previousPosition != transform.position)
        {
            Show(true);
        }
        previousPosition = transform.position;
    }
    public override int Collect()
    {
        if (!IsServer)
        {
            Show(false);
            return 0;
        }
        if (alreadyCollected)
        {
            return 0;
        }
        
        alreadyCollected = true;

        OnCollected?.Invoke(this);

        return coinValue;
    }

    public void Reset()
    {
        alreadyCollected = false;
    }
}
