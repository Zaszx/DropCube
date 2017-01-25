using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveData
{
    public int level;
    public bool musicEnabled;
    public bool fxEnabled;

    public void Init()
    {
        Debug.Log(Application.persistentDataPath);
        string saveLocation = Application.persistentDataPath + "/save.txt";
        if (File.Exists(saveLocation))
        {
            string[] allLines = File.ReadAllLines(saveLocation);
            level = int.Parse(allLines[1]);
            musicEnabled = bool.Parse(allLines[2]);
            fxEnabled = bool.Parse(allLines[3]);
        }
        else
        {
            level = 0;
            musicEnabled = true;
            fxEnabled = true;
        }
    }

    public void Write()
    {
        string saveLocation = Application.persistentDataPath + "/save.txt";
        string[] allLines = new string[4];
        allLines[0] = "*** WARNING: DO NOT MODIFY THIS FILE, OR YOU MAY BREAK THE GAME ***";
        allLines[1] = level.ToString();
        allLines[2] = musicEnabled.ToString();
        allLines[3] = fxEnabled.ToString();

        File.WriteAllLines(saveLocation, allLines);
    }
}
