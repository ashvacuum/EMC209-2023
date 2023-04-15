using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class StoreManager : MonoBehaviour
{
    private List<CatalogItem> _catalog = new List<CatalogItem>();

    public event Action PurchaseSuccessfulEvent;

    private const string catalogversion = "Test";

    public void Init()
    {
        PlayFabClientAPI.GetCatalogItems( new GetCatalogItemsRequest()
        {
            CatalogVersion = catalogversion
        }, (success) =>
        {
            _catalog = success.Catalog;


            if (success.Catalog == null)
            {
                Debug.LogError( success.ToJson());
            }
            else
            {
                foreach (var item in success.Catalog)
                {
                    var currencyValues = "";

                    if (item.VirtualCurrencyPrices != null)
                    {
                        currencyValues = item.VirtualCurrencyPrices.Aggregate(currencyValues, (current, keyValuePair) => current + $"\nCurrency:{keyValuePair.Key} Price:{keyValuePair.Value}");
                    }

                    Debug.Log($"Name:{item.DisplayName} \nDescription:{item.Description} {currencyValues}");
                }
            }

        }, (fail) =>
        {
            
        });
    }
    
    public void PurchaseItem(string itemId, CurrencyManager.VirtualCurrency currency = CurrencyManager.VirtualCurrency.VB)
    {
        var itemPurchase = _catalog.FirstOrDefault(s => s.ItemId == itemId);
        
        if (itemPurchase == null)
        {
            Debug.LogError($"Item: {itemId} not found");
            return;
        }

        var currencyToUse = currency.ToString();
        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest()
        {
            CatalogVersion = catalogversion,
            ItemId = itemPurchase.ItemId,
            Price = (int)itemPurchase.VirtualCurrencyPrices[currencyToUse],
            VirtualCurrency = currencyToUse
        }, (success) =>
        {
            Debug.Log("Purchase Successful");
            PurchaseSuccessfulEvent?.Invoke();
            
            
        }, (fail) =>
        {
            Debug.Log("Purchase Failed");
        });
    }
}
