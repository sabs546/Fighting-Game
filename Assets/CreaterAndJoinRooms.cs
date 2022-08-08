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
    [SerializeField]
    private GameObject clock;

    private bool host;

    [SerializeField]
    private Image whiteout;

    SettingsStorage settingsBackup;

    public void CreateRoom()
    {
        if (PhotonNetwork.CreateRoom(createInput.text))
        {
            host = true;
            p1Owner.SetView();
            leaveButton.interactable = true;
            WorldRules.online = true;
        }
    }

    public void JoinRoom()
    {
        if (PhotonNetwork.JoinRoom(joinInput.text))
        {
            host = false;
            p2Owner.SetView();
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
        leaveButton.interactable = false;
        WorldRules.online = false;

        hostButton.interactable = true;
        joinButton.interactable = true;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully connected");
        whiteout.color = Color.white;
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        hostButton.interactable = false;
        joinButton.interactable = false;
        if (!host)
        {
            p2Owner.SwapOfflineInputs();
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