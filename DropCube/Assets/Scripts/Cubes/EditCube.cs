using UnityEngine;
using System.Collections;

public class EditCube : Cube 
{
    public CubeType cubeType;
    public bool highlighted;
    public bool selected;
	public override void Start () 
    {
        highlighted = false;
        selected = false;
        base.Start();
	}
	
	public override void Update () 
    {

        base.Update();
	}

    public virtual void UpdateShaderProperties()
    {
        Renderer thisRenderer = GetComponent<Renderer>();
        thisRenderer.material.SetColor(ShaderProperties.colorId, GetCubeColor());

        base.UpdateShaderProperties();
    }

    public override Color GetCubeColor()
    {
        if(cubeType == CubeType.Gray)
        {
            return Color.gray;
        }
        else if(cubeType == CubeType.Bad)
        {
            return Color.red;
        }
        else if(cubeType == CubeType.Good)
        {
            return Color.green;
        }
        return Color.black;
    }

    public override CubeType GetCubeType()
    {
        return cubeType;
    }
}
