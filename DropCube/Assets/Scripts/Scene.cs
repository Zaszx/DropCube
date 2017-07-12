using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine.UI;

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
    Unloading,
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

    public List<bool> solution;

    public Vector2 sceneCenterOnScreen;

    public bool moveContainsError;

    public int screenIndex;
    public Image backgroundImage;

    public List<GameObject> exclams = new List<GameObject>();

    public Scene()
    {
        sceneStatus = SceneStatus.Loading;

        possibleGravityDirections[(int)Direction.Down] = Vector2.up;
        possibleGravityDirections[(int)Direction.Right] = Vector2.left;
        possibleGravityDirections[(int)Direction.Up] = Vector2.down;
        possibleGravityDirections[(int)Direction.Left] = Vector2.right;

        gravityDirection = Direction.Down;

        ticksParent = GameObject.Find("TicksParent");
        backgroundImage = GameObject.Find("Background").GetComponent<Image>();
        backgroundImage.sprite = Prefabs.playScreens[0];

        screenIndex = 0;

        moveContainsError = false;
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
        moveContainsError = false;
        GameObject.Destroy(levelRootObject);
    }

    public void UndoAllTriggered()
    {
        StaticCoroutine.StartCoroutine(UndoAllCoroutine());
    }

    public IEnumerator UndoAllCoroutine()
    {
        while(undoManager.doneOperations.Count > 0)
        {
            Coroutine c = StaticCoroutine.StartCoroutine(UndoCoroutine());
            yield return c;
        }
    }

    public int Generate(int complexity)
    {
        int movesRequired = -1;
        while (true)
        {

            Clear();
            int gridSize = Random.Range(complexity / 4, complexity / 3);
            gridSize = Mathf.Clamp(gridSize, 5, 15);
            CreateNewLevel(gridSize, gridSize);

            // Create Holes
            int holeCount = Random.Range(2, gridSize / 2);
            for (int i = 0; i < holeCount; i++)
            {
                bool holeInX = Random.value < 0.5f;
                int randomValue = Random.Range(0, gridSize - 1);
                bool holeInTop = Random.value < 0.5f;

                int holeXIndex = 0;
                int holeYIndex = 0;

                if (holeInX)
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
                if (currentCube.GetCubeType() == CubeType.Gray)
                {
                    i--;
                    continue;
                }
                if((holeXIndex == 0 && holeYIndex == 0) ||
                    (holeXIndex == gridSize - 1 && holeYIndex == 0) ||
                    (holeXIndex == 0 && holeYIndex == gridSize - 1) ||
                    (holeXIndex == gridSize - 1 && holeYIndex == gridSize - 1))
                {
                    i--;
                    continue;
                }

                ReplaceCubeWith(holeXIndex, holeYIndex, CubeType.Gray);
            }


            // Create cubes
            int cubeCount = Random.Range(complexity / 10, complexity / 3);
            cubeCount = Mathf.Min(cubeCount, (gridSize - 1) * (gridSize - 1) / 5);
            for (int i = 0; i < cubeCount; i++)
            {
                int randomX = Random.Range(1, gridSize - 2);
                int randomY = Random.Range(1, gridSize - 2);

                Cube currentCube = cubes[randomX, randomY];
                if (currentCube.GetCubeType() != CubeType.Gray)
                {
                    i--;
                    continue;
                }

                bool badCube = (i == 0) || Random.value < 0.5f;

                Cube cubeBelow = cubes[randomX, randomY + 1];
                if (randomX == 1 && cubeBelow.GetCubeType() == CubeType.Gray)
                {
                    i--;
                    continue;
                }

                if (cubeBelow.GetCubeType() == CubeType.Gray)
                {
                    ReplaceCubeWith(randomX, randomY + 1, CubeType.Wall);
                }

                ReplaceCubeWith(randomX, randomY, badCube ? CubeType.Bad : CubeType.Good);
            }


            // Generate Walls
            int wallCount = Random.Range(complexity / 10, complexity / 3);
            wallCount = Mathf.Min(cubeCount, (gridSize - 1) * (gridSize - 1) / 5);
            for (int i = 0; i < wallCount; i++)
            {
                int randomX = Random.Range(1, gridSize - 2);
                int randomY = Random.Range(1, gridSize - 2);

                Cube currentCube = cubes[randomX, randomY];
                if (currentCube.GetCubeType() != CubeType.Gray)
                {
                    i--;
                    continue;
                }

                ReplaceCubeWith(randomX, randomY, CubeType.Wall); ;
            }

            solution = new List<bool>();
            int movesRequiredToSolve = GetMovesRequiredToSolve();
            if(movesRequiredToSolve > 0)
            {
                movesRequired = movesRequiredToSolve;
                break;
            }
            int aa = solution.Count;
        }

        return movesRequired;
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
        while (currentDepth < 10 && solved == false)
        {
            currentDepth++;
            solved = Dfs(0, currentDepth, levelToInt, Direction.Down, new List<bool>());
        }
        if(solved)
        {
            return currentDepth - 1;
        }
        return -1;
    }

    public List<CubeType[,]> GetSolution()
    {
        List<CubeType[,]> solution = new List<CubeType[,]>();

        CubeType[,] initialPosition = new CubeType[levelWidth, levelHeight];
        for (int i = 0; i < levelWidth; i++)
        {
            for (int j = 0; j < levelHeight; j++)
            {
                initialPosition[i, j] = cubes[i, j].GetCubeType();
            }
        }

        CubeType[,] positionClone = new CubeType[levelWidth, levelHeight];
        for (int i = 0; i < levelWidth; i++)
        {
            for (int j = 0; j < levelHeight; j++)
            {
                positionClone[i, j] = cubes[i, j].GetCubeType();
            }
        }

        solution.Add(positionClone);
        Direction gravityDirection = Direction.Down;

        foreach(bool move in this.solution)
        {
            if (move)
            {
                gravityDirection = (Direction)((int)(gravityDirection + 1 + 4) % 4);
            }
            else
            {
                gravityDirection = (Direction)((int)(gravityDirection - 1 + 4) % 4);
            }
            UpdateCubesWithGravityDirection(initialPosition, possibleGravityDirections[(int)gravityDirection]);
            CubeType[,] clone = new CubeType[levelWidth, levelHeight];
            for (int i = 0; i < levelWidth; i++)
            {
                for (int j = 0; j < levelHeight; j++)
                {
                    clone[i, j] = initialPosition[i,j];
                }
            }
            solution.Add(clone);
        }
        return solution;
    }

    bool Dfs(int depth, int maxDepth, CubeType[,] cubes, Direction gravityDirection, List<bool> movements)
    {
        if(depth == maxDepth)
        {
            return false;
        }

        List<bool> movementsClone = new List<bool>();
        for(int i = 0; i < movements.Count; i++)
        {
            movementsClone.Add(movements[i]);
        }

        CubeType[,] cubesClone = new CubeType[levelWidth, levelHeight];
        for(int i = 0; i < levelWidth; i++)
        {
            for(int j = 0; j < levelHeight; j++)
            {
                cubesClone[i,j] = cubes[i,j];
            }
        }

        Vector2 currentDirection = possibleGravityDirections[(int)gravityDirection];
        bool result = UpdateCubesWithGravityDirection(cubesClone, currentDirection);
        if(result == false)
        {
            return false;
        }

        bool anyBadCube = false;

        for (int i = 0; i < levelWidth; i++)
        {
            for (int j = 0; j < levelHeight; j++)
            {
                if(cubesClone[i,j] == CubeType.Bad)
                {
                    anyBadCube = true;
                }
            }
        }

        if(anyBadCube == false)
        {
            solution = movementsClone;
            return true;
        }

        gravityDirection = (Direction)((int)(gravityDirection + 1 + 4) % 4);
        currentDirection = possibleGravityDirections[(int)gravityDirection];

        movementsClone.Add(true);

        result = Dfs(depth + 1, maxDepth, cubesClone, gravityDirection, movementsClone);
        if(result)
        {
            return true;
        }

        movementsClone.RemoveAt(movementsClone.Count - 1);
        movementsClone.Add(false);

        gravityDirection = (Direction)((int)(gravityDirection - 2 + 4) % 4);
        currentDirection = possibleGravityDirections[(int)gravityDirection];

        result = Dfs(depth + 1, maxDepth, cubesClone, gravityDirection, movementsClone);
        if(result)
        {
            return true;
        }


        return false;
    }

    bool UpdateCubesWithGravityDirection(CubeType[,] cubes, Vector2 gravityDirection)
    {
        Vector2 realGravityDirection = gravityDirection;
        Vector2 startingPosition = GetIterationBeginPoint(gravityDirection);
        if (gravityDirection.x == 0) gravityDirection.x = -1;
        if (gravityDirection.y == 0) gravityDirection.y = -1;
        //         for(int i = Mathf.RoundToInt(startingPosition.x); (i < levelWidth && i >= 0); i = i - Mathf.RoundToInt(gravityDirection.x))
        //         {
        //             for(int j = Mathf.RoundToInt(startingPosition.y); (j < levelHeight && j >= 0); j = j - Mathf.RoundToInt(gravityDirection.y))
        //             {
        for (int i = (int)startingPosition.x; (i < levelWidth && i >= 0); i = i - (int)gravityDirection.x)
        {
            for (int j = (int)startingPosition.y; (j < levelHeight && j >= 0); j = j - (int)gravityDirection.y)
            {
                CubeType currentCubeType = cubes[i, j];
                if(currentCubeType == CubeType.Bad || currentCubeType == CubeType.Good)
                {
                    Vector2 currentPosition = new Vector2(i, j);

                    while(true)
                    {
                        currentPosition = currentPosition + realGravityDirection;
                        if(IsInBounds(currentPosition))
                        {
                            CubeType cubeType = cubes[Mathf.RoundToInt(currentPosition.x), Mathf.RoundToInt(currentPosition.y)];
                            if(cubeType != CubeType.Gray)
                            {
                                Vector2 newPosition = currentPosition - realGravityDirection;
                                cubes[i, j] = CubeType.Gray;
                                cubes[Mathf.RoundToInt(newPosition.x), Mathf.RoundToInt(newPosition.y)] = currentCubeType;
                                break;
                            }
                        }
                        else
                        {
                            cubes[i, j] = CubeType.Gray;
                            if(currentCubeType == CubeType.Good)
                            {
                                return false;
                            }
                            break;
                        }
                    }
                }
            }
        }
        return true;
    }

    bool IsInBounds(Vector2 position)
    {
        int xpos = Mathf.RoundToInt(position.x);
        int ypos = Mathf.RoundToInt(position.y);
        if(xpos < levelWidth && xpos >= 0 && ypos < levelHeight && ypos >= 0)
        {
            return true;
        }
        return false;
    }

    void ReplaceCubeWith(int x, int y, CubeType newCube)
    {
        EditCube currentCube = cubes[x, y] as EditCube;
        currentCube.cubeType = newCube;
//         Cube currentCube = cubes[x, y];
//         if(currentCube.IsStatic() == false)
//         {
//             GameObject.Destroy(staticCubes[x, y].gameObject);
//         }
//         GameObject.Destroy(currentCube.gameObject);
// 
//         cubes[x, y] = newCube;
//         if(newCube.IsStatic())
//         {
//             staticCubes[x, y] = newCube;
//         }
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
                //newCube.UpdateShaderProperties();
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
        sceneCenterOnScreen = Camera.main.WorldToScreenPoint(sceneBounds.center);
    }

    public void Tick()
    {

    }


    private void AddUndoData(Dictionary<Cube, Vector3> cubeOriginalPositions, bool clockwise)
    {
        SceneUndoData newUndoData = new SceneUndoData();
        foreach (Cube c in dynamicCubes)
        {
            Vector3 cubeInitialPosition = c.transform.position;
            if (c.transform.position != cubeOriginalPositions[c])
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
    }

    public IEnumerator RotateCoroutine(bool clockwise)
    {
        sceneStatus = SceneStatus.Rotating;

        float totalTime = 0.3f;
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

        UpdateGridFromCubePositions();

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

        float goodCubeFallUnitTime = 0.1f;
        float badCubeFallUnitTime = 0.1f;

        int minGoodCubeFallDistance = 1000;
        int maxBadCubeFallDistance = 0;

        List<Coroutine> allCoroutinesToWait = new List<Coroutine>();
        for(int i = (int)iterationBeginPoint.x; (i < levelWidth && i >= 0); i = i - (int)iterationAmount.x)
        {
            for (int j = (int)iterationBeginPoint.y; (j < levelHeight && j >= 0); j = j - (int)iterationAmount.y)
            {
                Cube currentCube = cubes[i, j];
                if(currentCube.IsStatic() == false)
                {
                    Vector2 currentPosition = new Vector2(i, j);

                    while (true)
                    {
                        currentPosition = currentPosition + gravityInVec2;
                        if (IsInBounds(currentPosition))
                        {
                            Cube cubeToMove = cubes[Mathf.RoundToInt(currentPosition.x), Mathf.RoundToInt(currentPosition.y)];
                            if (cubeToMove.GetCubeType() != CubeType.Gray)
                            {
                                Vector2 targetPositionVec2 = currentPosition - gravityInVec2;
                                Vector3 targetPosition = new Vector3(targetPositionVec2.x, 0.01f, targetPositionVec2.y);
                                float fallTime = currentCube.GetCubeType() == CubeType.Good ? goodCubeFallUnitTime : badCubeFallUnitTime;
                                fallTime = fallTime * Vector2.Distance(targetPositionVec2, new Vector2(i, j));
                                Coroutine newCoroutine = StaticCoroutine.StartCoroutine(currentCube.MoveCoroutine(fallTime, targetPosition, GetStaticCubeWithIndex((int)(i + gravityInVec2.x), (int)(j + gravityInVec2.y))));
                                allCoroutinesToWait.Add(newCoroutine);

                                cubes[i, j] = staticCubes[i, j];
                                cubes[(int)targetPositionVec2.x, (int)targetPositionVec2.y] = currentCube;
                                break;
                            }
                        }
                        else
                        {
                            cubes[i, j] = staticCubes[i, j];
                            Vector2 targetPositionVec2 = currentPosition;
                            Vector3 targetPosition = new Vector3(targetPositionVec2.x, 0.01f, targetPositionVec2.y);
                            float fallTime = currentCube.GetCubeType() == CubeType.Good ? goodCubeFallUnitTime : badCubeFallUnitTime;
                            int distance = Mathf.RoundToInt(Vector2.Distance(targetPositionVec2, new Vector2(i, j)));
                            fallTime = fallTime * distance;
                            if(currentCube.GetCubeType() == CubeType.Good)
                            {
                                minGoodCubeFallDistance = Mathf.Min(minGoodCubeFallDistance, distance);
                            }
                            else
                            {
                                maxBadCubeFallDistance = Mathf.Max(maxBadCubeFallDistance, distance);
                            }
                            Coroutine newCoroutine = StaticCoroutine.StartCoroutine(currentCube.MoveCoroutine(fallTime, targetPosition, null));
                            allCoroutinesToWait.Add(newCoroutine);

                            break;
                        }
                    }

                }
            }
        }

        if(minGoodCubeFallDistance <= maxBadCubeFallDistance)
        {
            moveContainsError = true;
        }

        foreach (Coroutine c in allCoroutinesToWait)
        {
            yield return c;
        }

        UpdateGridFromCubePositions();

        AddUndoData(cubeOriginalPositions, clockwise);

        if (sceneStatus != SceneStatus.Errored)
        {
            sceneStatus = SceneStatus.Idle;
        }

        moveContainsError = false;
    }

    public IEnumerator UndoCoroutine()
    {
        foreach(GameObject exclam in exclams)
        {
            GameObject.Destroy(exclam);
        }
        exclams.Clear();

        sceneStatus = SceneStatus.Undoing;

        backgroundImage.sprite = Prefabs.playScreens[screenIndex];

        SceneUndoData undoData = undoManager.GetLastOperation();
        int maxFallAmount = 0;
        foreach(CubeUndoData cubeData in undoData.cubeData)
        {
            cubeData.fallAmount = Mathf.RoundToInt(Vector3.Distance(cubeData.startPosition, cubeData.endPosition));
            maxFallAmount = Mathf.Max(maxFallAmount, cubeData.fallAmount);
        }

        float waitAmount = 0.05f;
        List<Coroutine> allCoroutinesToWait = new List<Coroutine>();
        for(int i = maxFallAmount; i > 0; i--)
        {
            foreach (CubeUndoData cubeData in undoData.cubeData)
            {
                if(cubeData.fallAmount == i)
                {
                    cubeData.cube.gameObject.SetActive(true);
                    Coroutine c = StaticCoroutine.StartCoroutine(cubeData.cube.MoveTo(cubeData.startPosition, waitAmount * i));
                    allCoroutinesToWait.Add(c);
                }
            }
            yield return new WaitForSeconds(waitAmount);
        }

        foreach(Coroutine c in allCoroutinesToWait)
        {
            yield return c;
        }

        float totalTime = 0.2f;
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

    public IEnumerator WinLevel()
    {
        sceneStatus = SceneStatus.Unloading;
        backgroundImage.sprite = Prefabs.clearScreen;

        Coroutine c = StaticCoroutine.StartCoroutine(LoadLevelCoroutine(false));
        yield return c;

        levelRootObject.gameObject.SetActive(false);

        if (gameManager != null)
        {
            gameManager.OnLevelFinished(0);
        }
    }

    public void OnCubeFallsDown(Cube cube)
    {
        if(cube.GetCubeType() == CubeType.Good)
        {
            sceneStatus = SceneStatus.Errored;
            backgroundImage.sprite = Prefabs.failScreen;
            GameObject exclam = GameObject.Instantiate(Prefabs.exclam);
            exclam.transform.position = cube.transform.position + Vector3.up * 0.3f;
            exclams.Add(exclam);
        }
        else if(cube.GetCubeType() == CubeType.Bad)
        {
            Vector3 cubeLastPosition = cube.transform.position;
            cube.gameObject.SetActive(false);
            screenIndex++;
            if(screenIndex == 3)
            {
                screenIndex = 0;
            }
            if(sceneStatus != SceneStatus.Errored)
            {
                backgroundImage.sprite = Prefabs.playScreens[screenIndex];
            }

            if (moveContainsError == false)
            {
                bool andBadCubeLeft = false;
                foreach (Cube c in dynamicCubes)
                {
                    if (c.isActiveAndEnabled && c.GetCubeType() == CubeType.Bad)
                    {
                        andBadCubeLeft = true;
                    }
                }
                if (andBadCubeLeft == false && sceneStatus != SceneStatus.Errored)
                {
                    StaticCoroutine.StopAllCoroutines();
                    StaticCoroutine.StartCoroutine(WinLevel());
                }
//                 else if (gameManager != null)
//                 {
//                     StaticCoroutine.StartCoroutine(CreateTick(cubeLastPosition));
//                 }
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
            if(newCube.GetCubeType() == CubeType.Gray)
            {
                if (x == 0)
                {
                    GameObject prism = GameObject.Instantiate(Prefabs.prism);
                    prism.transform.position = new Vector3(-1, 0, y);
                    Vector3 prismRotation = new Vector3(0, 90, 0);
                    prism.transform.rotation = Quaternion.Euler(prismRotation);
                    prism.transform.parent = levelRootObject.transform;
                }
                if (y == 0)
                {
                    GameObject prism = GameObject.Instantiate(Prefabs.prism);
                    prism.transform.position = new Vector3(x, 0, -1);

                    prism.transform.parent = levelRootObject.transform;
                }
                if (x == levelWidth - 1)
                {
                    GameObject prism = GameObject.Instantiate(Prefabs.prism);
                    prism.transform.position = new Vector3(levelWidth, 0, y);
                    Vector3 prismRotation = new Vector3(0, -90, 0);
                    prism.transform.rotation = Quaternion.Euler(prismRotation);
                    prism.transform.parent = levelRootObject.transform;
                }
                if (y == levelHeight - 1)
                {
                    GameObject prism = GameObject.Instantiate(Prefabs.prism);
                    prism.transform.position = new Vector3(x, 0, levelHeight);
                    Vector3 prismRotation = new Vector3(0, 180, 0);
                    prism.transform.rotation = Quaternion.Euler(prismRotation);
                    prism.transform.parent = levelRootObject.transform;
                }
            }

            cubeNode = cubeNode.NextSibling;
        }

        sceneBounds = new Bounds();
        sceneBounds.Encapsulate(new Vector3(-0.5f, 0, -0.5f));
        sceneBounds.Encapsulate(new Vector3(levelWidth - 0.5f, 0, levelWidth - 0.5f));

        ComputeCameraPosition();
        sceneStatus = SceneStatus.Loading;
        levelRootObject.transform.localScale = Vector3.zero;
        StaticCoroutine.StartCoroutine(LoadLevelCoroutine(true));
    }

    public IEnumerator LoadLevelCoroutine(bool load)
    {
        float totalTime = 0.5f;
        float currentTime = 0.0f;

        while(currentTime < totalTime)
        {
            levelRootObject.transform.localScale = Vector3.Lerp(/*levelRootObject.transform.localScale*/ load ? Vector3.zero : Vector3.one, load ? Vector3.one : Vector3.zero, currentTime / totalTime);
            yield return new WaitForEndOfFrame();
            currentTime = currentTime + Time.deltaTime;
        }

        levelRootObject.transform.localScale = load ? Vector3.one : Vector3.zero;
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
                if(cubes[i,j] is EditCube)
                {
                    typeAttribute.Value = (cubes[i, j] as EditCube).cubeType.ToString();
                }
                else
                {
                    typeAttribute.Value = (cubes[i, j].GetCubeType().ToString());
                }
                cubeNode.Attributes.Append(typeAttribute);

                cubesNode.AppendChild(cubeNode);
            }
        }

        xmlDocument.Save(path);
    }

}
