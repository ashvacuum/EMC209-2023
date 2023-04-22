using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        var pv = col.gameObject.GetComponent<PhotonView>();
        if(pv != null);
        {
            if (pv.IsMine)
            {
                pv.RPC(nameof(LoseCondition), RpcTarget.Others);
                
                PlayFabClientAPI.UpdatePlayerStatistics( new UpdatePlayerStatisticsRequest()
                {
                     Statistics  = { new StatisticUpdate() {StatisticName = "Wins", Value = 1, Version = 0}}
                }, (updateSuccess) =>
                {
                    
                }, (updateFailure) => { }); 
                
            }
                
        }
    }

    [PunRPC]
    private void LoseCondition()
    {
        PlayFabClientAPI.UpdatePlayerStatistics( new UpdatePlayerStatisticsRequest()
        {
            Statistics  = { new StatisticUpdate() {StatisticName = "Failures", Value = 1, Version = 0}}
        }, (updateSuccess) =>
        {
                    
        }, (updateFailure) => { });      
    }
}
