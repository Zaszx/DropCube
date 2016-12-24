using UnityEngine;
using System.Collections;

public class Wall : Cube 
{
    public override void Start()
    {

        base.Start();
    }

    public override void Update()
    {

        base.Update();
    }

    public override Color GetCubeColor()
    {
        return Color.black;
    }

    public override CubeType GetCubeType()
    {
        return CubeType.Wall;
    }
}
