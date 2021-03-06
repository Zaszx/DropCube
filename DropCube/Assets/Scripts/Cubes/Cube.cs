﻿using UnityEngine;
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
        UpdateShaderProperties();
    }

    public virtual void Update () 
    {
        //UpdateShaderProperties();

    }

    public IEnumerator MoveTo(Vector3 position, float time)
    {
        float accumulatedTime = 0.0f;
        Vector3 initialPosition = transform.position;
        while(accumulatedTime < time)
        {
            transform.position = Vector3.Lerp(initialPosition, position, accumulatedTime / time);
            yield return new WaitForEndOfFrame();
            accumulatedTime = accumulatedTime + Time.deltaTime;
        }
        transform.position = position;
    }

    public virtual void UpdateShaderProperties()
    {
        Renderer thisRenderer = GetComponent<Renderer>();
        thisRenderer.material.SetColor(ShaderProperties.colorId, GetCubeColor());
    }

    public virtual Color GetCubeColor()
    {
        //return new Color(157.0f / 255.0f, 211.0f / 255.0f, 234.0f / 255.0f);
        //return new Color(0.514f, 0.851f, 1);
        return new Color(0.5f, 0.831f, 1);
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

    public IEnumerator MoveCoroutine(float totalTime, Vector3 targetPosition, Cube targetCube)
    {
        isMarkedToMove = true;

        Vector3 moveDirection = Vector3.forward;

        Vector3 initialPosition = transform.position;

        float accumulatedTime = 0.0f;

        while(accumulatedTime < totalTime)
        {
            var t = Curve.Instance.CubeMovement.Evaluate(accumulatedTime / totalTime);

            transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
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
