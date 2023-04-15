using System.Collections;
using System.Collections.Generic;
using PlayFab;
using UnityEngine;

public class GameplayExample : MonoBehaviour
{
    [ContextMenu("Buy Potion")]
    private void BuyPotion()
    {
        PlayfabManager.Instance.StoreManager.PurchaseItem("HealthPotion");
    }

    [ContextMenu("Lose HP")]
    private void LoseHP()
    {
        PlayfabManager.Instance.CurrencyManager.SubtractCurrency(CurrencyManager.VirtualCurrency.HP,1);
    }
    
    [ContextMenu("Use Potion")]
    private void UsePotion()
    {
        PlayfabManager.Instance.InventoryManager.UseItem("HealthPotion", () =>
        {
            PlayfabManager.Instance.CurrencyManager.AddCurrency(CurrencyManager.VirtualCurrency.HP, 1);
        });
    }

    [ContextMenu("Give Potion Money")]
    private void GiveMoneyForPotion()
    {
        
        PlayfabManager.Instance.CurrencyManager.AddCurrency(CurrencyManager.VirtualCurrency.VB, 500);
    }

}
