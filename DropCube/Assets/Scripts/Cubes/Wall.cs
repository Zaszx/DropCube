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
        return new Color(29.0f / 255.0f, 89.0f / 255.0f, 106.0f / 255.0f);
    }

    public override CubeType GetCubeType()
    {
        return CubeType.Wall;
    }
}
