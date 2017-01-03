using UnityEngine;
using System.Collections;

public class BadCube : Cube 
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
        return Color.red;
    }

    public override CubeType GetCubeType()
    {
        return CubeType.Bad;
    }

    public override bool IsStatic()
    {
        return false;
    }
}
