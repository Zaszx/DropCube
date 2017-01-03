using UnityEngine;
using System.Collections;

public enum CubeType
{
    Gray,
    Good,
    Bad,
    Wall,
}

public class Cube : MonoBehaviour 
{

	public virtual void Start () 
    {
	
	}
	
	public virtual void Update () 
    {
        UpdateShaderProperties();

    }

    public virtual void UpdateShaderProperties()
    {
        Renderer thisRenderer = GetComponent<Renderer>();
        thisRenderer.material.SetColor(ShaderProperties.colorId, GetCubeColor());
    }

    public virtual Color GetCubeColor()
    {
        return Color.gray;
    }

    public virtual bool IsStatic()
    {
        return true;
    }

    public virtual CubeType GetCubeType()
    {
        return CubeType.Gray;
    }
}
