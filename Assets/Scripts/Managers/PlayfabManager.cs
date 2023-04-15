using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using Photon.Pun;
using UnityEngine.SceneManagement;
using static ExperienceManager;

public class PlayfabManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private LoginPanel _login;
    [SerializeField] private RegistrationPanel _registration;
    [Space(20)]
    [Header("Managers")]
    [SerializeField] private ExperienceManager _experienceManager;
    [SerializeField] private CurrencyManager _currencyManager;
    [SerializeField] private InventoryManager _inventoryManager;
    [SerializeField] private StoreManager _storeManager;
    [Space(20)]
    [Header("Login Config")]
    [SerializeField] private GetPlayerCombinedInfoRequestParams _infoRequestParams;
    
    public static PlayfabManager Instance { get; private set; }
    
    public ExperienceManager ExperienceManager => _experienceManager;
    public CurrencyManager CurrencyManager => _currencyManager;
    public InventoryManager InventoryManager => _inventoryManager;
    public StoreManager StoreManager => _storeManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public string PlayFabId { get; private set; }

    private void OnEnable()
    {
        _login.LoginEvent += OnLoginCalled;
        _registration.RegisterEvent += OnRegisterCalled;

        _storeManager.PurchaseSuccessfulEvent += _currencyManager.CheckBalance;
    }

    private void OnDisable()
    {
        _login.LoginEvent -= OnLoginCalled;
        _registration.RegisterEvent -= OnRegisterCalled;
        
        
        _storeManager.PurchaseSuccessfulEvent -= _currencyManager.CheckBalance;
    }

    private void OnLoginCalled(string userName , string password )
    {
        PlayFabClientAPI.LoginWithEmailAddress(
                new LoginWithEmailAddressRequest()
                {
                    Email = userName,
                    Password = password,
                    TitleId = PlayFabSettings.TitleId,
                    InfoRequestParameters = _infoRequestParams
                    
                },(loginResult) => {
                    Debug.Log("Successfully Logged in");                    
                    if (loginResult.InfoResultPayload != null)
                    {

                        InitializeConfigValues(loginResult.InfoResultPayload);
                    }
                    else
                    {
                        Debug.LogError("Info Result Payload is null");
                    }
                    PhotonNetwork.NickName = userName;
                    //SceneManager.LoadScene("PunBasics-Launcher");
                },(error) => {
                    Debug.LogError($"Failed to login: {error.ErrorMessage}");
                }
            );
    }

    private void OnRegisterCalled(string userName, string password)
    {
        PlayFabClientAPI.LoginWithCustomID(
            new LoginWithCustomIDRequest
            {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true,
                TitleId = PlayFabSettings.TitleId,
                InfoRequestParameters = _infoRequestParams
            }, (loginSuccess) =>
            {
                if (loginSuccess.InfoResultPayload != null)
                {
                    InitializeConfigValues(loginSuccess.InfoResultPayload);
                }
                else
                {
                    Debug.LogError("Info Result Payload is null");
                }
                PlayFabClientAPI.AddUsernamePassword(new AddUsernamePasswordRequest
                {
                    Email = userName,
                    Password = password,
                    Username = loginSuccess.PlayFabId,

                }, (updateSuccess) =>
                    {
                        Debug.Log("Sucessfully Registered");
                        PhotonNetwork.NickName = userName;
                        //SceneManager.LoadScene("PunBasics-Launcher");
                    }, (updateFail) =>
                {
                    var msg = "";
                        foreach (var VARIABLE in updateFail.ErrorDetails)
                        {
                            msg += VARIABLE.Key + "\n";
                            foreach (var item in VARIABLE.Value)
                            {
                                msg += $"#{item}";
                            }
                        }
                        Debug.LogError($"Failed to Register: {updateFail.Error}\n {updateFail.ErrorMessage} \n {msg}");
                    });

            }, (loginFailure) =>
            {
                Debug.LogError($"Unable to Login with custom Id: {loginFailure.ErrorMessage}");
            });
    }

    private void InitializeConfigValues(GetPlayerCombinedInfoResultPayload payload)
    {
        _storeManager.Init();
        
        if (payload.TitleData == null) return;
        var titleData = payload.TitleData;


        if(titleData.TryGetValue("DropAndExpRate", out var totalRates))
        {
            var configRate = JsonUtility.FromJson<DropAndExpRate>(totalRates);
            _experienceManager.Init(configRate);
        }

        if(payload.UserVirtualCurrency != null)
        {
            foreach (var item in payload.UserVirtualCurrency)
            {
                _currencyManager.Init(item.Key, item.Value);
                Debug.Log($"{item.Key}: {item.Value}");
            }
        }

        if (payload.UserInventory != null)
        {
            _inventoryManager.UpdateInventory(payload.UserInventory);
        }

        if(payload.AccountInfo != null)
        {
            PlayFabId = payload.AccountInfo.PlayFabId;
        }
    }

    
    
}
