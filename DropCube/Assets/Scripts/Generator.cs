using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Generator : MonoBehaviour 
{
    public Scene scene;
    public Slider complexitySlider;
    public Text solutionText;

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
                solutionText.text = solutionText.text + "\nLeft";
            }
            else
            {
                solutionText.text = solutionText.text + "\nRight";
            }
        }

        string solutionString = "";

        List<CubeType[,]> solution = scene.GetSolution();

        foreach(CubeType[,] move in solution)
        {
            for(int i = 0; i < scene.levelWidth; i++)
            {
                for(int j = 0; j < scene.levelHeight; j++)
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
        EditingManager.SaveLevel(scene, "Assets/Resources/Levels/level.xml", "Assets/Resources/Levels/ss.png");
    }
}
