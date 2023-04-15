using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    private int _hpAmount;
    private int _vbAmount;

    public event Action CheckBalanceSuccessEvent;
    
    public void CheckBalance()
    {
        PlayFabClientAPI.GetUserInventory( new GetUserInventoryRequest(), (success) =>
        {
            foreach (var keyValuePair in success.VirtualCurrency)
            {
                Init(keyValuePair.Key, keyValuePair.Value);
            }

            CheckBalanceSuccessEvent?.Invoke();
            
            PlayfabManager.Instance.InventoryManager.UpdateInventory(success.Inventory);
            
        }, (failure) =>
        {
            
        });
    }
    
    
    public void Init(string currencyCode, int balance) 
    { 
        switch (currencyCode)
        {
            case "VB":
                
                _vbAmount = balance;
                break;
            case "HP":
                _hpAmount = balance;
                break;
        }

    }

    public void AddCurrency(VirtualCurrency virtualCurrency, int amountToAdd)
    {
        PlayFabClientAPI.AddUserVirtualCurrency(new PlayFab.ClientModels.AddUserVirtualCurrencyRequest()
        {
            Amount = amountToAdd,
            VirtualCurrency = virtualCurrency.ToString()
        }, OnSuccessfulModifyCurrency, OnFailedModifyCurrency);
    }

    public void SubtractCurrency(VirtualCurrency virtualCurrency, int amountToAdd)
    {
        PlayFabClientAPI.SubtractUserVirtualCurrency(new PlayFab.ClientModels.SubtractUserVirtualCurrencyRequest()
        {
            Amount = amountToAdd,
            VirtualCurrency = virtualCurrency.ToString()
        }, OnSuccessfulModifyCurrency, OnFailedModifyCurrency);
    }

    private void OnSuccessfulModifyCurrency(ModifyUserVirtualCurrencyResult result)
    {
          switch (result.VirtualCurrency)
        {
            case "VB":
                
                _vbAmount = result.Balance;
                Debug.Log($"VB:{_vbAmount}");
                break;
            case "HP":
                _hpAmount = result.Balance;
                Debug.Log($"HP:{_hpAmount}");
                break;
        }   
    }

    private void OnFailedModifyCurrency(PlayFabError error)
    {
        Debug.LogError(error.ToString());
    }

    public enum VirtualCurrency
    {
        HP,
        VB
    }

    [ContextMenu("Test Add and Subtract")]
    private void TestSubtract()
    {
        AddCurrency(VirtualCurrency.VB, 1000);
        SubtractCurrency(VirtualCurrency.HP, 1);
    }
}
