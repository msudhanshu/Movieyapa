using UnityEngine;
using System.Collections;

/**
 * Listens to a user clicking in path mode and sends to the path manager
 */ 
public class UIPathClickListener : MonoBehaviour {

	public Transform buildingOffset;
	/**
	 * Reference to the camera showing this object.
	 */ 
	protected UIDraggableCamera draggableCamera;

	/**
	 * Internal initialisation.
	 */
	void Awake(){
		draggableCamera = GameObject.FindObjectOfType(typeof(UIDraggableCamera)) as UIDraggableCamera;
	}

	/**
	 * When object is dragged, move object then snap it to the grid.
	 */ 
	void OnDrag (Vector2 delta) {
		draggableCamera.Drag(delta);
	}


	public void OnClick()
	{
		// TODO change that reference to be a look up of scale
		PathManager.GetInstance().SwitchPath("PATH", (GameManager.GetInstance().gameCamera.ScreenToWorldPoint(Input.mousePosition) / 0.003125f)  +
		                                     GameManager.GetInstance().gameView.transform.localPosition * -1.0f);
	}
}
