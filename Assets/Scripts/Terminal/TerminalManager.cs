using System;
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

    public Interpeter interpeter;

    private void Awake()
    {
        interpeter = GetComponent<Interpeter>();
    }

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
            
            //Add the interpretation lines
            int lines = AddInterpeterLines(interpeter.Interpret(userInput));
            
            //Scroll to the bottom of the scrollrect
            ScrollToBottom(lines);
            
            //Move the user input line to the end
            UserInputLine.transform.SetAsLastSibling();
            
            //Refocus the input field
            terminalInput.ActivateInputField();
            terminalInput.Select();
        }
    }

    private void ScrollToBottom(int lines)
    {
        if (lines > 4)
        {
            sr.velocity = new Vector2(0, 150*lines);
        }
        else
        {
            sr.verticalNormalizedPosition = 0;
        }
    }

    private int AddInterpeterLines(List<string> interpretation)
    {
        for (int i = 0; i < interpretation.Count; i++)
        {
            //Instantiate the response line.
            GameObject res = Instantiate(responseLine, msgList.transform);
            
            //Set it to the end of all messages
            res.transform.SetAsLastSibling();
            
            //Get the size of the message list, and resize
            Vector2 listSize = msgList.GetComponent<RectTransform>().sizeDelta;
            msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(listSize.x, listSize.y + 35.0f);
            
            //Set the text of this response line to be whaterver the interpreter string is.
            res.GetComponentInChildren<TextMeshProUGUI>().text = interpretation[i];
        }

        return interpretation.Count;
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
