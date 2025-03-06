using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealingZone : NetworkBehaviour
{
    [SerializeField] private Image healPowerBar;

    [SerializeField] private int maxHealPower = 30;
    [SerializeField] private float healCooldown = 60;
    [SerializeField] private float healTickRate = 1;
    [SerializeField] private int coinsPerTick = 1;
    [SerializeField] private int healPerTick = 10;

    private float remainingCooldown;
    private float tickTimer;

    private List<PlayerTank> tanksOnPad = new List<PlayerTank>();

    private NetworkVariable<int> HealPower = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged += HandleHealPowerChanged;
            HandleHealPowerChanged(0, HealPower.Value);
        }
        if(IsServer)
        {
            HealPower.Value = maxHealPower;
        }
    }
    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged -= HandleHealPowerChanged;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!IsServer) { return; }

        if (!other.TryGetComponent<PlayerTank>(out PlayerTank player)) { return; }
        
        tanksOnPad.Add(player);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) { return; }

        if (!other.TryGetComponent<PlayerTank>(out PlayerTank player)) { return; }

        tanksOnPad.Remove(player);
    }
    private void Update()
    {
        if (!IsServer) return;
        if (remainingCooldown > 0f)
        {
            remainingCooldown -= Time.deltaTime;

            if (remainingCooldown <= 0f)
            {
                HealPower.Value = maxHealPower;
            }
            else
            {
                return;
            }
        }

        tickTimer += Time.deltaTime;
        if (tickTimer >= 1/ healTickRate)
        {
            foreach (PlayerTank player in tanksOnPad)
            {
                if (HealPower.Value == 0) break;
                if (player.Health.CurrentHealth.Value == player.Health.MaxHealth) continue;
                if (player.Wallet.TotalCoins.Value < coinsPerTick) continue;

                player.Wallet.SpendCoins(coinsPerTick);
                player.Health.RestoreHealth(healPerTick);

                HealPower.Value -= 1;
                if(HealPower.Value == 0)
                {
                    remainingCooldown = healCooldown;
                }
            }

            tickTimer = tickTimer % (1 / healTickRate);
        }
    }
    private void HandleHealPowerChanged(int oldHealPower, int newHealPower)
    {
        healPowerBar.fillAmount = (float) newHealPower / maxHealPower;
    }
}
