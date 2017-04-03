using UnityEngine;
using System.Collections;

public enum SwipeStatus
{
    Idle,
    InProgress,
    Finished,
}

public class SwipeData
{
    public SwipeStatus swipeStatus;
    public bool isSingleClick;
    public Vector2 startPosition;
    public Vector2 endPosition;
    public Vector2 resultSwipe;


    public SwipeData()
    {
        Reset();
    }

    public void Reset()
    {
        swipeStatus = SwipeStatus.Idle;
        startPosition = Vector2.zero;
        endPosition = Vector2.zero;
        resultSwipe = Vector2.zero;
        isSingleClick = false;
    }

    public bool IsSwipeClockwise(Vector2 swipeDirection, Vector2 sceneCenter, Vector2 averageSwipePosition)
    {
        bool clockwise = false;


        if (averageSwipePosition.y < sceneCenter.y)
        {
            if (swipeDirection == Vector2.right)
            {
                clockwise = false;
            }
            else if (swipeDirection == Vector2.left)
            {
                clockwise = true;
            }
        }
        else
        {
            if (swipeDirection == Vector2.right)
            {
                clockwise = true;
            }
            else if (swipeDirection == Vector2.left)
            {
                clockwise = false;
            }
        }

        if (averageSwipePosition.x < sceneCenter.x)
        {
            if (swipeDirection == Vector2.up)
            {
                clockwise = true;
            }
            else if (swipeDirection == Vector2.down)
            {
                clockwise = false;
            }
        }
        else
        {
            if (swipeDirection == Vector2.up)
            {
                clockwise = false;
            }
            else if (swipeDirection == Vector2.down)
            {
                clockwise = true;
            }
        }

        return clockwise;
    }

    public void Tick()
    {
        if(swipeStatus == SwipeStatus.Idle && Input.GetMouseButtonDown(0))
        {
            swipeStatus = SwipeStatus.InProgress;
            startPosition = Input.mousePosition;
        }
        if(swipeStatus == SwipeStatus.InProgress && Input.GetMouseButtonUp(0))
        {
            endPosition = Input.mousePosition;

            resultSwipe = endPosition - startPosition;
            resultSwipe.Normalize();

            if(Vector2.Distance(endPosition, startPosition) < 1.0f)
            {
                isSingleClick = true;
            }
            else
            {
                isSingleClick = false;
            }

            swipeStatus = SwipeStatus.Finished;
        }
    }

}
