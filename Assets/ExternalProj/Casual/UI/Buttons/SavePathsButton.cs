using UnityEngine;
using System.Collections;

/**
 * Button which saves paths and exits path mode
 */ 
public class SavePathsButton : MonoBehaviour {

	public void OnClick() {
		PathManager.GetInstance().ExitPathMode();
		PopupManager.GetInstance().ShowPanel(PanelType.DEFAULT);
	}
}
