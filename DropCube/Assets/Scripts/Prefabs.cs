using UnityEngine;
using System.Collections;

public static class Prefabs 
{
    public static GameObject grayCube;
    
    static Prefabs()
    {
        grayCube = Resources.Load<GameObject>("Prefabs/GrayCube");
    }
}
