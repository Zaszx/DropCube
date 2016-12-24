using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
    Scene scene;
	void Start () 
    {
        scene = new Scene();
        scene.CreateNewLevel(10, 10);
	}
	

	void Update () 
    {
	
	}

}
