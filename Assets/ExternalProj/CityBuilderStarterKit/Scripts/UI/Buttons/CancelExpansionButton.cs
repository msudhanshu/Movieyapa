using UnityEngine;
using System.Collections;
using Expansion;
public class CancelExpansionButton  : MonoBehaviour 
	{
	public void OnClick() {
		ExpansionBlock.ActiveExpansion = null;
		PopupManager.GetInstance().ShowPanel(PanelType.DEFAULT);
	}
}

