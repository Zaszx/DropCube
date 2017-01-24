using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoManager
{
    public Stack<SceneUndoData> doneOperations = new Stack<SceneUndoData>();

    public UndoManager()
    {

    }

    public void AddData(SceneUndoData newUndoData)
    {
        doneOperations.Push(newUndoData);
    }

    public SceneUndoData GetLastOperation()
    {
        return doneOperations.Pop();
    }

    public bool IsEmpty()
    {
        return doneOperations.Count == 0;
    }
}
