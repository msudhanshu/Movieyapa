using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraMover : MonoBehaviour
{
	//#ToDo: Use Vectors instead of Camera objects for coordinates.
//	public Camera gameCamera;
	public List<Camera>Cameras = new List<Camera>();
	public float transitionTime = 1.0f;	//updated by Unity Editor

	// Use this for initialization
	void Awake ()
	{

	}

//	public void Update(){
//
//	}

}

