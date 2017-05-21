using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCube : Cube 
{
    public Color color;
    public Vector3 initialPosition;
	// Use this for initialization
    public override void Start() 
    {
        initialPosition = transform.position;
        initialPosition.x = Mathf.RoundToInt(initialPosition.x);
        initialPosition.y = Mathf.RoundToInt(initialPosition.y);
        initialPosition.z = Mathf.RoundToInt(initialPosition.z);

        int randomColorIndex = Random.Range(0, 9);
        switch(randomColorIndex)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
                color = Color.grey;
                break;
            case 6:
                color = Color.red;
                break;
            case 7:
                color = Color.green;
                break;
            case 8:
                color = Color.black;
                break;
        }
        Distort();
        base.Start();
	}

    public void Distort()
    {
        transform.position = transform.position + Random.onUnitSphere * 3;
    }
	
	// Update is called once per frame
    public override void Update() 
    {
        transform.position = Vector3.Lerp(transform.position, initialPosition, Time.timeSinceLevelLoad * 0.05f);
        base.Update();
	}

    public override Color GetCubeColor()
    {
        return color;
    }

    public override CubeType GetCubeType()
    {
        return CubeType.Gray;
    }

}
