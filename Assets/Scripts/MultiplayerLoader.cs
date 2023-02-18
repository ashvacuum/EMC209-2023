using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerLoader : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField _playerName;
    [SerializeField] private Button _connectBtn;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnEnable()
    {
        if(_connectBtn)
        {
            _connectBtn.onClick.AddListener(ConnectToPhoton);
        }
    }

    public override void OnDisable()
    {
        if (_connectBtn)
        {
            _connectBtn.onClick.RemoveListener(ConnectToPhoton);
        }
    }

    private string GetMessage(string message)
    {
        return $"{nameof(MultiplayerLoader)}: {message}";
    }

    private void ConnectToPhoton()
    {
        var name = _playerName.text;
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError(GetMessage("Name is Empty, please assign a name"));
        }

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
            Debug.Log(GetMessage("Joining Random Room"));
        } else
        {
            PhotonNetwork.NickName = name;
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "1";
            Debug.Log(GetMessage($"Connecting To Photon, {name}"));
        }

        _playerName.text = string.Empty;
    }
    #region Connection
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log(GetMessage("Successfully Connected to Master"));
        PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: new RoomOptions { MaxPlayers = 4});
    }

    public override void OnJoinedLobby()
    {
        Debug.Log(GetMessage("Successfully Connected to Lobby"));
    }

    public override void OnConnected()
    {
        Debug.Log(GetMessage("Successfully Connected to Server"));
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError(GetMessage($"Discoonected due to {cause}"));
    }

    
    #endregion

    #region Room Logic
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log(GetMessage(nameof(OnCreatedRoom)));
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(GetMessage(nameof(OnJoinedRoom)));
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(GetMessage(nameof(OnJoinRoomFailed)));
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(GetMessage(nameof(OnJoinRandomFailed)));
    }
    #endregion

}
