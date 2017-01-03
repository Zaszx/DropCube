using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public enum Direction
{
    Down,
    Right,
    Up,
    Left,
}

public enum SceneStatus
{
    Loading,
    Idle,
    Rotating,
    Moving,
}

public class Scene
{
    public int levelWidth;
    public int levelHeight;
    public Direction gravityDirection;
    public Cube[,] staticCubes;
    public Cube[,] cubes;
    public List<Cube> dynamicCubes;
    public Vector3 cameraPosition;
    public Bounds sceneBounds;
    public GameObject levelRootObject;
    public string scenePath;
    public SceneStatus sceneStatus;
    public GameManager gameManager;

    public Vector2[] possibleGravityDirections = new Vector2[4];

    public Scene()
    {
        sceneStatus = SceneStatus.Idle;

        possibleGravityDirections[(int)Direction.Down] = Vector2.up;
        possibleGravityDirections[(int)Direction.Right] = Vector2.left;
        possibleGravityDirections[(int)Direction.Up] = Vector2.down;
        possibleGravityDirections[(int)Direction.Left] = Vector2.right;
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

    public void Tick()
    {

    }

    public IEnumerator RotateCoroutine(bool clockwise)
    {
        sceneStatus = SceneStatus.Rotating;

        float totalTime = 1.0f;
        float accumulatedTime = 0.0f;

        Quaternion initialRotation = levelRootObject.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(levelRootObject.transform.rotation.eulerAngles + Vector3.up * (clockwise ? 90 : -90));

        while(accumulatedTime < totalTime)
        {
            levelRootObject.transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, accumulatedTime / totalTime);
            yield return new WaitForEndOfFrame();
            accumulatedTime = accumulatedTime + Time.deltaTime;
        }

        levelRootObject.transform.rotation = targetRotation;

        gravityDirection = gravityDirection + (clockwise ? 1 : -1);
        gravityDirection = (Direction)((int)gravityDirection % 4);
        if(gravityDirection < 0)
        {
            gravityDirection = gravityDirection + 4;
        }

        yield return new WaitForEndOfFrame();

        sceneStatus = SceneStatus.Moving;

        Vector2 gravityInVec2 = possibleGravityDirections[(int)gravityDirection];
        Debug.Log(gravityInVec2);
        Vector2 iterationAmount = gravityInVec2;
        if (iterationAmount.x == 0) iterationAmount.x = -1;
        if (iterationAmount.y == 0) iterationAmount.y = -1;
        Vector2 iterationBeginPoint = GetIterationBeginPoint(gravityInVec2);
        int iterationCount = 0;
        while(true)
        {
            List<Coroutine> allCoroutinesToWait = new List<Coroutine>();
            for(int i = (int)iterationBeginPoint.x; (i < levelWidth && i >= 0); i = i - (int)iterationAmount.x)
            {
                for (int j = (int)iterationBeginPoint.y; (j < levelHeight && j >= 0); j = j - (int)iterationAmount.y)
                {
                    Cube currentCube = cubes[i, j];
                    if(currentCube.IsStatic() == false)
                    {
                        Cube neighbourToMove = GetCubeWithIndex((int)(i + gravityInVec2.x), (int)(j + gravityInVec2.y));
                        if(neighbourToMove == null || (
                            (neighbourToMove.IsStatic() == true || neighbourToMove.isMarkedToMove == true) && neighbourToMove.GetCubeType() != CubeType.Wall))
                        {
                            Coroutine newCoroutine = gameManager.StartCoroutine(currentCube.MoveCoroutine(0.3f, GetStaticCubeWithIndex((int)(i + gravityInVec2.x), (int)(j + gravityInVec2.y))));
                            allCoroutinesToWait.Add(newCoroutine);
                        }
                    }
                }
            }

            for (int i = (int)iterationBeginPoint.x; (i < levelWidth && i >= 0); i = i - (int)iterationAmount.x)
            {
                for (int j = (int)iterationBeginPoint.y; (j < levelHeight && j >= 0); j = j - (int)iterationAmount.y)
                {
                    int ii = i;
                    int jj = j;
                    Cube currentCube = cubes[i, j];
                    if (currentCube.isMarkedToMove)
                    {
                        Debug.Log(currentCube.name);
                        cubes[i, j] = staticCubes[i, j];
                        Cube neighbourToMove = GetCubeWithIndex((int)(i + gravityInVec2.x), (int)(j + gravityInVec2.y));
                        if (neighbourToMove != null)
                        {
                            cubes[(int)(i + gravityInVec2.x), (int)(j + gravityInVec2.y)] = currentCube;
                            Debug.Log(new Vector2((int)(i + gravityInVec2.x), (int)(j + gravityInVec2.y)) + " REPLACED WITH " + new Vector2(i, j));
                        }
                    }
                }
            }

            if (allCoroutinesToWait.Count == 0)
            {
                break;
            }
            else
            {
                foreach(Coroutine c in allCoroutinesToWait)
                {
                    yield return c;
                }
            }

            iterationCount++;
        }

        Debug.Log("ITERATION COUNT: " + iterationCount);

        sceneStatus = SceneStatus.Idle;
    }

    Vector2 GetIterationBeginPoint(Vector2 gravity)
    {
        Vector2 result = Vector2.zero;
        if(gravity.x == 1)
        {
            result.x = levelWidth - 1;
        }
        if(gravity.y == 1)
        {
            result.y = levelHeight - 1;
        }
        return result;
    }

    public Cube GetCubeWithIndex(int x, int y)
    {
        if (x >= 0 && x < levelWidth && y >= 0 && y < levelHeight)
        {
            return cubes[x, y];
        }
        return null;
    }

    public Cube GetStaticCubeWithIndex(int x, int y)
    {
        if (x >= 0 && x < levelWidth && y >= 0 && y < levelHeight)
        {
            return staticCubes[x, y];
        }
        return null;
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

        levelRootObject.transform.position = new Vector3(levelWidth * 0.5f - 0.5f, 0, levelHeight * 0.5f - 0.5f);

        cubes = new Cube[levelWidth, levelHeight];
        staticCubes = new Cube[levelWidth, levelHeight];
        dynamicCubes = new List<Cube>();

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
            else
            {
                if(newCube.IsStatic() == false)
                {
                    newCube.transform.position += Vector3.up * 0.01f;
                    dynamicCubes.Add(newCube);

                    Cube staticCube = GameObject.Instantiate(Prefabs.grayCube).GetComponent<Cube>();
                    staticCube.transform.position = new Vector3(x, 0, y);
                    staticCube.transform.parent = levelRootObject.transform;
                    staticCubes[x, y] = staticCube;
                }
                else
                {
                    staticCubes[x, y] = newCube;
                }
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
