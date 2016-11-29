using UnityEngine;
using System.Collections;

public class OrthoGridCamera : MonoBehaviour {
	
	
	#region ToolCamera 
	public Camera arenaCamera;
	public Camera tileCamera;

	public float sensitivityX = 8F;
	public float sensitivityY = 8F;
	public float sensitivityZoom = 8F;
	public float MaxZoomOut = 100;
	public float MinZoomIn = 10;

	void Update() {
		UpdateCamera ();
	}

	private void UpdateCamera() {
		//if (!(Input.GetMouseButton (2))) return;
		float deltaX = Input.GetAxis("Mouse X") * sensitivityX;
		float deltaZ = Input.GetAxis("Mouse Y") * sensitivityY;
		float deltaZoom = Input.GetAxis ("Mouse ScrollWheel") * sensitivityZoom;
		if ((Input.GetMouseButton(2) ) )
			TranslateCamera(deltaX, deltaZ);
		ZoomCamera (deltaZoom);
	}
	
	void ZoomCamera(float deltaZoom) {
		//Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize-deltaY, 1, 6);
		float cameraFOVSize =  Mathf.Clamp (arenaCamera.orthographicSize-deltaZoom, MinZoomIn, MaxZoomOut);
		arenaCamera.orthographicSize = cameraFOVSize;
		tileCamera.orthographicSize = cameraFOVSize;
	}
	
	void TranslateCamera(float x, float y) {
		arenaCamera.transform.position -= new Vector3 (x, 0, y);
	}
	#endregion

}