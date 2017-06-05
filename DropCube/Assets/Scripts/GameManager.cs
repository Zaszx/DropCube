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

    int openLevelIndex;
    int maxOpenLevel;

    public GameState gameState;

    public Button undoButton;
    public Button menuButton;
    public Button nextLevelButton;

    public Image backGround;

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
        levels.Add(new Level("Levels/level10"));
        levels.Add(new Level("Levels/level11"));
        levels.Add(new Level("Levels/level12"));
        levels.Add(new Level("Levels/level13"));
        levels.Add(new Level("Levels/level14"));
        levels.Add(new Level("Levels/level15"));
        levels.Add(new Level("Levels/level16"));
        levels.Add(new Level("Levels/level17"));
        levels.Add(new Level("Levels/level18"));
        levels.Add(new Level("Levels/level19"));
        levels.Add(new Level("Levels/level20"));
        levels.Add(new Level("Levels/level21"));
        levels.Add(new Level("Levels/level22"));
        levels.Add(new Level("Levels/level23"));
        levels.Add(new Level("Levels/level24"));
        levels.Add(new Level("Levels/level25"));
        levels.Add(new Level("Levels/level26"));
        levels.Add(new Level("Levels/level27"));
        levels.Add(new Level("Levels/level28"));
        levels.Add(new Level("Levels/level29"));
        levels.Add(new Level("Levels/level30"));
        levels.Add(new Level("Levels/level31"));
        levels.Add(new Level("Levels/level32"));
        levels.Add(new Level("Levels/level33"));
        levels.Add(new Level("Levels/level34"));
        levels.Add(new Level("Levels/level35"));
        levels.Add(new Level("Levels/level36"));
        levels.Add(new Level("Levels/level37"));
        levels.Add(new Level("Levels/level38"));
        levels.Add(new Level("Levels/level39"));
        levels.Add(new Level("Levels/level40"));
        levels.Add(new Level("Levels/level41"));
        levels.Add(new Level("Levels/level42"));
        levels.Add(new Level("Levels/level43"));
        levels.Add(new Level("Levels/level44"));
        levels.Add(new Level("Levels/level45"));
        levels.Add(new Level("Levels/level46"));
        levels.Add(new Level("Levels/level47"));
        levels.Add(new Level("Levels/level48"));
        levels.Add(new Level("Levels/level49"));
    }

    void Start () 
    {
        scene = new Scene();
        scene.gameManager = this;

        saveData.Init();
        maxOpenLevel = saveData.level;

        menuManager.InitMenu(this, levels, maxOpenLevel);
        OpenMenu();

        openLevelIndex = 0;

        nextLevelButton.gameObject.SetActive(false);

        //scene.ReadLevel("Assets/Resources/Levels/testLevel.xml", false);
	}

    public void OpenMenu()
    {
        menuManager.SetVisible(true);
        backGround.sprite = Prefabs.playScreens[0];
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
        swipeData.Reset();
    }

    public void OnLevelButtonClicked(Level level)
    {
        if(scene != null)
        {
            scene.Clear();
        }
        scene = new Scene();
        scene.gameManager = this;
        scene.ReadLevel(level, "", false);
        gameState = GameState.Game;
        menuManager.SetVisible(false);

        openLevelIndex = levels.IndexOf(level);
        swipeData.Reset();
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
        if (openLevelIndex + 1 > maxOpenLevel)
        {
            maxOpenLevel = openLevelIndex + 1;
            menuManager.UnlockLevel(openLevelIndex + 1);
            saveData.level = maxOpenLevel;
            saveData.Write();
        }
        gameState = GameState.LevelCleared;
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
                    Vector2 averageSwipePosition = (swipeData.startPosition + swipeData.endPosition) * 0.5f;

                    bool clockwise = swipeData.IsSwipeClockwise(swipeDirection, scene.sceneCenterOnScreen, averageSwipePosition);

                    StartCoroutine(scene.RotateCoroutine(clockwise));
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
            nextLevelButton.gameObject.SetActive(false);
            menuButton.gameObject.SetActive(true);
        }
        else if(gameState == GameState.LevelCleared)
        {
            undoButton.gameObject.SetActive(false);
            nextLevelButton.gameObject.SetActive(true);
            menuButton.gameObject.SetActive(true);
        }
        else if(gameState == GameState.Menu)
        {
            undoButton.gameObject.SetActive(false);
            nextLevelButton.gameObject.SetActive(false);
            menuButton.gameObject.SetActive(false);
            menuManager.Tick();

//             Camera.main.transform.position = new Vector3(0, 10, 0);
//             Camera.main.transform.rotation = Quaternion.identity;
        }

	}

    public void UndoButtonClicked()
    {
        StartCoroutine(scene.UndoCoroutine());
    }

    public void RestartButtonClicked()
    {
        OnLevelButtonClicked(levels[openLevelIndex]);
    }

    public void NextLevelButtonClicked()
    {
        NextLevel();
        //OnLevelButtonClicked(levels[openLevelIndex + 1]);
    }

    public void MenuButtonClicked()
    {
        scene.Clear();
        swipeData.Reset();
        gameState = GameState.Menu;
        backGround.sprite = Prefabs.playScreens[0];
        StaticCoroutine.StopAllCoroutines();
        menuManager.SetVisible(true);
    }

}
