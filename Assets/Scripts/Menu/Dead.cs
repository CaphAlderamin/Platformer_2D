using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dead : MonoBehaviour
{
    public Animator back;
    public Animator front;
    public Animator EscText;

    private void Start()
    {
        GlobalEventManager.OnHeroDeath.AddListener(DeathGUI);
    }

    private void DeathGUI()
    {
        StartCoroutine(wait());
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(1f);
        back.SetBool("DeadBackRise", true);
        front.SetBool("DeadRise", true);

        //old esc restart
        //yield return new WaitForSeconds(3f);
        //EscText.SetBool("EscRise", true);

        //new auto restart
        yield return new WaitForSeconds(4f);
        GameManager.Instance.Restart();
    }

    
}
