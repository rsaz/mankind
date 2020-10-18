using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworking : MonoBehaviour
{
    public MonoBehaviour[] scriptsToIgnore;
    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (!photonView.isMine)
        {
            foreach (var script in scriptsToIgnore)
            {
                script.enabled = false;
            }
        }
    }
}
