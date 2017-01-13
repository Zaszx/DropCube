using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
    Scene scene;
    SwipeData swipeData = new SwipeData();

    List<Level> levels = new List<Level>();
    MenuManager menuManager = new MenuManager();

    void Awake()
    {
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
        levels.Add(new Level("Levels/testLevel"));
    }

    void Start () 
    {
        scene = new Scene();
        scene.gameManager = this;

        menuManager.InitMenu(this, levels);

        //scene.ReadLevel("Assets/Resources/Levels/testLevel.xml", false);
	}

    public void OnLevelButtonClicked(Level level)
    {
        
    }

    public Vector2 GetDigitalDirectionFromAnalog(Vector2 analogDirection)
    {
        Vector2[] possibleDirections = new Vector2[4];
        possibleDirections[0] = Vector2.left;
        possibleDirections[1] = Vector2.up;
        possibleDirections[2] = Vector2.down;
        possibleDirections[3] = Vector2.right;

        float biggestDotp = 0.0f;
        Vector2 result = Vector2.zero;
        for (int i = 0; i < 4; i++)
        {
            float currentDotp = Vector2.Dot(analogDirection, possibleDirections[i]);
            if (currentDotp > biggestDotp)
            {
                biggestDotp = currentDotp;
                result = possibleDirections[i];
            }
        }
        return result;
    }

    void Update () 
    {
        swipeData.Tick();
        scene.Tick();

        if(swipeData.swipeStatus == SwipeStatus.Finished)
        {
            if(scene.sceneStatus == SceneStatus.Idle)
            {
                Vector2 resultSwipe = swipeData.resultSwipe;
                Vector2 swipeDirection = GetDigitalDirectionFromAnalog(resultSwipe);
                bool isRotating = (swipeDirection == Vector2.left || swipeDirection == Vector2.right);
                if(isRotating)
                {
                    StartCoroutine(scene.RotateCoroutine(swipeDirection == Vector2.right));
                }
            }

            swipeData.Reset();
        }
	}

}
