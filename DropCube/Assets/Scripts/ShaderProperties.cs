using UnityEngine;
using System.Collections;

public static class ShaderProperties
{
    public static int colorId;
    public static int argumentId;
    
    static ShaderProperties()
    {
        colorId = Shader.PropertyToID("_Color");
        argumentId = Shader.PropertyToID("_Argument");
    }
}
