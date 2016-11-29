using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIButtonSwitchPanel : MonoBehaviour {
	
	public PanelType type;

	public void OnClick() {
		PopupManager.GetInstance().ShowPanel (type);
	}
}
