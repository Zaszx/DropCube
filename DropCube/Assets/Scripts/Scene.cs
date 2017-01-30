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
    Undoing,
    Rotating,
    Moving,
    Errored,
}

public class Scene
{
    public int levelWidth;
    public int levelHeight;
    public Direction gravityDirection;
    public Cube[,] staticCubes;
    public Cube[,] cubes;
    public List<Cube> dynamicCubes = new List<Cube>();
    public Vector3 cameraPosition;
    public Bounds sceneBounds;
    public GameObject levelRootObject;
    public string scenePath;
    public SceneStatus sceneStatus;
    public GameManager gameManager;

    public UndoManager undoManager = new UndoManager();

    public GameObject ticksParent;

    public Vector2[] possibleGravityDirections = new Vector2[4];

    public Scene()
    {
        sceneStatus = SceneStatus.Idle;

        possibleGravityDirections[(int)Direction.Down] = Vector2.up;
        possibleGravityDirections[(int)Direction.Right] = Vector2.left;
        possibleGravityDirections[(int)Direction.Up] = Vector2.down;
        possibleGravityDirections[(int)Direction.Left] = Vector2.right;

        ticksParent = GameObject.Find("TicksParent");
    }

    public void Clear()
    {
        for (int i = 0; i < levelWidth; i++)
        {
            for (int j = 0; j < levelHeight; j++)
            {
                Cube currentCube = cubes[i, j];
                if(currentCube.IsStatic() == false)
                {
                    Cube staticCube = staticCubes[i, j];
                    if(staticCube != null)
                    {
                        GameObject.Destroy(staticCube.gameObject);
                    }
                }
                if(currentCube != null)
                {
                    GameObject.Destroy(cubes[i, j].gameObject);
                }
            }
        }
        foreach(Cube c in dynamicCubes)
        {
            if(c != null && c.gameObject != null)
            {
                GameObject.Destroy(c.gameObject);
            }
        }

        staticCubes = null;
        cubes = null;
        levelWidth = 0;
        levelHeight = 0;
        GameObject.Destroy(levelRootObject);
    }

    public void Generate(int complexity)
    {
        Clear();
        int gridSize = Random.Range(complexity / 4, complexity / 3);
        CreateNewLevel(gridSize, gridSize);

        // Create Holes
        int holeCount = Random.Range(2, gridSize / 2);
        for(int i = 0; i < holeCount; i++)
        {
            bool holeInX = Random.value < 0.5f;
            int randomValue = Random.Range(0, gridSize - 1);
            bool holeInTop = Random.value < 0.5f;

            int holeXIndex = 0;
            int holeYIndex = 0;

            if(holeInX)
            {
                holeXIndex = randomValue;
                holeYIndex = holeInTop ? 0 : gridSize - 1;
            }
            else
            {
                holeXIndex = holeInTop ? 0 : gridSize - 1;
                holeYIndex = randomValue;
            }

            Cube currentCube = cubes[holeXIndex, holeYIndex];
            if(currentCube.GetCubeType() == CubeType.Gray)
            {
                i--;
                continue;
            }

            Cube grayCube = GameObject.Instantiate(Prefabs.grayCube).GetComponent<Cube>();
            grayCube.transform.position = new Vector3(holeXIndex, 0, holeYIndex);
            ReplaceCubeWith(holeXIndex, holeYIndex, grayCube);
        }


        // Create cubes
        int cubeCount = Random.Range(complexity / 10, complexity / 3);
        cubeCount = Mathf.Min(cubeCount, (gridSize - 1) * (gridSize - 1) / 5);
        for(int i = 0; i < cubeCount; i++)
        {
            int randomX = Random.Range(1, gridSize - 2);
            int randomY = Random.Range(1, gridSize - 2);

            Cube currentCube = cubes[randomX, randomY];
            if(currentCube.GetCubeType() != CubeType.Gray)
            {
                i--;
                continue;
            }

            bool badCube = (i == 0) || Random.value < 0.5f;

            Cube cubeBelow = cubes[randomX, randomY + 1];
            if(randomX == 1 && cubeBelow.GetCubeType() == CubeType.Gray)
            {
                i--;
                continue;
            }

            if(cubeBelow.GetCubeType() == CubeType.Gray)
            {
                Cube wallCube = GameObject.Instantiate(Prefabs.wallCube).GetComponent<Cube>();
                wallCube.transform.position = new Vector3(randomX, 0, randomY + 1);
                ReplaceCubeWith(randomX, randomY + 1, wallCube);
            }

            Cube newCube = GameObject.Instantiate(badCube ? Prefabs.badCube : Prefabs.goodCube).GetComponent<Cube>();
            newCube.transform.position = new Vector3(randomX, 0, randomY);
            ReplaceCubeWith(randomX, randomY, newCube);
        }


        // Generate Walls
        int wallCount = Random.Range(complexity / 10, complexity / 3);
        wallCount = Mathf.Min(cubeCount, (gridSize - 1) * (gridSize - 1) / 5);
        for(int i = 0; i < wallCount; i++)
        {
            int randomX = Random.Range(1, gridSize - 2);
            int randomY = Random.Range(1, gridSize - 2);

            Cube currentCube = cubes[randomX, randomY];
            if (currentCube.GetCubeType() != CubeType.Gray)
            {
                i--;
                continue;
            }

            Cube wallCube = GameObject.Instantiate(Prefabs.wallCube).GetComponent<Cube>();
            wallCube.transform.position = new Vector3(randomX, 0, randomY);
            ReplaceCubeWith(randomX, randomY, wallCube); ;
        }
    }

    public int GetMovesRequiredToSolve()
    {
        int currentDepth = 0;
        bool solved = false;
        CubeType[,] levelToInt = new CubeType[levelWidth, levelHeight];
        for (int i = 0; i < levelWidth; i++ )
        {
            for(int j = 0; j < levelHeight; j++)
            {
                levelToInt[i, j] = cubes[i, j].GetCubeType();
            }
        }
        while (currentDepth < 20 && solved == false)
        {
            currentDepth++;
            solved = Dfs(0, currentDepth, levelToInt, Vector2.up);
        }
        if(solved)
        {
            return currentDepth;
        }
        return -1;
    }

    bool Dfs(int depth, int maxDepth, CubeType[,] cubes, Vector2 gravityDirection)
    {
        if(depth == maxDepth)
        {
            return false;
        }


        return true;
    }

    void ReplaceCubeWith(int x, int y, Cube newCube)
    {
        Cube currentCube = cubes[x, y];
        if(currentCube.IsStatic() == false)
        {
            GameObject.Destroy(staticCubes[x, y].gameObject);
        }
        GameObject.Destroy(currentCube.gameObject);

        cubes[x, y] = newCube;
        if(newCube.IsStatic())
        {
            staticCubes[x, y] = newCube;
        }
    }

    public void CreateNewLevel(int levelWidth, int levelHeight)
    {
        this.levelWidth = levelWidth;
        this.levelHeight = levelHeight;

        levelRootObject = new GameObject("LevelRoot");

        cubes = new Cube[levelWidth, levelHeight];
        staticCubes = new Cube[levelWidth, levelHeight];

        for(int i = 0; i < levelWidth; i++)
        {
            for(int j = 0; j < levelHeight; j++)
            {
                Cube newCube = GameObject.Instantiate(Prefabs.editCube).GetComponent<Cube>();
                newCube.transform.position = new Vector3(i, 0, j);
                newCube.transform.rotation = Quaternion.identity;
                newCube.scene = this;

                cubes[i, j] = newCube;
                staticCubes[i, j] = newCube;

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

        Camera.main.orthographicSize = Mathf.Max(requiredVerticalExtent, requiredHorizontalExtent) * 1.3f;
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

        Dictionary<Cube, Vector3> cubeOriginalPositions = new Dictionary<Cube, Vector3>();
        foreach(Cube c in dynamicCubes)
        {
            cubeOriginalPositions[c] = c.transform.position;
        }

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
                            Coroutine newCoroutine = gameManager.StartCoroutine(currentCube.MoveCoroutine(currentCube.GetCubeType() == CubeType.Good ? 0.27f : 0.3f, GetStaticCubeWithIndex((int)(i + gravityInVec2.x), (int)(j + gravityInVec2.y))));
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

        SceneUndoData newUndoData = new SceneUndoData();
        foreach(Cube c in dynamicCubes)
        {
            if(c.transform.position != cubeOriginalPositions[c])
            {
                CubeUndoData currentCubeData = new CubeUndoData();
                currentCubeData.cube = c;
                currentCubeData.fellDown = c.isActiveAndEnabled;
                currentCubeData.startPosition = cubeOriginalPositions[c];
                currentCubeData.endPosition = c.transform.position;

                newUndoData.cubeData.Add(currentCubeData);
            }
        }
        newUndoData.isRotationClockwise = clockwise;
        undoManager.AddData(newUndoData);

        if (sceneStatus != SceneStatus.Errored)
        {
            sceneStatus = SceneStatus.Idle;
        }
    }

    public IEnumerator UndoCoroutine()
    {
        sceneStatus = SceneStatus.Undoing;
        SceneUndoData undoData = undoManager.GetLastOperation();
        int maxFallAmount = 0;
        foreach(CubeUndoData cubeData in undoData.cubeData)
        {
            cubeData.fallAmount = Mathf.RoundToInt(Vector3.Distance(cubeData.startPosition, cubeData.endPosition));
            maxFallAmount = Mathf.Max(maxFallAmount, cubeData.fallAmount);
        }

        float waitAmount = 0.1f;
        List<Coroutine> allCoroutinesToWait = new List<Coroutine>();
        for(int i = maxFallAmount; i > 0; i--)
        {
            foreach (CubeUndoData cubeData in undoData.cubeData)
            {
                if(cubeData.fallAmount == i)
                {
                    cubeData.cube.gameObject.SetActive(true);
                    Coroutine c = gameManager.StartCoroutine(cubeData.cube.MoveTo(cubeData.startPosition, waitAmount * i));
                    allCoroutinesToWait.Add(c);
                }
            }
            yield return new WaitForSeconds(waitAmount);
        }

        foreach(Coroutine c in allCoroutinesToWait)
        {
            yield return c;
        }

        float totalTime = 0.3f;
        float accumulatedTime = 0.0f;

        Quaternion initialRotation = levelRootObject.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(levelRootObject.transform.rotation.eulerAngles + Vector3.up * (undoData.isRotationClockwise ? -90 : 90));

        while (accumulatedTime < totalTime)
        {
            levelRootObject.transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, accumulatedTime / totalTime);
            yield return new WaitForEndOfFrame();
            accumulatedTime = accumulatedTime + Time.deltaTime;
        }

        levelRootObject.transform.rotation = targetRotation;

        gravityDirection = gravityDirection + (undoData.isRotationClockwise ? -1 : 1);
        gravityDirection = (Direction)((int)gravityDirection % 4);
        if (gravityDirection < 0)
        {
            gravityDirection = gravityDirection + 4;
        }

        UpdateGridFromCubePositions();

        if(sceneStatus != SceneStatus.Errored)
        {
            sceneStatus = SceneStatus.Idle;
        }
    }

    public void UpdateGridFromCubePositions()
    {
        for (int i = 0; i < levelWidth; i++)
        {
            for (int j = 0; j < levelHeight; j++)
            {
                Cube staticCube = staticCubes[i, j];
                int cubeX = Mathf.RoundToInt(staticCube.transform.position.x);
                int cubeZ = Mathf.RoundToInt(staticCube.transform.position.z);
                cubes[cubeX, cubeZ] = staticCube;
            }
        }

        for (int i = 0; i < levelWidth; i++)
        {
            for (int j = 0; j < levelHeight; j++)
            {
                staticCubes[i, j] = cubes[i, j];
            }
        }

        foreach (Cube c in dynamicCubes)
        {
            int cubeX = Mathf.RoundToInt(c.transform.position.x);
            int cubeZ = Mathf.RoundToInt(c.transform.position.z);
            if(cubeX < levelWidth && cubeX >= 0 && cubeZ >= 0 && cubeZ < levelHeight)
            {
                cubes[cubeX, cubeZ] = c;
            }
        }

        gravityDirection = Direction.Down;
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

    public void RestartLevel()
    {

    }

    public void WinLevel()
    {
        gameManager.OnLevelFinished(0);
    }

    public void OnCubeFallsDown(Cube cube)
    {
        if(cube.GetCubeType() == CubeType.Good)
        {
            sceneStatus = SceneStatus.Errored;
        }
        else if(cube.GetCubeType() == CubeType.Bad)
        {
            Vector3 cubeLastPosition = cube.transform.position;
            cube.gameObject.SetActive(false);
            bool andBadCubeLeft = false;
            foreach(Cube c in dynamicCubes)
            {
                if(c.isActiveAndEnabled && c.GetCubeType() == CubeType.Bad)
                {
                    andBadCubeLeft = true;
                }
            }
            if(andBadCubeLeft == false && sceneStatus != SceneStatus.Errored)
            {
                gameManager.StopAllCoroutines();
                WinLevel();
            }
            else
            {
                gameManager.StartCoroutine(CreateTick(cubeLastPosition));
            }
        }
    }

    public IEnumerator CreateTick(Vector3 worldPosition)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        GameObject tickObject = GameObject.Instantiate(Prefabs.tickObject);
        tickObject.transform.SetParent(ticksParent.transform);
        tickObject.transform.position = screenPosition;
        yield return new WaitForSeconds(0.5f);
        GameObject.Destroy(tickObject);
    }

    public void ReadLevel(Level level, string path, bool isEditMode)
    {
        Clear();

        scenePath = path;

        XmlDocument xmlDocument = new XmlDocument();

        if(level == null)
        {
            xmlDocument.Load(path);
        }
        else
        {
            xmlDocument.LoadXml(level.textAsset.text);
            path = level.path;
            scenePath = path;
        }

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
            newCube.scene = this;

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
                    staticCube.scene = this;
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

        sceneStatus = SceneStatus.Idle;
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
