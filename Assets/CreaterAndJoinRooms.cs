using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class CreaterAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI createInput;
    public TextMeshProUGUI joinInput;
    public Button startButton;
    public Button leaveButton;
    private bool host;
    public PlayerController p1Owner;
    public PlayerController p2Owner;

    [SerializeField]
    private Image whiteout;

    public void CreateRoom()
    {
        if (PhotonNetwork.CreateRoom(createInput.text))
        {
            host = true;
            p1Owner.SetView();
            leaveButton.interactable = true;
            whiteout.color = Color.white;
        }
        else
        {
            whiteout.color = Color.red;
        }
    }

    public void JoinRoom()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.LeaveRoom();
        }

        if (PhotonNetwork.JoinRoom(joinInput.text))
        {
            host = false;
            p2Owner.SetView();
            leaveButton.interactable = true;
            whiteout.color = Color.white;
        }
        else
        {
            whiteout.color = Color.red;
        }
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.LeaveRoom();
        }
        leaveButton.interactable = false;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully connected");
        if (!host) p2Owner.SwapOfflineInputs();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        startButton.interactable = true;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        startButton.interactable = false;
    }
}
