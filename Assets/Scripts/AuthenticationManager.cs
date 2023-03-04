using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class AuthenticationManager : MonoBehaviour
{
    [SerializeField] private LoginPanel _login;
    [SerializeField] private RegistrationPanel _registration;

    private void OnEnable()
    {
        _login.LoginEvent += OnLoginCalled;
        _registration.RegisterEvent += OnRegisterCalled;
    }

    private void OnDisable()
    {

        _login.LoginEvent -= OnLoginCalled;
        _registration.RegisterEvent -= OnRegisterCalled;
    }

    private void OnLoginCalled(string userName , string password )
    {
        PlayFabClientAPI.LoginWithEmailAddress(
                new LoginWithEmailAddressRequest()
                {
                    Email = userName,
                    Password = password,
                    TitleId = PlayFabSettings.TitleId
                    
                },(loginResult) => {
                    Debug.Log("Successfully Logged in");
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
                CustomId = Guid.NewGuid().ToString(),
                CreateAccount = true,
                TitleId = PlayFabSettings.TitleId
            }, (loginSuccess) =>
            {
                PlayFabClientAPI.AddUsernamePassword(new AddUsernamePasswordRequest
                {
                    Email = userName,
                    Password = password,
                    Username = "testUser2"
                }, (updateSuccess) =>
                    {
                        Debug.Log("Sucessfully Registered");
                    }, (updateFail) =>
                    {
                        Debug.LogError($"Failed to Register: {updateFail.ErrorMessage}");
                    });

            }, (loginFailure) =>
            {
                Debug.LogError($"Unable to Login with custom Id: {loginFailure.ErrorMessage}");
            });
    }
    
}
