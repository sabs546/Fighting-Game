using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class CreaterAndJoinRooms : MonoBehaviourPunCallbacks
{
    [Header("Text")]
    [SerializeField]
    private TextMeshProUGUI createInput;
    [SerializeField]
    private TextMeshProUGUI joinInput;
    [SerializeField]
    private TextMeshProUGUI roomName;

    [Header("Buttons")]
    [SerializeField]
    private Button hostButton;
    [SerializeField]
    private Button joinButton;
    [SerializeField]
    private Button startButton;
    [SerializeField]
    private Button leaveButton;

    [Header("Objects")]
    [SerializeField]
    private PlayerController p1Owner;
    [SerializeField]
    private PlayerController p2Owner;

    private bool host;

    [SerializeField]
    private Image whiteout;

    public void CreateRoom()
    {
        if (PhotonNetwork.CreateRoom(createInput.text))
        {
            host = true;
            p1Owner.SetView();
            leaveButton.interactable = true;
            WorldRules.offline = false;
        }
        else
        {
            whiteout.color = Color.red;
        }
    }

    public void JoinRoom()
    {
        if (PhotonNetwork.JoinRoom(joinInput.text))
        {
            host = false;
            p2Owner.SetView();
            leaveButton.interactable = true;
            WorldRules.offline = false;
        }
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.LeaveRoom();
        }
        leaveButton.interactable = false;
        WorldRules.offline = true;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully connected");
        whiteout.color = Color.white;
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        hostButton.interactable = false;
        joinButton.interactable = false;
        if (!host) p2Owner.SwapOfflineInputs();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        if (!PhotonNetwork.InRoom)
        {
            whiteout.color = Color.red;
        }
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
