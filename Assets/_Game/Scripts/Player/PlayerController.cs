﻿using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Input System Configuration
    private GameControls controls = null;
    public GameControls Controls { get => controls; }

    private void OnEnable() => controls.Game.Enable();
    private void OnDisable() => controls.Game.Disable();
    #endregion

    #region Shared Components
    public Rigidbody2D Rigidbody2D { get => GetComponent<Rigidbody2D>(); }
    public Animator Animator { get => GetComponent<Animator>(); }  
    public CinemachineVirtualCamera VirtualCameraV1 { get => GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();  }
    #endregion

    #region Unity Events
    private void Awake()
    {
        if (controls == null)
        {
            controls = new GameControls();
        }
    }
    #endregion
}
