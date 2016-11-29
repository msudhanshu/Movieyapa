using UnityEngine;
using System.Collections;

public class CallOutUIFaceCamera : MonoBehaviour {
	public Transform target;
	// Use this for initialization
	void Start () {
		target = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {
		//Vector3 towardCam = target.forward - transform.position;
	//	transform.LookAt ( transform.position + transform.position - target.position);
		transform.rotation = target.rotation;
	//	transform.localScale =  
	}
}
