using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StatusWhenConnecting : MonoBehaviour
{
    public GUISkin Skin;

    void OnGUI()
    {
        if (Skin != null)
        {
            GUI.skin = Skin;
        }

        float width = 400;
        float height = 100;

        Rect centeredRect = new Rect((Screen.width - width) / 2, (Screen.height - height) / 2, width, height);

        GUILayout.BeginArea(centeredRect, GUI.skin.box);
        {
            GUILayout.Label("Connecting" + GetConnectingDots(), GUI.skin.customStyles[0]);
            GUILayout.Label("Status: " + PhotonNetwork.IsConnected);
        }
        GUILayout.EndArea();

        if (PhotonNetwork.InRoom)
        {
            enabled = false;
        }
    }

    string GetConnectingDots()
    {
        string str = "";
        int numberOfDots = Mathf.FloorToInt(Time.timeSinceLevelLoad * 3f % 4);

        for (int i = 0; i < numberOfDots; ++i)
        {
            str += " .";
        }

        return str;
    }
}
