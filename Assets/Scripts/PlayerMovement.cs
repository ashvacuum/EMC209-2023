using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviourPun
{
    [SerializeField]private float _gravity = 0.1f;
    [SerializeField] private float _rayDistance = 1.2f;
    [SerializeField] private float _moveSpeed = 5f;
    
    [SerializeField] private List<Color> _playerAssignmentColors;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        var assignment =
            _playerAssignmentColors[Mathf.Min(photonView.Controller.ActorNumber - 1, _playerAssignmentColors.Count - 1)];
        _spriteRenderer.color = assignment;
    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        
        Gravity();
        var horInput = Input.GetAxis("Horizontal");
        var speedMath = horInput * Time.deltaTime * _moveSpeed;
        if (horInput == 0) return;
        var transform1 = this.transform;
        var position = transform1.position;
        transform1.position = new Vector3(position.x + speedMath, position.y, position.z);


        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("I'm pressing space");
            photonView.RPC(nameof(MessageTest), RpcTarget.All, photonView.Controller.NickName);
        }
    }

    private void Gravity()
    {
        var psx = Physics2D.Raycast(this.transform.position, Vector2.down, _rayDistance);
        if (psx.collider == null)
        {
            var position = this.transform.position;
            Debug.DrawRay(position, Vector2.down, Color.green);
            this.transform.position = new Vector3(position.x, position.y - _gravity, position.z );
        }
        else
        {
            Debug.DrawRay(this.transform.position, Vector2.down, Color.red);
        }
    }

    [PunRPC]
    private void MessageTest(string playerName)
    {
        Debug.Log($"{playerName}:Are you ok?");
    }
}
