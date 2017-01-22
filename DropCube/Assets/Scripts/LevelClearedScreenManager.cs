using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelClearedScreenManager
{
    public GameObject levelClearedParent;
    public Button nextLevelButton;
    public Button menuButton;
    public Image[] starImages = new Image[3];

    public GameManager gameManager;

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;

        levelClearedParent = GameObject.Find("LevelClearedParent");
        nextLevelButton = GameObject.Find("NextLevelButton").GetComponent<Button>();
        menuButton = GameObject.Find("MenuButton").GetComponent<Button>();

        starImages[0] = GameObject.Find("Star0").GetComponent<Image>();
        starImages[1] = GameObject.Find("Star1").GetComponent<Image>();
        starImages[2] = GameObject.Find("Star2").GetComponent<Image>();

        menuButton.onClick.AddListener(MenuButtonClicked);
        nextLevelButton.onClick.AddListener(NextLevelButtonClicked);

        levelClearedParent.SetActive(false);
    }

    public void OnLevelCleared(int starCount)
    {
        levelClearedParent.SetActive(true);

    }

    public void MenuButtonClicked()
    {
        levelClearedParent.SetActive(false);
        gameManager.OpenMenu();
    }

    public void NextLevelButtonClicked()
    {
        levelClearedParent.SetActive(false);
        gameManager.NextLevel();
    }


}
