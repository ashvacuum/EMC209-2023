using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private List<ItemInstance> _currentInventory = new List<ItemInstance>();

    public void UpdateInventory(List<ItemInstance> items)
    {
        _currentInventory = items;
    }
    
    public void UseItem(string itemId, Action successCallback)
    {
        var itemToUse = _currentInventory.Find(s => s.ItemId == itemId);
        
        if (itemToUse == null)
        {
            Debug.LogError($"Item: {itemId} not found");
            return;
        }
        
        PlayFabClientAPI.ConsumeItem(new ConsumeItemRequest()
        {
            ItemInstanceId = itemToUse.ItemInstanceId,
            ConsumeCount = 1
        }, (success) =>
        {   
            
            successCallback?.Invoke();
            for( var i = 0; i < _currentInventory.Count; i++)
            {
                if (_currentInventory[i].ItemInstanceId == success.ItemInstanceId)
                {
                    _currentInventory[i].RemainingUses = success.RemainingUses;
                    break;
                }
            }
        }, (fail) =>
        {
            Debug.Log("Item Use Failed");
        });
    }
}
