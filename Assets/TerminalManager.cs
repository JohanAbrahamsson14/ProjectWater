using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TerminalManager : MonoBehaviour
{
    public GameObject directoryLine;
    public GameObject responseLine;
    
    public TMP_InputField terminalInput;
    public GameObject UserInputLine;
    public ScrollRect sr;
    public GameObject msgList;

    private void OnGUI()
    {
        if (terminalInput.isFocused && terminalInput.text != "" && Input.GetKey(KeyCode.Return))
        {
            //Store all functions called by the user
            string userInput = terminalInput.text;

            //Clear the input field
            ClearInputField();
            
            //Instansite a GameObject with a directory prefix
            AddDirectoryLine(userInput);
            
            //Move the user input line to the end
            UserInputLine.transform.SetAsLastSibling();
            
            //Refocus the input field
            terminalInput.ActivateInputField();
            terminalInput.Select();
        }
    }

    private void AddDirectoryLine(string userInput)
    {
        //Resizing the command line Container.
        Vector2 msgListSize = msgList.GetComponent<RectTransform>().sizeDelta;
        msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(msgListSize.x, msgListSize.y + 35.0f);
        
        //Instantiate the directory line.
        GameObject msg = Instantiate(directoryLine, msgList.transform);
        
        //Set its child index.
        msg.transform.SetSiblingIndex(msgList.transform.childCount - 1);
        
        //Set the text of this new gameObject.
        msg.GetComponentsInChildren<TextMeshProUGUI>()[1].text = userInput;
    }

    private void ClearInputField()
    {
        terminalInput.text = "";
    }
}
