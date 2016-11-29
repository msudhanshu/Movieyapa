using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIBuildingCompleteItem : MonoBehaviour
{
	public Text description;
	public Text status;

	public bool collectableSufficient = false;

	virtual public void InitialiseWithData(AssetStateCollectable assetStateCollectable) {
		description.text = assetStateCollectable.collectable.name;
		int currentCount = ResourceManager.GetInstance ().GetCollectableValue (assetStateCollectable.collectable);

		if (currentCount >= assetStateCollectable.GetQuantity ())
			collectableSufficient = true;

		status.text = currentCount + " / " + assetStateCollectable.GetQuantity();
	}
}
