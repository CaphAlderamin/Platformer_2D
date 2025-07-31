using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject Player;

    public StatPanel StatPanel;
    public StatUpPanel StatUpPanel;

    public GameObject CollideGameObject;

    //public string WhatIsTriggered;
}
