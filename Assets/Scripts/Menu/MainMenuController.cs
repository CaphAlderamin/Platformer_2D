using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Text;

public class MainMenuController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject LoadMenu;
    [SerializeField] private GameObject CreateLoadMenu;
    [SerializeField] private DialogWindow dialogWindow;

    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject SaveRowPrefab;

    [SerializeField] private InputField InputText;
    [SerializeField] private Text ErrorText;

    //[NonSerialized] 
    public string SaveFileNameString;

    public int index = 1;
    
    private void Start()
    {
        Cursor.visible = true;
        ErrorText.text = "";
        MainMenu.SetActive(true);
    }

    //Main Menu buttons
    public void LoadGameMenu()
    {
        index++;
        
        MainMenu.SetActive(false);
        LoadMenu.SetActive(true);

        UpdateContext();
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    //Load Menu buttons
    public void CreateSaveMenu()
    {
        index++;
        CreateLoadMenu.SetActive(true);
    }
    public void Load()
    {
        if (SaveFileNameString != null && SaveFileNameString != "")
        {
            LoadSave(SaveFileNameString, false);
        }
        else
        {
            Debug.Log("Save is not selected");
            ErrorText.text = "Save is not selected";
        }
    }
    public void Back()
    {
        if (index == 2)
        {
            MainMenu.SetActive(true);
            LoadMenu.SetActive(false);
            CreateLoadMenu.SetActive(false);
        }
        if (index == 3)
        {
            LoadMenu.SetActive(true);
            CreateLoadMenu.SetActive(false);
        }
        index--;
        ErrorText.text = null;
        InputText.text = null;
    }

    //functions
    public void LoadSave(string SaveFileNameString, bool NewSaveFile)
    {
        Debug.Log("load save");
        DataHolder.SaveFileNameString = SaveFileNameString;
        DataHolder.NewSaveFile = NewSaveFile;
        DataHolder.CurrentLevel = 0;
        Cursor.visible = false;
        SceneManager.LoadScene("Level0");
    }
    public void CreateSave()
    {
        if (InputText.text != null && InputText.text != "")
        {
            if (IsValidFilename(InputText.text))
            {
                string path = Application.persistentDataPath + "/" + InputText.text + ".veas";
                if (!File.Exists(path))
                {
                    FileStream stream = new FileStream(path, FileMode.Create);
                    stream.Close();
                }
                else
                {
                    Debug.Log("File name already exists");
                    ErrorText.text = "Save with this name is already exists";
                    return;
                }
                    
                if (File.Exists(path))
                {
                 //   byte[] bytes = {	// Offset 0x00000000 to 0x000000EC
	             //   0x00, 0x01, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0C, 0x02, 0x00, 0x00, 0x00, 0x46, 0x41, 0x73, 0x73, 0x65, 0x6D, 0x62, 0x6C, 0x79, 0x2D, 0x43, 0x53, 0x68, 0x61, 0x72, 0x70, 0x2C, 0x20, 0x56, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x3D, 0x30, 0x2E, 0x30, 0x2E, 0x30, 0x2E, 0x30, 0x2C, 0x20, 0x43, 0x75, 0x6C, 0x74, 0x75, 0x72, 0x65, 0x3D, 0x6E, 0x65, 0x75, 0x74, 0x72, 0x61, 0x6C, 0x2C, 0x20, 0x50, 0x75, 0x62, 0x6C, 0x69, 0x63, 0x4B, 0x65, 0x79, 0x54, 0x6F, 0x6B, 0x65, 0x6E, 0x3D, 0x6E, 0x75, 0x6C, 0x6C, 0x05, 0x01, 0x00, 0x00, 0x00, 0x0A, 0x50, 0x6C, 0x61, 0x79, 0x65, 0x72, 0x44, 0x61, 0x74, 0x61, 0x03, 0x00, 0x00, 0x00, 0x19, 0x50, 0x6C, 0x61, 0x79, 0x65, 0x72, 0x43, 0x75, 0x72, 0x72, 0x65, 0x6E, 0x74, 0x48, 0x65, 0x61, 0x6C, 0x74, 0x68, 0x50, 0x6F, 0x69, 0x6E, 0x74, 0x73, 0x10, 0x50, 0x6C, 0x61, 0x79, 0x65, 0x72, 0x43, 0x6F, 0x69, 0x6E, 0x41, 0x6D, 0x6F, 0x75, 0x6E, 0x74, 0x0A, 0x53, 0x74, 0x61, 0x74, 0x4C, 0x65, 0x76, 0x65, 0x6C, 0x73, 0x00, 0x00, 0x07, 0x0B, 0x08, 0x08, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x48, 0x42, 0x00, 0x00, 0x00, 0x00, 0x09, 0x03, 0x00, 0x00, 0x00, 0x0F, 0x03, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0B};
                 //   stream.Write(bytes, 0, bytes.Length);
                 //   stream.Close();

                    Debug.Log("Save created");
                    LoadSave(InputText.text, true);
                }
                else
                {
                    Debug.LogError("Save file is not found in" + path);
                    ErrorText.text = "File is not found";
                }
            }
            else
            {
                Debug.Log("Save name is not corrected");
                ErrorText.text = "Enter another save name";
            }
        }
        else
        {
            Debug.Log("Save name is not entered");
            ErrorText.text = "Enter new save name";
        }
    }
    public void DeleteSave()
    {
        if (SaveFileNameString != null && SaveFileNameString != "")
        {
            dialogWindow.Show("You want to delete save: " + SaveFileNameString + " ?", "Yes", "No", "Cancel", onClosed: (res) =>
            {
                switch (res)
                {
                    case DialogResult.Ok:
                        string path = Application.persistentDataPath + "/" + SaveFileNameString + ".veas";
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                            UpdateContext();
                        }
                        else
                        {
                            Debug.LogError("Save file is not found in" + path);
                            ErrorText.text = "File is not found";
                        }
                        break;
                    case DialogResult.Cancel:
                        Debug.Log("User press NO");
                        break;
                    case DialogResult.Ignore:
                        Debug.Log("User press Cancel");
                        break;
                }
            });
        }
        else
        {
            Debug.Log("Save is not selected");
            ErrorText.text = "Save is not selected";
        }
    }

    public void UpdateContext()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/");
        FileInfo[] SaveFileNameStrings = dir.GetFiles("*.veas", SearchOption.TopDirectoryOnly);
        for (int i = 0; i < SaveFileNameStrings.Length; i++)
        {
            //scrollRect.content.transform.GetChild(i).gameObject.SetActive(false);

            if (scrollRect.content.transform.childCount - 1 >= i)
            {
                scrollRect.content.transform.GetChild(i).GetComponent<Text>().text = SaveFileNameStrings[i].Name.Replace(".veas", "");
                scrollRect.content.transform.GetChild(i).gameObject.SetActive(true);
            }
            else if (scrollRect.content.transform.childCount - 1 < i)
            {
                GameObject cln = Instantiate(SaveRowPrefab, scrollRect.content.transform);
                cln.GetComponent<Text>().text = SaveFileNameStrings[i].Name.Replace(".veas", "");
            }
        }
        for (int i = SaveFileNameStrings.Length; i < scrollRect.content.transform.childCount; i++)
        {
            scrollRect.content.transform.GetChild(i).gameObject.SetActive(false);
        }

        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, scrollRect.content.childCount * 38);
    }
    bool IsValidFilename(string testName)
    {
        if (testName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
        {
            return false;
        }
        else return true;
    }
    public void ToCorrectText()
    {
        InputText.text = InputText.text.ToLower();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        SaveFileNameString = null;
    }
}
