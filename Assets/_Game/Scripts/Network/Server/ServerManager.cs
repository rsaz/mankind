using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Threading;
using System.Collections;
using System.Linq;

public class ServerManager : MonoBehaviourPunCallbacks, IConnect
{
    #region Properties
    [Tooltip("Maximum number of players per server room")]
    [SerializeField] private byte maxPlayersPerRoom;
    [Tooltip("The Ui Text to inform the user about the connection progress")]
    [SerializeField]
    private TextMeshProUGUI feedbackText;
    [Tooltip("Disconnect or Cancel selection")]
    public GameObject popupDisconnect;
    private string masterClientID = string.Empty;
    #endregion

    #region Private Methods
    private void Log(string message)
    {
        // we do not assume there is a feedbackText defined.
        if (feedbackText == null)
        {
            return;
        }

        // add new messages as a new line and at the bottom of the log.
        feedbackText.text += System.Environment.NewLine + message;
        feedbackText.text += System.Environment.NewLine +
            "---------------------------------------------";
    }
    #endregion

    #region Public Methods
    public void Connect()
    {
        if (!ClientServerCommon.Connect())
        {
            popupDisconnect.SetActive(true);
        }
    }

    public void CancelDisconnect() => popupDisconnect.SetActive(false);
    public void DisconnectServer()
    {
        PhotonNetwork.Disconnect();
        CancelDisconnect();
    }
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        maxPlayersPerRoom = 100;
    }
    #endregion

    #region PUN Callbacks
    public override void OnConnected()
    {
        Log("Server Connected: " + PhotonNetwork.IsConnected + " Region: " +
            PhotonNetwork.CloudRegion);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom(ClientServerCommon.Continent(), new RoomOptions { MaxPlayers = maxPlayersPerRoom }, TypedLobby.Default);
        Log("OnConnnectedToMaster(): Join or Creating the Room");
    }

    public override void OnJoinedRoom()
    {
        Log("Connected to Room " + PhotonNetwork.CurrentRoom.Name);
        Log("Is Master Client: " + PhotonNetwork.IsMasterClient);

        ClientServerCommon.GetPlayersList();
        //masterClientID = ClientServerCommon.Players[0].UserId;
        //PhotonNetwork.SetMasterClient(ClientServerCommon.Players[0]);
        print("Current: " + PhotonNetwork.MasterClient.UserId);
       // print(masterClientID);
    }

    // Implement custom master client event to disconnect players from the game
    public override void OnLeftRoom()
    {
        foreach (var player in ClientServerCommon.Players)
        {
            PhotonNetwork.CloseConnection(player);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Log("Server Connected: " + PhotonNetwork.IsConnected + " -> Cause: " + cause.ToString());
    }

    #endregion
}
