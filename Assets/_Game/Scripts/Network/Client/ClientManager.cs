using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class ClientManager : MonoBehaviourPunCallbacks, IConnect
{
    #region Public Methods
    public void Connect()
    {
        if (!ClientServerCommon.Connect())
        {
            PhotonNetwork.JoinRoom(ClientServerCommon.Continent());
        }
    }

    #endregion

    #region Unity Callbacks
    private void Start()
    {
        Connect();
    }
    #endregion

    #region Pun Callbacks
    public override void OnConnectedToMaster()
    {
        print("connected to master");
        PhotonNetwork.JoinLobby(PhotonNetwork.CurrentLobby);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(ClientServerCommon.Map);
        ClientServerCommon.GetPlayersList();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("Error code: " + returnCode + " Error: " + message);
        print("Not connected to any room");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion
}
