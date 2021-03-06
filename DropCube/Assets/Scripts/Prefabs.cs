﻿using UnityEngine;
using System.Collections;

public static class Prefabs 
{
    public static GameObject grayCube;
    public static GameObject editCube;
    public static GameObject goodCube;
    public static GameObject badCube;
    public static GameObject wallCube;

    public static GameObject levelButton;
    public static GameObject tickObject;
    public static GameObject exclam;

    public static Sprite initialScreen;
    public static Sprite[] playScreens = new Sprite[3];
    public static Sprite levelSelectScreen;
    public static Sprite failScreen;
    public static Sprite clearScreen;

    public static GameObject prism;
    
    static Prefabs()
    {
        grayCube = Resources.Load<GameObject>("Prefabs/GrayCube");
        editCube = Resources.Load<GameObject>("Prefabs/EditCube");
        goodCube = Resources.Load<GameObject>("Prefabs/GoodCube");
        badCube = Resources.Load<GameObject>("Prefabs/BadCube");
        wallCube = Resources.Load<GameObject>("Prefabs/WallCube");
        prism = Resources.Load<GameObject>("Prefabs/Prism");
        exclam = Resources.Load<GameObject>("Prefabs/exclam");

        levelButton = Resources.Load<GameObject>("Prefabs/UI/levelButton");

        tickObject = Resources.Load<GameObject>("Prefabs/UI/TickIcon");

        playScreens[0] = Resources.Load<Sprite>("Images/backgrounds/play-screen");
        playScreens[1] = Resources.Load<Sprite>("Images/backgrounds/play-screen_2");
        playScreens[2] = Resources.Load<Sprite>("Images/backgrounds/play-screen_3");

        failScreen = Resources.Load<Sprite>("Images/backgrounds/fail-screen");

        initialScreen = Resources.Load<Sprite>("Images/backgrounds/start-screen");

        clearScreen = Resources.Load<Sprite>("Images/backgrounds/clear-screen");
        levelSelectScreen = Resources.Load<Sprite>("Images/backgrounds/levelselect-screen");
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
