using UnityEngine;

public class CoinBounty : Coin
{
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

        Destroy(gameObject);

        return coinValue;
    }

}
