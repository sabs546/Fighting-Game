using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Image whiteout;
    [SerializeField]
    private Button HostButton;
    [SerializeField]
    private Button JoinButton;
    [SerializeField]
    private TextMeshProUGUI roomName;

    public void ServerDisconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public void ServerConnect()
    {
        PhotonNetwork.ConnectUsingSettings();
        roomName.text = "Connecting...";
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        whiteout.color = Color.white;
        roomName.text = "Lobby";
        HostButton.interactable = true;
        JoinButton.interactable = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        if (cause == DisconnectCause.MaxCcuReached)
        {
            whiteout.color = Color.red;
            roomName.text = "Failed to connect";
        }
    }
}
