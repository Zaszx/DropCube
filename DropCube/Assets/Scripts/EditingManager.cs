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
    string screenshotPath;

    void Awake()
    {

    }

    void Start()
    {
        scene = new Scene();
        lastOpenedScenePath = "Assets/Resources/Levels/level.xml";
        screenshotPath = "Assets/Resources/Levels/ss.png";
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

    public void SolveClicked()
    {
        scene.GetMovesRequiredToSolve();
        int a = 5;
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

        lastOpenedScenePath = "Assets/Resources/Levels/level.xml";
        screenshotPath = "Assets/Resources/Levels/ss.png";
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
        string filePath = "";
#if UNITY_EDITOR
        filePath = EditorUtility.OpenFilePanel("Choose level", "Assets/Resources/Levels", "xml");
#endif
        scene.Clear();
        scene.ReadLevel(null, filePath, true);
        UpdateEditCubes();

        screenshotPath = filePath.Substring(0, filePath.LastIndexOf("/"));
        screenshotPath = screenshotPath + "/ss.png";
    }

    public static void SaveLevel(Scene scene, string scenePath, string screenshotPath)
    {
        scene.WriteLevel(scenePath);

        Bounds b = scene.sceneBounds;
        Camera cam = Camera.main;

        Vector3 centerScreenPoint = cam.WorldToScreenPoint(b.center + b.extents);
        Vector3 edgeScreenPoint = cam.WorldToScreenPoint(b.center - b.extents);

        Vector3 rectSize = edgeScreenPoint - centerScreenPoint;

        Rect ScrRect = new Rect(centerScreenPoint, rectSize);

        //Construct a rect
        //Rect ScrRect = Rect.MinMaxRect(b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.x + b.extents.x, b.center.y + b.extents.y);
        Debug.Log("Rect: " + ScrRect);
        Debug.Log("Width: " + Screen.width + " Height: " + Screen.height);
        Debug.Log("Bounds: " + b);
        //  Note: GUI.Box(ScrRect,"Captured Data") works great from this Rect    

        // Invert the Y for ScreenSpace not GUI space
        //ScrRect.y = Screen.height - ScrRect.y;

        // Grab texture from Rect

        // This is a factor that enlarges the snapshot's resolution. It sets the resolution of the rendertexture to a multiple of the Screen's resolution.
        // If you don't want that, just set it to 1.
        int superSamplingFactor = 1;

        Texture2D snapShot = new Texture2D((int)ScrRect.width * superSamplingFactor, (int)ScrRect.height * superSamplingFactor, TextureFormat.ARGB32, false);

        RenderTexture snapShotRT = new RenderTexture(Screen.width * superSamplingFactor, Screen.height * superSamplingFactor, 24, RenderTextureFormat.ARGB32); // We're gonna render the entire screen into this
        RenderTexture.active = snapShotRT;
        Camera.main.targetTexture = snapShotRT;
        Camera.main.Render();

        Rect lassoRectSS = new Rect(ScrRect.xMin * superSamplingFactor, ScrRect.yMin * superSamplingFactor, ScrRect.width * superSamplingFactor, ScrRect.height * superSamplingFactor);

        snapShot.ReadPixels(lassoRectSS, 0, 0);
        snapShot.Apply();
        RenderTexture.active = null;
        Camera.main.targetTexture = null;

        // This is another planar mesh in the view so I can see the captured image
        byte[] fileBytes = snapShot.EncodeToPNG();
        File.WriteAllBytes(screenshotPath, fileBytes);
    }

    public void SaveButtonClicked()
    {
        SaveLevel(scene, lastOpenedScenePath, screenshotPath);
    }
}
