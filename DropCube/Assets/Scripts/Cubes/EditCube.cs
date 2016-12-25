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
        UpdateShaderProperties();
        base.Update();
	}

    public override void UpdateShaderProperties()
    {
        Renderer thisRenderer = GetComponent<Renderer>();

        thisRenderer.material.SetVector(ShaderProperties.argumentId, new Vector4(highlighted ? 1 : 0, selected ? 1 : 0, 0, 0));

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
