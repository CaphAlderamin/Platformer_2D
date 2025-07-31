using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogWindow : MonoBehaviour
{
    [SerializeField] Text txContent;
    [SerializeField] Text txButton0;
    [SerializeField] Text txButton1;
    [SerializeField] Text txButton2;
    [SerializeField] Button bt0;
    [SerializeField] Button bt1;
    [SerializeField] Button bt2;

    Action<DialogResult> onClosed;

    //static DialogWindow Instance;

    //static void InitIfNeeded()
    //{
    //    if (!Instance)
    //        Instance = Instantiate(Resources.Load<DialogWindow>("DialogWindow"));
    //    if (!Instance)
    //        throw new System.Exception("Can not load DialogWindow from resource folder");
    //}

    //public static void ShowDialog(string content, string okText = "Ok", string cancelText = null, string ignoreText = null, Action<DialogResult> onClosed = null)
    //{
    //    //InitIfNeeded();
    //    Instance.Show(content, okText, cancelText, ignoreText, onClosed);
    //}

    public void Show(string content, string okText = "Ok", string cancelText = null, string ignoreText = null, Action<DialogResult> onClosed = null)
    {
        gameObject.SetActive(true);
        txContent.text = content;
        txButton0.text = okText;
        txButton1.text = cancelText;
        txButton2.text = ignoreText;
        bt0.gameObject.SetActive(!string.IsNullOrWhiteSpace(okText));
        bt1.gameObject.SetActive(!string.IsNullOrWhiteSpace(cancelText));
        bt2.gameObject.SetActive(!string.IsNullOrWhiteSpace(ignoreText));
        this.onClosed = onClosed;
    }

    public void OnButtonClick(int dialogResult)
    {
        gameObject.SetActive(false);
        onClosed?.Invoke((DialogResult)dialogResult);
    }
}

public enum DialogResult
{
    Ok,     //0
    Cancel, //1
    Ignore  //2
}
