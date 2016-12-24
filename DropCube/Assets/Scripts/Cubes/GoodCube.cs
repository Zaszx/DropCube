using UnityEngine;
using System.Collections;

public class GoodCube : Cube 
{

	public override void Start () 
    {

        base.Start();
	}

    public override void Update() 
    {

        base.Update();
	}

    public override Color GetCubeColor()
    {
        return Color.green;
    }

    public override CubeType GetCubeType()
    {
        return CubeType.Good;
    }
}
