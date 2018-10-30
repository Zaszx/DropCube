using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public enum GameState
{
    Menu,
    Game,
    LevelCleared,

}

public enum TutorialState
{
    Playing,
    Finished,
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

    public float undoButtonSeconds;
    public bool undoButtonIsDown;

    public Text pressAndHoldText;

    public Image tutorialHand;
    public Transform tutorialHandInitialTransform;
    public Transform tutorialHandTargetTransform;

    public TutorialState tutorialState;

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

        undoButtonIsDown = false;
        undoButtonSeconds = 0.0f;

        pressAndHoldText.gameObject.SetActive(false);

        Advertisement.Initialize("1653772");
        //scene.ReadLevel("Assets/Resources/Levels/testLevel.xml", false);
    }

    public void OpenMenu()
    {
        menuManager.SetVisible(true);
        backGround.sprite = Prefabs.levelSelectScreen;
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

        bool isReady = Advertisement.IsReady();
        Advertisement.Show();
    }

    public void UndoButtonDown()
    {
        if(undoButton.IsInteractable())
        {
            undoButtonIsDown = true;
            pressAndHoldText.gameObject.SetActive(true);
        }
    }

    public void UndoButtonUp()
    {
        undoButtonIsDown = false;
        pressAndHoldText.gameObject.SetActive(false);
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
        if(openLevelIndex == 0)
        {
            StartCoroutine(TutorialHandCoroutine());
        }
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

    public IEnumerator TutorialHandCoroutine()
    {
        tutorialHand.gameObject.SetActive(true);

        while(true)
        {
            Vector3 handInitialPosition = tutorialHandInitialTransform.position;
            Vector3 handTargetPosition = tutorialHandTargetTransform.position;

            float currentTime = -0.5f;
            float totalTime = 1.0f;

            while(currentTime < totalTime + 0.5f)
            {
                if (gameState == GameState.LevelCleared || gameState == GameState.Menu)
                {
                    break;
                }

                tutorialHand.rectTransform.position = Vector3.Lerp(handInitialPosition, handTargetPosition, currentTime / totalTime);

                yield return new WaitForEndOfFrame();
                currentTime = currentTime + Time.deltaTime;
            }
            if(gameState == GameState.LevelCleared || gameState == GameState.Menu)
            {
                break;
            }
        }

        tutorialHand.gameObject.SetActive(false);
    }

    void Update () 
    {
        swipeData.Tick();

        if(gameState == GameState.Game)
        {
            if(undoButtonIsDown)
            {
                undoButtonSeconds += Time.deltaTime;
            }
            else
            {
                undoButtonSeconds = 0.0f;
            }
            if(undoButtonSeconds > 2.0f)
            {
                scene.UndoAllTriggered();
                undoButtonIsDown = false;
                undoButtonSeconds = 0.0f;
                pressAndHoldText.gameObject.SetActive(false);
            }

            undoButton.gameObject.SetActive(true);
            menuButton.gameObject.SetActive(true);

            scene.Tick();

            if (swipeData.swipeStatus == SwipeStatus.Finished)
            {
                if (scene.sceneStatus == SceneStatus.Idle && swipeData.isSingleClick == false)
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
            nextLevelButton.gameObject.SetActive(openLevelIndex != levels.Count);
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
        OpenMenu();
        StaticCoroutine.StopAllCoroutines();
    }

}
