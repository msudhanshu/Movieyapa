using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class CameraFacingBillboard: MonoBehaviour 
{
	Transform MyTransform;
	Camera Cam;
	// Use this for initialization
	void Start () 
	{
		MyTransform = transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		MyTransform.LookAt(Camera.main.transform.position, Camera.main.transform.up);
	}
}