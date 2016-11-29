using UnityEngine;
using System.Collections;

public class UIFpsViewExitPanel : MonoBehaviour
{	
	public void OnClick() {
		this.onFpsExitClicked ();
	}

	public void onFpsExitClicked(){
		GameManager.GetInstance().gameCamera.SendMessage("ExitFPSViewWithZoomOutOnCharacter");
	}

}

