using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Image whiteout;
    [SerializeField]
    private Button searchButton;
    [SerializeField]
    private TextMeshProUGUI roomName;
    [SerializeField]
    private GameObject roomListBox;
    [SerializeField]
    private GameObject roomButton;
    private List<RoomInfo> roomListCache;
    public int targetRoomCount { get; private set; }

    [SerializeField]
    private CreaterAndJoinRooms roomActions;

    private void Start()
    {
        roomListCache = new List<RoomInfo>();
        targetRoomCount = -1;
    }

    public void ServerDisconnect()
    {
        PhotonNetwork.Disconnect();
        targetRoomCount = -1;
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
        roomName.text = "Waiting in lobby";
        searchButton.interactable = true;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (targetRoomCount == -1)
        {
            roomListCache = roomList;
            foreach (Transform child in roomListBox.transform)
            {
                Destroy(child.gameObject);
            }

            int i = 0;
            foreach (RoomInfo room in roomList)
            {
                GameObject newButton = roomButton;
                roomButton.GetComponentInChildren<TextMeshProUGUI>().text = room.Name;
                Vector3 pos = roomButton.GetComponent<RectTransform>().anchoredPosition;
                Vector3 buttonPos = new Vector3(pos.x, pos.y - roomButton.GetComponent<RectTransform>().rect.height * i, pos.z);

                roomButton.GetComponent<RectTransform>().anchoredPosition = buttonPos;
                Instantiate(roomButton, roomListBox.transform).GetComponent<Button>().onClick.AddListener(delegate { roomActions.JoinRoom(roomButton.GetComponentInChildren<TextMeshProUGUI>().text); });
                i++;
            }
            targetRoomCount = i;
        }
        else if (!roomListCache.Contains(roomList.First()))
        { // Probably a single room is being added
            int i = targetRoomCount;
            foreach (RoomInfo room in roomList)
            {
                roomButton.GetComponentInChildren<TextMeshProUGUI>().text = room.Name;
                Vector3 pos = roomButton.GetComponent<RectTransform>().anchoredPosition;
                Vector3 buttonPos = new Vector3(pos.x, pos.y - roomButton.GetComponent<RectTransform>().rect.height * i, pos.z);

                roomButton.GetComponent<RectTransform>().anchoredPosition = buttonPos;
                Instantiate(roomButton, roomListBox.transform).GetComponent<Button>().onClick.AddListener(delegate { roomActions.JoinRoom(roomButton.GetComponentInChildren<TextMeshProUGUI>().text); });
                roomListCache.Add(room);
                i++;
            }
            targetRoomCount++;
        }
        else if (roomListCache.Contains(roomList.First()))
        { // Probably a single room is being removed
            RoomInfo room = roomListCache.Find(n => n.Name == roomList.First().Name);
            roomListCache.Remove(room);
            bool shiftDown = false;
            foreach (Transform child in roomListBox.transform)
            {
                if (shiftDown)
                {
                    float newY = child.GetComponent<RectTransform>().anchoredPosition.y + child.GetComponent<RectTransform>().rect.height;
                    child.GetComponent<RectTransform>().anchoredPosition = new Vector2(child.GetComponent<RectTransform>().anchoredPosition.x, newY);
                }
                else if (child.GetComponentInChildren<TextMeshProUGUI>().text == room.Name)
                {
                    Destroy(child.gameObject);
                    shiftDown = true;
                }
            }
            targetRoomCount--;
        }
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
