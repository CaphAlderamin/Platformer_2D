using UnityEngine;

public class DialogueBtnNextSentences : MonoBehaviour
{
    public DialogueManager dm;
    void Update()
    {
        if (Input.GetButtonDown("Use Button") && dm.InDialog)
        {
            Debug.Log("F");
            dm.DisplayNextSentence();
        }
    }
}
