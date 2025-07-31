using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseAnimator : MonoBehaviour
{
    [SerializeField] private Animator startAnim;
    [SerializeField] private bool IsDialog = false;
    [SerializeField] private DialogueManager dm;

    private void OnValidate()
    {
        //startAnim = GameObject.FindGameObjectWithTag("InteractionObject").GetComponent<Animator>();

        GameObject interactionObject = GameObject.FindWithTag("InteractionObject");
        if (interactionObject != null)
        {
            startAnim = interactionObject.GetComponent<Animator>();
        }
        else
        {
            startAnim = null;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player" && col is Collider2D)
        {
            startAnim.SetBool("startOpen", true);
            PlayerManager.Instance.CollideGameObject = gameObject;
        } 
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player" && col is Collider2D)
        {
            startAnim.SetBool("startOpen", false);
            PlayerManager.Instance.CollideGameObject = null;
            if (IsDialog && dm != null)
            {
                dm.EndDialogue();
            }
        } 
    }
}
