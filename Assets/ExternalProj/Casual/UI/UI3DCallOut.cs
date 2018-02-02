//
//  UI3DCallOut.cs
//
//  Author:
//       Manjeet <msudhanshu@kiwiup.com>
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityExtensions;

/*
 * UI on top of building to have callout and activity timer.
 * It will be 3d ui always facing the camera in horizontal plane (not in vertical dir)..
 * need a billboard shader for it .
 * It can have further different shader (of type billboard) for diff effects( glow, progress bar using shader etc)
 */
using UnityEngine.UI;

public class UI3DCallOut : MonoBehaviour
{
	Transform callout;
	TextMesh textMesh;

	public CanvasGroup calloutButtonGroup;
	public Slider progressBarSlider;
	public Text progressPercentageText;
	public Button calloutButton;
	public Image calloutImage;
	ICallOutClickListner callOutClickListner=null;
	private Vector3 worldPos;
	public void SetWorldPosition(Vector3 _worldPos) {
		this.transform.position = _worldPos;
		this.worldPos = _worldPos;
	}

	//TEMP : HACK : MANJEET
	//Unity 4.6 ui bug hack..
	public RectTransform SliderfillPanelHack;
	private void Init() {
		SliderfillPanelHack.localPosition = Vector3.zero;
	//	SliderfillPanelHack.localPosition.y=0;
	}

	public static UI3DCallOut GetUI3DActivityStatus(Transform parent, Vector3 callOutOffsetPos,ICallOutClickListner callOutClickListner=null) {
		UI3DCallOut ui3DCallOut = null;

		Transform activityStatusPrefab = null;
		GameObject activityStatusObject = (GameObject) Resources.Load(Config.UI3DActivityStatusPrefabName, typeof(GameObject)) ;
		if (activityStatusObject != null)
			activityStatusPrefab = Pool.Instantiate( activityStatusObject, Vector3.zero, Quaternion.identity);
		
		if(activityStatusPrefab==null) return null;
		ui3DCallOut  = activityStatusPrefab.GetComponent<UI3DCallOut>();
		if(ui3DCallOut!=null) {
			activityStatusPrefab.transform.parent = parent;
			ui3DCallOut.SetWorldPosition(parent.position + callOutOffsetPos);
			if(callOutClickListner!=null) ui3DCallOut.SetCallOutClickListner(callOutClickListner);
			ui3DCallOut.Init();
		}
		return ui3DCallOut;
	}

	public void SetCallOutClickListner(ICallOutClickListner callOutClickListner) {
		this.callOutClickListner = callOutClickListner;
	}

	protected void Shuffle(GameObject toShowContainer, GameObject toHideContainer) {
		toShowContainer.SetActive(true);
		toHideContainer.SetActive(false);
	}

	public void ShowProgressBar() {
		if(calloutButton!= null && progressBarSlider!= null)
		Shuffle(progressBarSlider.gameObject,calloutButton.gameObject);
	}
	
	//need to pass callout type
	public void ShowCallOut(Sprite callOutIcon) {
		calloutButtonGroup.alpha = 1;
		if (calloutImage != null) {
			calloutImage.sprite = callOutIcon;
		}
		if(calloutButton!= null && progressBarSlider!= null)
			Shuffle(calloutButton.gameObject,progressBarSlider.gameObject);
	}

	public void SetCallOut(string imageName) {
		Sprite iconSprite = Resources.Load<Sprite>(imageName);
		if(iconSprite == null) Debug.LogError("Image not found in resource "+imageName);
		calloutImage.sprite = iconSprite;
		if(calloutButton!= null && progressBarSlider!= null)
			Shuffle(calloutButton.gameObject,progressBarSlider.gameObject);
	}

	public void CallOutClicked() {
		calloutButtonGroup.alpha = 0.5f;
		if(callOutClickListner!=null) callOutClickListner.OnCallOutClick();
	}

	public void UpdateProgressUI(float percentage) {
		progressBarSlider.value = percentage;
		string percentageString = (percentage*100).ToString();
		percentageString = percentageString.Substring(0,Math.Min(5,percentageString.Length)) + "%";
		progressPercentageText.text = percentageString;
	}

	public void SetText(string text) {
		if (callout == null) {
			callout = gameObject.transform.Find("CallOut");
			if(callout==null)
				callout = Util.SearchHierarchyForBone(transform,"CallOut");
		}
		if(callout==null) return;
		if (textMesh == null) {
			textMesh = callout.gameObject.GetComponent<TextMesh>();
		}

	//	if(callout != null)
//		callout.transform.SetPositionY(gameObject.GetComponent<UserAssetController>().asset.sizeHeight + 5);
		if(textMesh != null)
		textMesh.text = text;
	}



	//FIXME : if we want callout to be screen space 
	/*private Vector3 screenPos {
		get {
			return Camera.main.WorldToScreenPoint(worldPos);
		}
	}
	public RectTransform panelTransform;
	void Update() {
		if(panelTransform != null)
			panelTransform.position = screenPos;
	}
	*/
}

public interface ICallOutClickListner {
	void OnCallOutClick();
}
