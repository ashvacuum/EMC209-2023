using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private float _currentHealth;
    [SerializeField] private BOOL _equipment;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_currentHealth);
            stream.SendNext(_equipment);
        } else
        {
            _currentHealth = (float)stream.ReceiveNext();
            _equipment = (string)stream.ReceiveNext();
        }
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {

        }
    }
}
