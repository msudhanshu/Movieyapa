using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/**
 * Button for collecting stored resources.
 */ 
public class CollectButton : MonoBehaviour {
	
//	public Text resourceLabel;
//	public Image icon;
//	public Image ring;
	
	protected Building myBuilding;
	
	virtual public void Init(Building building) {
		myBuilding = building;
	//	icon.sprite = building.Type.generationType.ToString().ToLower() + "_icon";
	//	ring.color = UIColor.GetColourForRewardType(building.asset.generationType);
	//	StartCoroutine (DoUpdateResourceLabel());
	}
	
	virtual public void OnClick() {
		if (myBuilding.shouldAcknowledge())
			myBuilding.Acknowledge();
		PopupManager.GetInstance().ShowPanel (PanelType.DEFAULT);
	//	myBuilding.Collect();
	//	resourceLabel.text = "" + myBuilding.StoredResources;
	//	GameEventTask.notifyAction("03state2");
	}
	
	/**
	 * Coroutine to ensure the displayed resource is up to date.
	 */ 
/*	private IEnumerator DoUpdateResourceLabel() {
		resourceLabel.text = "" + myBuilding.StoredResources;
		while (gameObject.activeInHierarchy || myBuilding == null) {
			resourceLabel.text = "" + myBuilding.StoredResources;
			// Update frequently
			yield return new WaitForSeconds(0.25f);
		}
	}*/
}
