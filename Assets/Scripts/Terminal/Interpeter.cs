using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Interpeter : MonoBehaviour
{
    private Dictionary<string, string> colors = new Dictionary<string, string>()
    {
        { "black", "#021b21" },
        { "gray", "#555d71" },
        { "red", "#ff5879" },
        { "yellow", "#f2f1b9" },
        { "blue", "#9ed9d8" },
        { "purple", "#d926ff" },
        { "orange", "#ef5847" },
    };
    
    private List<string> response = new List<string>();
    
    public List<string> Interpret(string userInput)
    {
        response.Clear();

        var capitalUserInput = userInput.ToUpper();
        
        string[] args = capitalUserInput.Split();

        if (args[0] == "HELP")
        {
            ListEntry("help", "returns a list of commands");
            ListEntry("stop", "pauses the game.");
            ListEntry("run", "resumes the game.");
            ListEntry("four", "Blah blah blah.");

            return response;
        }

        if (args[0] == "ASCII")
        {
            LoadTitle("ascii.txt", "red", 2);

            return response;
        }
        
        if (args[0] == "RR")
        {
            LoadTitle("rickroll.txt", "red", 2);

            return response;
        }
        
        if (args[0] == "BOOP")
        {
            response.Add("Thanks for using the Terminal!");
            response.Add("Boop!");
            response.Add("Boop!!");
            response.Add("Boop!!!");
            response.Add("Boop!!");
            response.Add("Boop!");

            return response;
        }
        
        response.Add("Command not recognized. Type help for a list of commands.");
        
        return response;
        
    }

    public string ColorString(string s, string color)
    {
        string leftTag = "<color=" + color + ">";
        string rightTag = "</color>";

        return leftTag + s + rightTag;
    }

    public void ListEntry(string a, string b)
    {
        response.Add(ColorString(a, colors["orange"]) + ": " + ColorString(b, colors["yellow"]));
    }

    public void LoadTitle(string path, string color, int spacing)
    {
        StreamReader file = new StreamReader(Path.Combine(Application.streamingAssetsPath, path));

        for (int i = 0; i < spacing; i++)
        {
            response.Add("");
        }

        while (!file.EndOfStream)
        {
            response.Add(ColorString(file.ReadLine(), colors[color]));
        }
        
        for (int i = 0; i < spacing; i++)
        {
            response.Add("");
        }
        
        file.Close();
    }
    
}
