using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using System.IO;

public class RoomListing : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] panelItemText;
    private RoomInfo _roomInfo;

    public RoomInfo RoomInfo { get; private set; }

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        _roomInfo = roomInfo;
        panelItemText[0].text = roomInfo.Name;
        panelItemText[1].text = "ping";
        panelItemText[2].text = roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;
        gameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate { EnterRoom(roomInfo.Name); });
    }

    private void EnterRoom(string room) => PhotonNetwork.JoinRoom(room);

    public virtual void OnJoinedRoom()
    {
        // look for randomize terrain instantiation
        var player = PhotonNetwork.Instantiate("Ava", new Vector3(4,6,0), Quaternion.identity) as GameObject;

        if (player)
        {
            player.GetComponent<PlayerController>().VirtualCameraV1.Follow = player.transform; 
        }
    }

}
