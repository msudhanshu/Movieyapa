using UnityEngine;
using System.Collections;
using Expansion;
public class ConfirmExpansionButton  : MonoBehaviour 
{
	public void OnClick() {
		if (ExpansionBlock.ActiveExpansion != null) {
			ExpansionBlock.ActiveExpansion.RequestExpansion();
			PopupManager.GetInstance().ShowPanel(PanelType.DEFAULT);
		}
	}
}

