using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonManager : Photon.MonoBehaviour
{
    public bool AutoConnect = true;
    public byte Version = 1;
    private bool ConnectInUpdate = true;

    public Transform spawnPoint;
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
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions() { MaxPlayers = 4 }, TypedLobby.Default);
    }

    public virtual void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, 
            new Vector3(Random.Range(-19, 14), (Random.Range(1, -14))), Quaternion.identity, 0);
    }

}
