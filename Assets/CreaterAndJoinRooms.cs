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
    private TextMeshProUGUI roomInput;
    [SerializeField]
    private TextMeshProUGUI roomName;

    [Header("Buttons")]
    [SerializeField]
    private Button searchButton;
    [SerializeField]
    private Button startButton;
    [SerializeField]
    private Button leaveButton;

    [Header("Objects")]
    [SerializeField]
    private PlayerController p1Owner;
    [SerializeField]
    private PlayerController p2Owner;
    [SerializeField]
    private GameObject clock;

    private bool host;

    [SerializeField]
    private Image whiteout;
    [SerializeField]
    private GameObject roomListBox;

    SettingsStorage settingsBackup;

    public void CreateRoom()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2;
        options.IsVisible = true;
        if (PhotonNetwork.JoinOrCreateRoom(roomInput.text, options, TypedLobby.Default))
        {
            leaveButton.interactable = true;
            WorldRules.online = true;
        }
    }

    public void JoinRoom(string roomName)
    {
        if (PhotonNetwork.JoinRoom(roomName))
        {
            leaveButton.interactable = true;
            WorldRules.online = true;
        }
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            if (!host)
            {
                HealthManager p1HP = p1Owner.GetComponent<HealthManager>();
                HealthManager p2HP = p2Owner.GetComponent<HealthManager>();
                p1HP.maxHealth = settingsBackup.MaxHealth;
                p2HP.maxHealth = settingsBackup.MaxHealth;
                p1HP.ResetHealth();
                p2HP.ResetHealth();

                WorldRules.roundLimit = settingsBackup.RoundLimit;
                WorldRules.roundTimer = settingsBackup.RoundTimer;
            }

            PhotonNetwork.LeaveRoom();
        }
        startButton.interactable = false;
        leaveButton.interactable = false;
        WorldRules.online = false;

        searchButton.interactable = true;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully connected");
        whiteout.color = Color.white;
        roomName.text = "Room: " + PhotonNetwork.CurrentRoom.Name;
        searchButton.interactable = false;
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            host = true;
            p1Owner.SetView();
        }
        else
        {
            host = false;
            p2Owner.SetView();
            p2Owner.SwapOfflineInputs();
        }

        foreach (Transform child in roomListBox.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);

        if (!PhotonNetwork.InRoom)
        {
            whiteout.color = Color.red;
        }
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
        GetComponent<PhotonView>().RPC("RPC_SetRoomConditions",
                                       PhotonNetwork.PlayerListOthers[0],
                                       p1Owner.GetComponent<HealthManager>().maxHealth,
                                       WorldRules.roundLimit,
                                       WorldRules.roundTimer,
                                       clock.activeSelf);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!host) LeaveRoom();
        startButton.interactable = false;
    }

    [PunRPC]
    private void RPC_SetRoomConditions(int maxHealth, int roundLimit, float roundTimer, bool activeClock)
    {
        HealthManager p1HP = p1Owner.GetComponent<HealthManager>();
        HealthManager p2HP = p2Owner.GetComponent<HealthManager>();

        settingsBackup.MaxHealth = p1HP.maxHealth;
        settingsBackup.MaxHealth = p2HP.maxHealth;
        p1HP.maxHealth = maxHealth;
        p2HP.maxHealth = maxHealth;
        p1HP.ResetHealth();
        p2HP.ResetHealth();

        settingsBackup.RoundLimit = WorldRules.roundLimit;
        settingsBackup.RoundTimer = WorldRules.roundTimer;
        settingsBackup.ClockActive = clock.activeSelf;
        WorldRules.roundLimit = roundLimit;
        WorldRules.roundTimer = roundTimer;
        clock.SetActive(activeClock);
    }
}

struct SettingsStorage
{
    public int MaxHealth;
    public int RoundLimit;
    public float RoundTimer;
    public bool ClockActive;
}