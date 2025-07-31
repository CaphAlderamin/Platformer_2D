using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseTrigger : MonoBehaviour
{
    public DialogueManager dm;
    public Animator startAnim;
    

    void Update()
    {
        if (Input.GetButtonDown("Use Button") && PlayerManager.Instance.CollideGameObject != null)
        {
            if (startAnim.GetBool("startOpen"))
            {
                if (PlayerManager.Instance.CollideGameObject.CompareTag("NPC"))
                {
                    PlayerManager.Instance.CollideGameObject.GetComponent<DialogueTrigger>().TriggerDialogue();
                    return;
                }

                if (PlayerManager.Instance.CollideGameObject.CompareTag("Chest"))
                {
                    PlayerManager.Instance.CollideGameObject.GetComponent<ChestTrigger>().OpenChest();
                    return;
                }

                if (PlayerManager.Instance.CollideGameObject.CompareTag("StatsUpNPC"))
                {
                    PlayerManager.Instance.CollideGameObject.GetComponent<StatUpTrigger>().OpenStatUpMenu();
                    return;
                }

                if (PlayerManager.Instance.CollideGameObject.CompareTag("NextLevelDoor"))
                {
                    PlayerManager.Instance.CollideGameObject.GetComponent<NextLevelTrigger>().NextLevel();
                    return;
                }
            }
            else if (dm.InDialog)
            {
                dm.DisplayNextSentence();
                return;
            }
        }
    }

}