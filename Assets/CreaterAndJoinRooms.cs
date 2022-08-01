using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class CreaterAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI createInput;
    public TextMeshProUGUI joinInput;
    public Button enableButton;
    private bool host;
    public PlayerController p2Owner;

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text);
        host = true;
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
        host = false;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully connected");
        enableButton.interactable = true;
        if (!host) p2Owner.SwapOfflineInputs();
    }
}
