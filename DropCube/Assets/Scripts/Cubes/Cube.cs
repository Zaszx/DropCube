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
    public bool isMarkedToMove;
    public Scene scene;
	public virtual void Start () 
    {
        isMarkedToMove = false;
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

    public virtual void OnFallDown()
    {
        scene.OnCubeFallsDown(this);
    }

    public IEnumerator MoveCoroutine(float totalTime, Cube targetCube)
    {
        isMarkedToMove = true;

        Vector3 moveDirection = Vector3.forward;

        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = initialPosition + moveDirection;

        float accumulatedTime = 0.0f;

        while(accumulatedTime < totalTime)
        {
            transform.position = Vector3.Lerp(initialPosition, targetPosition, accumulatedTime / totalTime);
            yield return new WaitForEndOfFrame();
            accumulatedTime = accumulatedTime + Time.deltaTime;
        }

        transform.position = targetPosition;

        if(targetCube == null)
        {
            OnFallDown();
        }

        isMarkedToMove = false;
    }
}
