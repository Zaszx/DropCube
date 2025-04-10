﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class InitialScreenManager : MonoBehaviour 
{
    public Scene tutorialScene;
    public bool[] solution;
    public int currentSolutionIndex = 0;
    public bool waiting;
    public Image whiteScreen;
    public Image background;
    public Button playButton;

    void Start () 
    {
        background.sprite = Prefabs.initialScreen;
        //StartCoroutine(ResetCoroutine());
    }

    private void Reset()
    {
        solution = new bool[] { false, false, false, true, true, true, false, false, false };

        if(tutorialScene != null)
        {
            tutorialScene.Clear();
        }

        tutorialScene = new Scene();
        Level tutorialLevel = new Level("Levels/tutorialLevel");
        tutorialScene.ReadLevel(tutorialLevel, "", false);
        currentSolutionIndex = 0;
    }
    
    public IEnumerator ResetCoroutine()
    {
        waiting = true;
        if(tutorialScene != null)
        {
            yield return new WaitForSeconds(1.0f);
        }
        Reset();
        yield return new WaitForSeconds(1.0f);
        waiting = false;
    }

	void Update () 
    {
        tutorialScene.levelRootObject.SetActive(false);
        tutorialScene.Tick();
		if(tutorialScene.sceneStatus == SceneStatus.Idle && waiting == false)
        {
            if(currentSolutionIndex == solution.Length)
            {
                StartCoroutine(ResetCoroutine());
            }
            else
            {
                StartCoroutine(tutorialScene.RotateCoroutine(solution[currentSolutionIndex]));
                currentSolutionIndex++;
            }
        }
	}

    public void PlayButtonClicked()
    {
        StartCoroutine(PlayButtonCoroutine());
    }

    public IEnumerator PlayButtonCoroutine()
    {
        playButton.interactable = false;
        whiteScreen.CrossFadeAlpha(0.0f, 0.8f, true);

        float currentTime = 0;
        float totalTime = 1.0f;

        while(currentTime < totalTime)
        {
            yield return new WaitForEndOfFrame();
            currentTime = currentTime + Time.deltaTime;
        }

        SceneManager.LoadScene("gameScene");
    }
}
