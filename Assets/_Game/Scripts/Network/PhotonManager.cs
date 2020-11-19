using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonManager : Photon.MonoBehaviour
{
    public bool AutoConnect = true;
    public byte Version = 1;
    public byte MaxPlayers = 10;
    private bool ConnectInUpdate = true;

    public TerrainManager terrainManager;
    public GameObject playerPrefab;

    public virtual void Start()
    {
        PhotonNetwork.autoJoinLobby = false;
    }

    public virtual void Update()
    {
        if (ConnectInUpdate && AutoConnect && !PhotonNetwork.connected)
        {
            ConnectInUpdate = false;
            PhotonNetwork.ConnectUsingSettings(null); // + "." + SceneManagerHelper.ActiveSceneBuildIndex
        }
    }

    public virtual void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public virtual void OnJoinedLobby()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions() { MaxPlayers = MaxPlayers }, TypedLobby.Default);
    }

    public virtual void OnJoinedRoom()
    {       
        var player = PhotonNetwork.Instantiate(playerPrefab.name, 
            terrainManager.RandomCellPosition(PhotonNetwork.player.ID), Quaternion.identity, 0) as GameObject;

        if (player)
        {
            player.GetComponent<PlayerController>().VirtualCameraV1.Follow = player.transform; 
        }
    }

}
