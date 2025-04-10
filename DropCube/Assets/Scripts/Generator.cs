﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.TextMeshPro;

public class Generator : MonoBehaviour 
{
    public Scene scene;
    public Slider complexitySlider;
    public TMP_Text solutionText;
    public InputField sceneNameField;

	void Start () 
    {
        scene = new Scene();
	}
	
	void Update () 
    {
		
	}

    public void Generate()
    {
        StartCoroutine(GenerateLevelWithComplexity((int)complexitySlider.value));
    }

    IEnumerator GenerateLevelWithComplexity(int complexity)
    {
        scene.Clear();
        scene = new Scene();
        int movesRequired = scene.Generate(complexity);
        solutionText.text = "" + movesRequired;
        solutionText.text += "\n" + scene.solution.Count;
        foreach(bool move in scene.solution)
        {
            if(move)
            {
                solutionText.text = solutionText.text + "\nRight";
            }
            else
            {
                solutionText.text = solutionText.text + "\nLeft";
            }
        }

        string solutionString = "";

        List<CubeType[,]> solution = scene.GetSolution();

        foreach(CubeType[,] move in solution)
        {
            for(int j = 0; j < scene.levelHeight; j++)
            {
                for(int i = scene.levelWidth - 1; i >= 0; i--)
                {
                    solutionString = solutionString + (int)move[i, j] + " ";
                }
                solutionString = solutionString + "\n";
            }
            solutionString = solutionString + "\n**************\n\n";
        }
        File.WriteAllText("Assets/Resources/Levels/solution.txt", solutionString);

        yield return null;
    }

    public void Save()
    {
        string folderPath = "Assets/Resources/Levels/" + sceneNameField.text + "/";
        if(Directory.Exists(folderPath) == false)
        {
            Directory.CreateDirectory(folderPath);
        }

        string xmlPath = folderPath + "level.xml";
        string ssPath = folderPath + "ss.png";
        EditingManager.SaveLevel(scene, xmlPath, ssPath);
    }
}
