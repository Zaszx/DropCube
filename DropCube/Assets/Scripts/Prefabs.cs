using UnityEngine;
using System.Collections;

public static class Prefabs 
{
    public static GameObject grayCube;
    public static GameObject editCube;
    public static GameObject goodCube;
    public static GameObject badCube;
    public static GameObject wallCube;

    public static GameObject levelButton;
    
    static Prefabs()
    {
        grayCube = Resources.Load<GameObject>("Prefabs/GrayCube");
        editCube = Resources.Load<GameObject>("Prefabs/EditCube");
        goodCube = Resources.Load<GameObject>("Prefabs/GoodCube");
        badCube = Resources.Load<GameObject>("Prefabs/BadCube");
        wallCube = Resources.Load<GameObject>("Prefabs/WallCube");

        levelButton = Resources.Load<GameObject>("Prefabs/UI/levelButton");
    }

    public static GameObject GetCubePrefabWithType(CubeType type)
    {
        if(type == CubeType.Gray)
        {
            return grayCube;
        }
        else if(type == CubeType.Good)
        {
            return goodCube;
        }
        else if(type == CubeType.Bad)
        {
            return badCube;
        }
        else if(type == CubeType.Wall)
        {
            return wallCube;
        }
        return null;
    }
}
