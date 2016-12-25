using UnityEngine;
using System.Collections;
using System.Xml;

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
    public string scenePath;


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

        levelRootObject = new GameObject("LevelRoot");

        cubes = new Cube[levelWidth, levelHeight];

        for(int i = 0; i < levelWidth; i++)
        {
            for(int j = 0; j < levelHeight; j++)
            {
                Cube newCube = GameObject.Instantiate(Prefabs.editCube).GetComponent<Cube>();
                newCube.transform.position = new Vector3(i, 0, j);
                newCube.transform.rotation = Quaternion.identity;

                cubes[i, j] = newCube;

                newCube.transform.parent = levelRootObject.transform;
                newCube.UpdateShaderProperties();
            }
        }

        for (int i = 0; i < levelWidth; i++ )
        {
            EditCube editCube = cubes[i, 0] as EditCube;
            editCube.cubeType = CubeType.Wall;

            editCube = cubes[levelWidth - 1, i] as EditCube;
            editCube.cubeType = CubeType.Wall;
        }

        for (int i = 0; i < levelHeight; i++ )
        {
            EditCube editCube = cubes[0, i] as EditCube;
            editCube.cubeType = CubeType.Wall;

            editCube = cubes[i, levelHeight - 1] as EditCube;
            editCube.cubeType = CubeType.Wall;
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
        scenePath = path;

        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(path);

        string[] filePathSplitted = path.Split('/');
        levelRootObject = new GameObject(filePathSplitted[filePathSplitted.Length - 1]);

        XmlNode levelRootNode = xmlDocument.FirstChild;
        int levelWidth = int.Parse(levelRootNode.Attributes["levelWidth"].Value);
        this.levelWidth = levelWidth;
        int levelHeight = int.Parse(levelRootNode.Attributes["levelHeight"].Value);
        this.levelHeight = levelHeight;

        cubes = new Cube[levelWidth, levelHeight];
        int cubeIndex = 0;

        XmlNode cubesNode = levelRootNode.FirstChild;
        XmlNode cubeNode = cubesNode.FirstChild;
        while (cubeNode != null)
        {
            string cubeTypeString = cubeNode.Attributes["type"].Value;
            CubeType cubeType = (CubeType)System.Enum.Parse(typeof(CubeType), cubeTypeString);

            GameObject cubePrefab = Prefabs.GetCubePrefabWithType(cubeType);
            if(isEditMode)
            {
                cubePrefab = Prefabs.editCube;
            }
            int x = cubeIndex / levelHeight;
            int y = cubeIndex % levelHeight;

            Cube newCube = GameObject.Instantiate(cubePrefab).GetComponent<Cube>();

            newCube.transform.position = new Vector3(x, 0, y);
            if(isEditMode)
            {
                (newCube as EditCube).cubeType = cubeType;
            }

            newCube.transform.parent = levelRootObject.transform;

            cubes[x, y] = newCube;
            cubeIndex++;

            cubeNode = cubeNode.NextSibling;
        }

        sceneBounds = new Bounds();
        sceneBounds.Encapsulate(new Vector3(-0.5f, 0, -0.5f));
        sceneBounds.Encapsulate(new Vector3(levelWidth - 0.5f, 0, levelWidth - 0.5f));

        ComputeCameraPosition();
    }

    public void WriteLevel(string path)
    {
        XmlDocument xmlDocument = new XmlDocument();

        XmlNode levelRootNode = xmlDocument.CreateElement("level");
        xmlDocument.AppendChild(levelRootNode);
        XmlAttribute levelWidthAttribute = xmlDocument.CreateAttribute("levelWidth");
        levelWidthAttribute.Value = levelWidth.ToString();
        levelRootNode.Attributes.Append(levelWidthAttribute);
        XmlAttribute levelHeightAttribute = xmlDocument.CreateAttribute("levelHeight");
        levelHeightAttribute.Value = levelHeight.ToString();
        levelRootNode.Attributes.Append(levelHeightAttribute);

        XmlNode cubesNode = xmlDocument.CreateElement("cubes");
        levelRootNode.AppendChild(cubesNode);

        for (int i = 0; i < levelWidth; i++)
        {
            for (int j = 0; j < levelHeight; j++)
            {
                XmlNode cubeNode = xmlDocument.CreateElement("cube");
                XmlAttribute typeAttribute = xmlDocument.CreateAttribute("type");
                typeAttribute.Value = (cubes[i, j] as EditCube).cubeType.ToString();
                cubeNode.Attributes.Append(typeAttribute);

                cubesNode.AppendChild(cubeNode);
            }
        }

        xmlDocument.Save(path);
    }

}
