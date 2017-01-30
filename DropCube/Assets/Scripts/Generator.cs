using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Generator : MonoBehaviour 
{
    public Scene scene;
    public Slider complexitySlider;

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
        scene.Generate(complexity);

        yield return null;
    }
}
