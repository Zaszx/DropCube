using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum GameState
{
    Menu,
    Game,
    LevelCleared,

}

public class GameManager : MonoBehaviour 
{
    Scene scene;
    SwipeData swipeData = new SwipeData();

    List<Level> levels = new List<Level>();
    MenuManager menuManager = new MenuManager();
    LevelClearedScreenManager levelClearedScreenManager = new LevelClearedScreenManager();

    int openLevelIndex;
    int maxOpenLevel;

    public GameState gameState;

    public Button undoButton;
    public Button menuButton;

    public SaveData saveData = new SaveData();

    void Awake()
    {
        levels.Add(new Level("Levels/level0"));
        levels.Add(new Level("Levels/level1"));
        levels.Add(new Level("Levels/level2"));
        levels.Add(new Level("Levels/level3"));
        levels.Add(new Level("Levels/level4"));
        levels.Add(new Level("Levels/level5"));
        levels.Add(new Level("Levels/level6"));
        levels.Add(new Level("Levels/level7"));
        levels.Add(new Level("Levels/level8"));
        levels.Add(new Level("Levels/level9"));
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

        saveData.Init();
        maxOpenLevel = saveData.level;

        menuManager.InitMenu(this, levels, maxOpenLevel);
        levelClearedScreenManager.Init(this);
        OpenMenu();

        openLevelIndex = 0;

        //scene.ReadLevel("Assets/Resources/Levels/testLevel.xml", false);
	}

    public void OpenMenu()
    {
        menuManager.SetVisible(true);
        gameState = GameState.Menu;
    }

    public void NextLevel()
    {
        openLevelIndex++;
        if(openLevelIndex > maxOpenLevel)
        {
            maxOpenLevel = openLevelIndex;
            menuManager.UnlockLevel(openLevelIndex);
            saveData.level = maxOpenLevel;
            saveData.Write();
        }
        scene.Clear();
        scene = new Scene();
        scene.gameManager = this;
        scene.ReadLevel(levels[openLevelIndex], "", false);
        gameState = GameState.Game;
    }

    public void OnLevelButtonClicked(Level level)
    {
        scene = new Scene();
        scene.gameManager = this;
        scene.ReadLevel(level, "", false);
        gameState = GameState.Game;
        menuManager.SetVisible(false);

        openLevelIndex = levels.IndexOf(level);
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

    public void OnLevelFinished(int starCount)
    {
        gameState = GameState.LevelCleared;
        levelClearedScreenManager.OnLevelCleared(starCount);
    }

    void Update () 
    {
        swipeData.Tick();

        if(gameState == GameState.Game)
        {
            undoButton.gameObject.SetActive(true);
            menuButton.gameObject.SetActive(true);

            scene.Tick();

            if (swipeData.swipeStatus == SwipeStatus.Finished)
            {
                if (scene.sceneStatus == SceneStatus.Idle)
                {
                    Vector2 resultSwipe = swipeData.resultSwipe;
                    Vector2 swipeDirection = GetDigitalDirectionFromAnalog(resultSwipe);
                    bool isRotating = (swipeDirection == Vector2.left || swipeDirection == Vector2.right);
                    if (isRotating)
                    {
                        StartCoroutine(scene.RotateCoroutine(swipeDirection == Vector2.right));
                    }
                }

                swipeData.Reset();
            }

            if(Input.GetKeyDown(KeyCode.Z))
            {
                if((scene.sceneStatus == SceneStatus.Idle || scene.sceneStatus == SceneStatus.Errored) && scene.undoManager.doneOperations.Count > 0)
                {
                    StartCoroutine(scene.UndoCoroutine());
                }
            }

            if(scene != null)
            {
                bool undoAllowed = (scene.sceneStatus == SceneStatus.Idle || scene.sceneStatus == SceneStatus.Errored) && scene.undoManager.doneOperations.Count > 0;
                undoButton.interactable = undoAllowed;
            }

        }
        else
        {
            undoButton.gameObject.SetActive(false);
            menuButton.gameObject.SetActive(false);
        }

	}

    public void UndoButtonClicked()
    {
        StartCoroutine(scene.UndoCoroutine());
    }

    public void MenuButtonClicked()
    {
        scene.Clear();
        menuManager.SetVisible(true);
    }

}
