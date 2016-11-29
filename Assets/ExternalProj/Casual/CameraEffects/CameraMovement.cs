using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	/**
	 * --------------------------------------------------
	 * First person camera perspective -- starts
	 * --------------------------------------------------
	 **/
	public static bool FPS_VIEW_ENABLE_FLAG = false;			//Whether FPS view is enabled in the build
	private bool IS_FPS_VIEW_CURRENTLY_ENABLED = false;	//Whether FPS view is enabled at runtime; Could be enabled/disabled on starting/ending pathfinder
	public float smooth = 1.5f;         // The relative speed at which the camera will catch up.

	public Transform followedPlayer;           // Reference to the player's transform.
	private Vector3 relCameraPos;       // The relative position of the camera from the player.
	private float relCameraPosMag;      // The distance of the camera from the player.
	private Vector3 newPos;             // The position the camera is trying to reach.

	private Vector3 fpsStartPosition = Vector3.zero;
	private Quaternion fpsStartRotation = Quaternion.identity;

	private float RELATIVE_DIFF_X = -25.0f;
	private float RELATIVE_DIFF_Y = +16.0f;
	private float RELATIVE_DIFF_Z = -25.0f;

	private Vector3 playerCenterHeight = new Vector3(0.0f, 2.0f, 0.0f);

	private int currentCameraNodeIndex = 0;
	private int newCameraNodeIndex = -1;

	void Awake (){
		if(FPS_VIEW_ENABLE_FLAG){
			// Setting up the reference.
			//			player = GameObject.FindGameObjectWithTag(Tags.player).transform;
			
			// Setting the relative position as the initial relative position of the camera in the scene.
//			relCameraPos = transform.position - player.position;
//			relCameraPosMag = relCameraPos.magnitude - 0.5f;
			relCameraPos = new Vector3(RELATIVE_DIFF_X, RELATIVE_DIFF_Y, RELATIVE_DIFF_Z);
			relCameraPosMag = relCameraPos.magnitude - 0.5f;
		}
	}

	public bool IS_FPSVIEW_ENABLED_IN_BUILD(){
		return FPS_VIEW_ENABLE_FLAG;
	}

	public bool IS_FPSVIEW_ENABLED_NOW(){
		return FPS_VIEW_ENABLE_FLAG && IS_FPS_VIEW_CURRENTLY_ENABLED;
	}

	public void EnableFPSView(){
		if(!FPS_VIEW_ENABLE_FLAG){
			return;
		}

		//Do not show exit panel on concurrent character movement
		if(!IS_FPS_VIEW_CURRENTLY_ENABLED){
			PopupManager.GetInstance().ShowPanel(PanelType.FPS_EXIT);
		}

		fpsStartPosition = this.transform.position;
		fpsStartRotation = this.transform.rotation;

		IS_FPS_VIEW_CURRENTLY_ENABLED = true;
		MainCamera.IS_PANNING_ENABLED = false;
		MainCamera.IS_ZOOMING_ENABLED = false;
		MainCamera.IS_ROTATION_ENABLED = false;
	}

	public void DisableFPSView(){
		if(!FPS_VIEW_ENABLE_FLAG){
			return;
		}

		IS_FPS_VIEW_CURRENTLY_ENABLED = false;
		MainCamera.IS_PANNING_ENABLED = true;
		MainCamera.IS_ZOOMING_ENABLED = true;
		MainCamera.IS_ROTATION_ENABLED = true;
	}

	void FixedUpdate ()
	{
		if (FPS_VIEW_ENABLE_FLAG && IS_FPS_VIEW_CURRENTLY_ENABLED && followedPlayer != null) {
			// The standard position of the camera is the relative position of the camera from the player.
			Vector3 standardPos = followedPlayer.position + relCameraPos;

			// The abovePos is directly above the player at the same distance as the standard position.
			Vector3 abovePos = followedPlayer.position + relCameraPos + Vector3.up * relCameraPosMag;
		//	abovePos.y = 6.0f;

			// An array of 5 points to check if the camera can see the player.
			Vector3[] verticalCheckPoints = new Vector3[5];
			// The first is the standard position of the camera.
			verticalCheckPoints [0] = standardPos;
			// The next three are 25%, 50% and 75% of the distance between the standard position and abovePos.
			verticalCheckPoints [1] = Vector3.Lerp (standardPos, abovePos, 0.25f);
			verticalCheckPoints [2] = Vector3.Lerp (standardPos, abovePos, 0.5f);
			verticalCheckPoints [3] = Vector3.Lerp (standardPos, abovePos, 0.75f);
			// The last is the abovePos.
			verticalCheckPoints [4] = abovePos;

//			Vector3[] horizontalCheckPoints = new Vector3[3];
//			horizontalCheckPoints[0] = player.position + new Vector3(0.0f, RELATIVE_DIFF_Y, RELATIVE_DIFF_Z);							// 270 degree : towards us (-Z axis)
//			horizontalCheckPoints[1] = player.position + new Vector3(-1.0f*RELATIVE_DIFF_X, RELATIVE_DIFF_Y, RELATIVE_DIFF_Z);			// 315 degree
//			horizontalCheckPoints[2] = player.position + new Vector3(RELATIVE_DIFF_X, RELATIVE_DIFF_Y, RELATIVE_DIFF_Z);				// 235 degree :

			Vector3[] horizontalCheckPoints = new Vector3[8];
			horizontalCheckPoints[0] = followedPlayer.position + new Vector3(0.0f, RELATIVE_DIFF_Y, RELATIVE_DIFF_Z);							// 270 degree : towards us (-Z axis)
			horizontalCheckPoints[1] = followedPlayer.position + new Vector3(-1.0f*RELATIVE_DIFF_X, RELATIVE_DIFF_Y, RELATIVE_DIFF_Z);			// 315 degree
			horizontalCheckPoints[2] = followedPlayer.position + new Vector3(-1.0f*RELATIVE_DIFF_X, RELATIVE_DIFF_Y, 0.0f);						// 0 degree: towards right (+X axis)
			horizontalCheckPoints[3] = followedPlayer.position + new Vector3(-1.0f*RELATIVE_DIFF_X, RELATIVE_DIFF_Y, -1.0f*RELATIVE_DIFF_Z);	// 45 degree:
			horizontalCheckPoints[4] = followedPlayer.position + new Vector3(0.0f, RELATIVE_DIFF_Y, -1.0f*RELATIVE_DIFF_Z);						//	90 degree: away from us (+Z axis)
			horizontalCheckPoints[5] = followedPlayer.position + new Vector3(RELATIVE_DIFF_X, RELATIVE_DIFF_Y, -1.0f*RELATIVE_DIFF_Z);			// 135 degree
			horizontalCheckPoints[6] = followedPlayer.position + new Vector3(RELATIVE_DIFF_X, RELATIVE_DIFF_Y, 0.0f);							// 180 degree : towards left (-X axis)
			horizontalCheckPoints[7] = followedPlayer.position + new Vector3(RELATIVE_DIFF_X, RELATIVE_DIFF_Y, RELATIVE_DIFF_Z);				// 235 degree :

	//		Debug.Log ("Player position = " + player.position);
	//		Debug.Log ("Standard position = " + standardPos);
	//		Debug.Log ("Above position = " + abovePos);
		//	Debug.Log("checkPoint[0-1-2-3-4]= " + checkPoints[0] + " , " + checkPoints[1] + " , " + checkPoints[2] + " , " +  checkPoints[3] + " , " + checkPoints[4]);

			// Run through the vertical check points...
//			for (int i = 0; i < verticalCheckPoints.Length; i++) {
//					// ... if the camera can see the player...
//					if (ViewingPosCheck (verticalCheckPoints [i])){
//						//Debug.Log ("Selected Point position = " + i);
//					// ... break from the loop.
//							break;
//					}else if (i == verticalCheckPoints.Length -1){
//						newPos = verticalCheckPoints[i];
//					}
//			}


//			// Run through the horizontal check points...
//			for (int i = 0; i < horizontalCheckPoints.Length; i++) {
//				// ... if the camera can see the player...
//				if (ViewingPosCheck (horizontalCheckPoints [i])){
//					Debug.Log ("Selected Point position = " + i);
//					newCameraNodeIndex = i;
//					// ... break from the loop.
//					break;
//				}else{ 
//					if (i == horizontalCheckPoints.Length -1){
//						newCameraNodeIndex = i;
//						newPos = horizontalCheckPoints[i];
//					}
//				}
//			}

			// Run through the check points...
			for (int index = 0; index < horizontalCheckPoints.Length; index++) {
				int offset = (index %2 == 0) ? -1*(index/2) : 1+(index/2);
				int normalizedIndex = currentCameraNodeIndex + offset;
				if(normalizedIndex >= horizontalCheckPoints.Length){	//Edge conditions
					normalizedIndex -= horizontalCheckPoints.Length;
				}else if (normalizedIndex < 0){
					normalizedIndex += horizontalCheckPoints.Length;
				}

//				Debug.Log ("index = " + index + ", offset = " + offset + ", normalizedIndex= " + normalizedIndex);

				// ... if the camera can see the player...
				if (ViewingPosCheck (horizontalCheckPoints [normalizedIndex])){
					Debug.Log ("Selected Point position = " + normalizedIndex);
					newCameraNodeIndex = normalizedIndex;
					// ... break from the loop.
					break;
				}else{ 
					if (normalizedIndex == horizontalCheckPoints.Length -1){
						newCameraNodeIndex = normalizedIndex;
						newPos = horizontalCheckPoints[normalizedIndex];
					}
				}
			}

			if(currentCameraNodeIndex == newCameraNodeIndex && !transform.position.Equals(horizontalCheckPoints[newCameraNodeIndex])){
				if(transform.position.y == RELATIVE_DIFF_Y && ViewingPosCheck(transform.position)){
					//Dont update camera's transform position
					Debug.Log("Don't update camera position.");
				}else{
					// Lerp the camera's position between it's current position and it's new position.
					transform.position = Vector3.Lerp (transform.position, newPos, smooth * Time.deltaTime);
				}
			}else{
				currentCameraNodeIndex = newCameraNodeIndex;
				// Lerp the camera's position between it's current position and it's new position.
				transform.position = Vector3.Lerp (transform.position, newPos, smooth * Time.deltaTime);
			}


//			transform.position = Vector3.Lerp (transform.position, newPos, smooth * Time.deltaTime);

			//Debug.Log ("Interpolated camera position = " + transform.position);
//			transform.position = new Vector3 (player.position.x - 25.0f, 6.0f, player.position.z - 25.0f);

//			Vector3 trailPos = player.position - 25.0f*player.forward;
//			transform.position = new Vector3(trailPos.x, 10.0f, trailPos.z);

			//Debug.Log ("Camera position = " + transform.position);

			// Make sure the camera is looking at the player.
			SmoothLookAt ();
		} else {


		}
	}
	
	
	bool ViewingPosCheck (Vector3 checkPos)
	{
		RaycastHit hit;
		// If a raycast from the check position to the player hits something...
		Vector3 direction = followedPlayer.position + playerCenterHeight - checkPos;
		//Debug.DrawRay (checkPos, (direction / direction.magnitude) * relCameraPosMag, Color.red, 0.5f, false);
//		Debug.DrawLine (checkPos, player.position + playerCenterHeight, Color.red, 2, false);
		if (Physics.Raycast (checkPos, direction, out hit, relCameraPosMag)) {
			// ... if it is not the player...
			if (hit.transform != followedPlayer){
				// This position isn't appropriate.
				return false;
			}
		}
		// If we haven't hit anything or we've hit the player, this is an appropriate position.
		newPos = checkPos;
		return true;
	}
	
	
	void SmoothLookAt ()
	{
		// Create a vector from the camera towards the player.
		Vector3 relPlayerPosition = followedPlayer.position - transform.position;
		
		// Create a rotation based on the relative position of the player being the forward vector.
		Quaternion lookAtRotation = Quaternion.LookRotation(relPlayerPosition, Vector3.up);
		
		// Lerp the camera's rotation between it's current rotation and the rotation that looks at the player.
		transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, smooth * Time.deltaTime);
	}


	public void ExitFPSViewWithZoomOutOnCharacter(){
		if(!FPS_VIEW_ENABLE_FLAG){
			return;
		}

	
	}

	/**
	 * --------------------------------------------------
	 * First person camera perspective -- ends
	 * --------------------------------------------------
	 **/
	
}
