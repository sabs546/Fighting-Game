using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Image whiteout;
    public Button HostButton;
    public Button JoinButton;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        whiteout.color = Color.white;
        Debug.Log("Successfully joined lobby");
        HostButton.interactable = true;
        JoinButton.interactable = true;
    }
}
