using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class EditingManager : MonoBehaviour
{
    public Scene scene;

    public List<EditCube> cubes;
    public List<EditCube> selectedCubes;
    public EditCube highlightedCube;
    public Text scenePathText;

    public Slider widthSlider;
    public Slider heightSlider;

    string lastOpenedScenePath;

    void Awake()
    {

    }

    void Start()
    {
        scene = new Scene();
        lastOpenedScenePath = "Assets/Resources/Levels/newLevel.xml";
    }

    void Update()
    {
        EditCube cubeUnderMouse = GetCubeUnderMouse();
        if (cubeUnderMouse != null)
        {
            if (highlightedCube != null)
            {
                highlightedCube.highlighted = false;
            }
            cubeUnderMouse.highlighted = true;
            highlightedCube = cubeUnderMouse;
        }
        else
        {
            if (highlightedCube)
            {
                highlightedCube.highlighted = false;
                highlightedCube = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && highlightedCube != null)
        {
            highlightedCube.highlighted = false;
            highlightedCube.selected = true;
            if (Input.GetKey(KeyCode.LeftControl))
            {
                selectedCubes.Add(highlightedCube);
            }
            else
            {
                foreach (EditCube cube in selectedCubes)
                {
                    cube.selected = false;
                }
                selectedCubes.Clear();
                selectedCubes.Add(highlightedCube);
            }
            highlightedCube = null;
        }

        scenePathText.text = lastOpenedScenePath;
    }

    EditCube GetCubeUnderMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayResult = new RaycastHit();
        bool result = Physics.Raycast(ray, out rayResult);
        if (result)
        {
            EditCube collidedCube = rayResult.collider.gameObject.GetComponent<EditCube>();
            if (collidedCube != null)
            {
                return collidedCube;
            }
        }
        return null;
    }

    public void CubeTypeComboboxChanged(Int32 value)
    {
        CubeType newCubeType = (CubeType)value;
        foreach (EditCube cube in selectedCubes)
        {
            cube.cubeType = newCubeType;
        }
    }

    public void NewButtonClicked()
    {
        scene.Clear();
        scene.CreateNewLevel((int)widthSlider.value, (int)heightSlider.value);

        lastOpenedScenePath = "Assets/Resources/Levels/newLevel.xml";
        UpdateEditCubes();
    }

    public void UpdateEditCubes()
    {
        cubes.Clear();
        selectedCubes.Clear();
        highlightedCube = null;

        for(int i = 0; i < scene.levelWidth; i++)
        {
            for(int j = 0; j < scene.levelHeight; j++)
            {
                cubes.Add(scene.cubes[i, j] as EditCube);
            }
        }
    }

    public void OpenButtonClicked()
    {
        string filePath = EditorUtility.OpenFilePanel("Choose level", "Assets/Resources/Levels", "xml");
        scene.Clear();
        scene.ReadLevel(null, filePath, true);
        UpdateEditCubes();
    }

    public void SaveButtonClicked()
    {
        scene.WriteLevel(lastOpenedScenePath);
    }

}
