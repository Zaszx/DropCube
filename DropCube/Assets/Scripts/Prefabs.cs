using UnityEngine;
using System.Collections;

public static class Prefabs 
{
    public static GameObject grayCube;
    public static GameObject editCube;
    
    static Prefabs()
    {
        grayCube = Resources.Load<GameObject>("Prefabs/GrayCube");
        editCube = Resources.Load<GameObject>("Prefabs/EditCube");
    }
}
