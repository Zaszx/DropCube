using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager
{
    public GameObject menuParent;
    public GameManager gameManager;
    public Dictionary<Button, Level> buttonToLevelMap = new Dictionary<Button, Level>();
    public Dictionary<Button, int> buttonToLevelIndexMap = new Dictionary<Button, int>();
    public Dictionary<int, Button> levelIndexToButtonMap = new Dictionary<int, Button>();
    public GameObject buttonsParent;
    public int maxOpenLevel;

    public void InitMenu(GameManager gameManager, List<Level> levels, int maxOpenLevel)
    {
        this.gameManager = gameManager;

        menuParent = GameObject.Find("MenuParent");
        buttonsParent = GameObject.Find("ButtonsParent");

        float minLength = Mathf.Min(Screen.width, Screen.height) * 0.9f;
        float buttonSize = minLength * 0.2f;
        float iterationSize = buttonSize * 1.05f;
        buttonSize = 2000;

        for(int i = 0; i < levels.Count; i++)
        {
            Vector3 position = new Vector3(((i % 5) - 2) * iterationSize, -((i / 5) - 2) * iterationSize, 0);
            Button newButton = GameObject.Instantiate(Prefabs.levelButton).GetComponent<Button>();
            newButton.transform.SetParent(buttonsParent.transform);
            RectTransform newButtonTransform = newButton.GetComponent<RectTransform>();
            newButtonTransform.localPosition = Vector3.zero;
            newButtonTransform.position = position;
            newButtonTransform.localScale = Vector3.one;
            newButtonTransform.sizeDelta = new Vector2(buttonSize, buttonSize);

            Text levelText = newButton.GetComponentInChildren<Text>();
            levelText.text = "" + (i + 1);

            newButton.onClick.AddListener(delegate { OnButtonPressed(newButton); });

            newButton.GetComponent<Image>().sprite = levels[i].image;

            buttonToLevelMap.Add(newButton, levels[i]);
            buttonToLevelIndexMap.Add(newButton, i);

            if (i > maxOpenLevel)
            {
                Color color = newButton.GetComponent<Image>().color;
                color.a = 0.3f;
                newButton.GetComponent<Image>().color = color;
            }

            levelIndexToButtonMap.Add(i, newButton);
        }

        this.maxOpenLevel = maxOpenLevel;

        //buttonsParent.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
    }

    public void Tick()
    {
//         foreach (KeyValuePair<Button, Level> pair in buttonToLevelMap)
//         {
//             pair.Key.GetComponent<RectTransform>().position = Vector3.one;
// 
//             pair.Key.GetComponent<RectTransform>().localPosition = Vector3.zero;
//         }
    }

    public void UnlockLevel(int index)
    {
        if(index > maxOpenLevel)
        {
            maxOpenLevel = index;
            Button button = levelIndexToButtonMap[index];
            button.GetComponent<Image>().color = Color.white;
        }
    }

    void OnButtonPressed(Button button)
    {
        Debug.Log("Button pressed!");
        if(button.GetComponent<Image>().color.a >= 0.98f)
        {
            gameManager.OnLevelButtonClicked(buttonToLevelMap[button]);
        }

    }

    public void SetVisible(bool value)
    {
        menuParent.SetActive(value);
        foreach(KeyValuePair<Button, int> pair in buttonToLevelIndexMap)
        {
            pair.Key.GetComponent<RectTransform>().position = Vector3.one;

            pair.Key.GetComponent<RectTransform>().localPosition = Vector3.zero;

//             if (pair.Value > gameManager.saveData.level)
//             {
//                 Color color = pair.Key.GetComponent<Image>().color;
//                 color.a = 0.3f;
//                 pair.Key.GetComponent<Image>().color = color;
//             }
//             else
//             {
//                 Color color = pair.Key.GetComponent<Image>().color;
//                 color.a = 1.0f;
//                 pair.Key.GetComponent<Image>().color = color;
//             }
        }
    }
}
