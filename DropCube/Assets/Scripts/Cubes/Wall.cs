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
        //return new Color(29.0f / 255.0f, 89.0f / 255.0f, 106.0f / 255.0f);
        //return new Color(0.035f, 0.216f, 0.267f);
        //return new Color(0.135f, 0.316f, 0.367f);
        return new Color(0.185f, 0.366f, 0.407f);
    }

    public override CubeType GetCubeType()
    {
        return CubeType.Wall;
    }
}
