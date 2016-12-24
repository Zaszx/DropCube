using UnityEngine;
using System.Collections;

public enum Direction
{
    Up,
    Left,
    Down,
    Right,
}

public class Scene
{
    public int levelWidth;
    public int levelHeight;
    public Direction gravityDirection;
    public Cube[,] cubes;
    public Vector3 cameraPosition;
    public Bounds sceneBounds;
    public GameObject levelRootObject;


    public Scene()
    {

    }

    public void Clear()
    {
        for (int i = 0; i < levelWidth; i++)
        {
            for (int j = 0; j < levelHeight; j++)
            {
                GameObject.Destroy(cubes[i, j].gameObject);
            }
        }
        GameObject.Destroy(levelRootObject);
    }

    public void CreateNewLevel(int levelWidth, int levelHeight)
    {
        this.levelWidth = levelWidth;
        this.levelHeight = levelHeight;

        levelRootObject = new GameObject();

        cubes = new Cube[levelWidth, levelHeight];

        for(int i = 0; i < levelWidth; i++)
        {
            for(int j = 0; j < levelHeight; j++)
            {
                Cube newCube = GameObject.Instantiate(Prefabs.grayCube).GetComponent<Cube>();
                newCube.transform.position = new Vector3(i, 0, j);
                newCube.transform.rotation = Quaternion.identity;

                cubes[i, j] = newCube;

                newCube.transform.parent = levelRootObject.transform;
                newCube.UpdateShaderProperties();
            }
        }
        sceneBounds = new Bounds();
        sceneBounds.Encapsulate(new Vector3(-0.5f, 0, -0.5f));
        sceneBounds.Encapsulate(new Vector3(levelWidth - 0.5f, 0, levelWidth - 0.5f));

        ComputeCameraPosition();
    }

    public void ComputeCameraPosition()
    {
        Vector3 cameraSize = (sceneBounds.extents);

        float requiredVerticalExtent = Mathf.Abs(cameraSize.x) * (float)Screen.height / (float)Screen.width;
        float requiredHorizontalExtent = Mathf.Abs(cameraSize.z);// * (float)Screen.width / (float)Screen.height;

        Camera.main.transform.position = sceneBounds.center + new Vector3(0, 1, 0);
        Camera.main.transform.LookAt(sceneBounds.center);

        Camera.main.orthographicSize = Mathf.Max(requiredVerticalExtent, requiredHorizontalExtent) * 1.1f;
        Camera.main.transform.rotation = Quaternion.Euler(90, 180, 0);

        cameraPosition = Camera.main.transform.position;
    }

    public void ReadLevel(string path, bool isEditMode)
    {

    }

    public void WriteLevel(string path)
    {

    }

}
