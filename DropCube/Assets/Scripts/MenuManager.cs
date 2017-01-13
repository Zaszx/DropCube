using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager
{
    public Canvas canvas;
    public GameManager gameManager;
    public Dictionary<Button, Level> buttonToLevelMap = new Dictionary<Button, Level>();
    public GameObject buttonsParent;

    public void InitMenu(GameManager gameManager, List<Level> levels)
    {
        this.gameManager = gameManager;

        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        buttonsParent = new GameObject("ButtonsParent");
        buttonsParent.transform.parent = canvas.transform;
        buttonsParent.transform.position = Vector3.zero;

        float minLength = Mathf.Min(Screen.width, Screen.height) * 0.8f;
        float buttonSize = minLength * 0.23f;

        Debug.Log("Screen width: " + Screen.width + " Height: " + Screen.height + " MinLen: " + minLength + " ButtonSize: " + buttonSize);

        for(int i = 0; i < levels.Count; i++)
        {
            Vector3 position = new Vector3(((i % 5) - 2) * buttonSize, -((i / 5) - 2) * buttonSize, 0);
            Button newButton = GameObject.Instantiate(Prefabs.levelButton).GetComponent<Button>();
            newButton.transform.parent = buttonsParent.transform;
            RectTransform newButtonTransform = newButton.GetComponent<RectTransform>();
            newButtonTransform.position = position;
            newButtonTransform.sizeDelta = new Vector2(buttonSize, buttonSize);

            Text levelText = newButton.GetComponentInChildren<Text>();
            levelText.text = "" + (i + 1);

            newButton.onClick.AddListener(delegate { OnButtonPressed(newButton); });

            buttonToLevelMap.Add(newButton, levels[i]);
        }

        buttonsParent.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
    }

    void OnButtonPressed(Button button)
    {
        Debug.Log("Button pressed!");
        gameManager.OnLevelButtonClicked(buttonToLevelMap[button]);
    }
}
