using UnityEngine;
using System.Collections;

public static class ShaderProperties
{
    public static int colorId;
    
    static ShaderProperties()
    {
        colorId = Shader.PropertyToID("_Color");
    }
}
