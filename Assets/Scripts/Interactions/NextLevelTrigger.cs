using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    public void NextLevel()
    {
        GameManager.Instance.NextLevel();
    }
}
