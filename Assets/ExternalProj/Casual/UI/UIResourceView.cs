using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIResourceView : MonoBehaviour {

	public Text silverLabel;

	public Text goldLabel;

	public Text levelLabel;

	public Text axeLabel;
	public Text cheerLabel;

	public Slider xpProgress;

	public Text userIDLabel;

	public Sprite xpSprite;

	private int displayedSilver;
	private int displayedGold;
	private int displayedAxe;
	private int displayedCheer;

	void Start() {
		displayedSilver = 0;
		silverLabel.text = displayedSilver.ToString();
		displayedGold = 0;
		goldLabel.text = displayedGold.ToString();

		#region old_implementation
        /*		List<CustomResourceType> resources = ResourceManager.GetInstance().GetCustomResourceTypes();
		if (resources.Count == 0) {
			customResource1Label.gameObject.SetActive(false);
			customResource1Sprite.gameObject.SetActive(false);
		} else {
			customResource1Label.gameObject.SetActive(true);
			customResource1Sprite.gameObject.SetActive(true);
			customResource1Sprite.spriteName = resources[0].spriteName;
			displayedCustomResources1 = ResourceManager.GetInstance().GetCustomResource(resources[0].id);
			customResource1Label.text = displayedCustomResources1.ToString();
			customResourceType1 = resources[0].id;
		}
		if (resources.Count < 2) {
			customResource2Label.gameObject.SetActive(false);
			customResource2Sprite.gameObject.SetActive(false);
		} else {
			customResource2Label.gameObject.SetActive(true);
			customResource2Sprite.gameObject.SetActive(true);
			customResource2Sprite.spriteName = resources[1].spriteName;
			displayedCustomResources2 = ResourceManager.GetInstance().GetCustomResource(resources[1].id);
			customResource2Label.text = displayedCustomResources2.ToString();
			customResourceType2 = resources[1].id;
		}*/
		#endregion
		//ResourceManager.OnResourceUpdated += UpdateGold;
	}

	public void UpdateGenericResource(IGameResource resource) {
		StopCoroutine ("DisplayGenericResource");
		StartCoroutine ("DisplayGenericResource", new ResourceUpdate(resource, false));
	}

	public void UpdateGenericResourceInstant(IGameResource resource) {
		StopCoroutine ("DisplayGenericResource");
		StartCoroutine ("DisplayGenericResource", new ResourceUpdate(resource, true));
	}

	private IEnumerator DisplayGenericResource(ResourceUpdate resUpdate) {
		IGameResource resource = resUpdate.resource;
		bool instant = resUpdate.instant;

		switch (resource.getId()) {
		case "gold" :
			while( !UpdateGenericResource(ref displayedGold, ResourceManager.GetInstance().Gold, ref goldLabel, instant))
				yield return true;
			break;
		case "silver" :
			while( !UpdateGenericResource(ref displayedSilver, ResourceManager.GetInstance().Silver, ref silverLabel, instant))
				yield return true;
			break;
		case "axe" :
			while( !UpdateGenericResource(ref displayedAxe, ResourceManager.GetInstance().Axe, ref axeLabel, instant))
				yield return true;
            break;
		case "cheer" :
			while( !UpdateGenericResource(ref displayedCheer, ResourceManager.GetInstance().Cheer, ref cheerLabel, instant))
				yield return true;
            break;
        case "xp" :
			break;
		}
	}
	private bool UpdateGenericResource(ref int shownValue, int resManagerValue, ref Text label, bool instant = false) {
		if (instant) {
			label.text = resManagerValue.ToString();
			return true;
		}

		//Debug.LogError("Resource View shown " + shownValue + " Res Man Value " + resManagerValue + " instant " + instant);

		if (shownValue != resManagerValue) {
			int difference = shownValue - resManagerValue;
			if (difference > 2000) shownValue -= 1000;
			else if (difference > 200) shownValue -= 100;
			else if (difference > 20) shownValue -= 10;
			else if (difference > 0) shownValue -= 1;
			else if (difference < -2000) shownValue += 1000;
			else if (difference < -200) shownValue += 100;
            else if (difference < -20) shownValue += 10;
            else if (difference < 0) shownValue += 1;
            label.text = shownValue.ToString();
            return false;
        }
        
        return true;
    }

	public void UpdateLevel(bool showLevelUp) {
		levelLabel.text = ResourceManager.GetInstance().Level.ToString();
		
		xpProgress.value = (float)ResourceManager.GetInstance().Xp/(float)ResourceManager.GetInstance().XpRequiredForNextLevel();
		
		//xpSprite. = (float)(ResourceManager.GetInstance().Xp - ResourceManager.GetInstance().XpRequiredForCurrentLevel()) / (float)ResourceManager.GetInstance().XpRequiredForNextLevel();
		if (showLevelUp) {
			// TODO Some kind of level up thingy

			//call the level up popup with level up rewards 
		}
        userIDLabel.text = "uid:" + Config.USER_ID;
	}

	private class ResourceUpdate {
		public IGameResource resource;
		public bool instant;

		public ResourceUpdate(IGameResource resource, bool instant) {
			this.resource = resource;
			this.instant = instant;
		}
	}
}
