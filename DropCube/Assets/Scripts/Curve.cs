using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Curve : ScriptableObject
{
    private static Curve _instance;

    public static Curve Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<Curve>("Curve");
            }

            return _instance;
        }
    }

    public AnimationCurve CubeMovement;
    public AnimationCurve CubeUndoMovement;
    public AnimationCurve SceneMovement;
    public AnimationCurve SkyColorChangeFail;
    public AnimationCurve SkyColorChangeUndo;
    public AnimationCurve LevelLoadScale;
}
