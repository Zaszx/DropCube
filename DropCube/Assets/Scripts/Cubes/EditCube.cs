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

        thisRenderer.material.SetColor(ShaderProperties.colorId, GetCubeColor());
    }

    public override bool IsStatic()
    {
        if (cubeType == CubeType.Gray)
        {
            return true;
        }
        else if (cubeType == CubeType.Bad)
        {
            return false;
        }
        else if (cubeType == CubeType.Good)
        {
            return false;
        }
        else if (cubeType == CubeType.Wall)
        {
            return true;
        }
        return true;
    }

    public override Color GetCubeColor()
    {
        if(cubeType == CubeType.Gray)
        {
            return new Color(157.0f / 255.0f, 211.0f / 255.0f, 234.0f / 255.0f);
        }
        else if(cubeType == CubeType.Bad)
        {
            return Color.white;
        }
        else if(cubeType == CubeType.Good)
        {
            return Color.black;
        }
        else if(cubeType == CubeType.Wall)
        {
            return new Color(29.0f / 255.0f, 89.0f / 255.0f, 106.0f / 255.0f);
        }
        return Color.black;
    }

    public override CubeType GetCubeType()
    {
        return cubeType;
    }
}
