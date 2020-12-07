using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;

public static class ClientServerCommon
{
    #region Properties
    public static string Map { get => "Map"; }
    public static List<Player> Players { get; set; }
    #endregion


    #region Public Methods
    public static AppSettings AppSettings()
    {
        var appSettings = new AppSettings()
        {
            AppVersion = "1",
            AppIdRealtime = "6f07d691-a728-4df4-8ba6-7b2250b75480",
        };
        return appSettings;
    }

    public static string Continent()
    {
        string continent = string.Empty;

        switch (PhotonNetwork.CloudRegion)
        {
            case "usw":
                continent = "USA, West";
                break;
            case "sa":
                continent = "South America, Sao paulo";
                break;
        }

        return continent;
    }

    public static bool Connect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings(AppSettings());
            return true;
        }
        return false;
    }

    public static void GetPlayersList() => Players = PhotonNetwork.PlayerList.ToList();
    #endregion
}
