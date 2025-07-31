 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatUpTrigger : MonoBehaviour
{
    public void OpenStatUpMenu()
    {
        GameMenuController.Instance.OpenStatsUp();
    }
}
