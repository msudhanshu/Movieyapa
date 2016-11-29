//
//  Doober.cs
//
//  Author:
//       Manjeet <msudhanshu@kiwiup.com>
//

using System;
using UnityEngine;
using UnityEngine.UI;


public enum DooberState {
	POPPING_UP, 
	RESTING, 
	DISAPPEARING
	}


public class Doober : MonoBehaviour
{
	IGameResource gameResource;
	int quantity;
	
	
	public Image iconImage;
	public Text iconText;
	public RectTransform panelTransform;
	
	public DooberState? currentState = null;
	
	private Vector3 popoutWorldPosition;
	private Vector3 popoutScreenPosition {
		get {
			return Camera.main.WorldToScreenPoint(popoutWorldPosition);
		}
	}
	private Vector3 restingWorldPosition;
	private Vector3 restingScreenPosition {
		get {
			return Camera.main.WorldToScreenPoint(restingWorldPosition);
		}
	}
	
	private Vector3 terminationScreenPosition;
	
	float timeLapsed = 0;
	
	private Vector3 currentPosition;


	public void Init(IGameResource res, int quantity) {
		SetIconActive(false);
		gameResource = res;
		this.quantity = quantity;
		SetImage(CreateImageName());
		SetText(quantity+"");
	}

	public static Doober GetDoober(IGameResource res, int quantity) {
		Doober doober = null;
		Transform dooberPrefab = null;
		GameObject dooberIconObject = (GameObject) Resources.Load(Config.DooberPrefabName, typeof(GameObject)) ;
		if (dooberIconObject != null) //,Config.DOOBER_DISAPPEAR_TIMEOUT
			dooberPrefab = Pool.Instantiate( dooberIconObject, Vector3.zero, Quaternion.identity);

		if(dooberPrefab==null) return null;
		doober  = dooberPrefab.GetComponent<Doober>();
		if(doober!=null)
		doober.Init(res,quantity);
		return doober;
	}

	private string CreateImageName() {
		if( gameResource.getGameResourceType() == GameResourceType.COLLECTABLE ) 
			return Config.AddSuffix(gameResource.getId() , Config.COLLECTABLE_DOOBER_IMAGENAME_SUFFIX);
		else if ( gameResource.getGameResourceType() == GameResourceType.RESOURCE ) 
			return Config.AddSuffix(gameResource.getId() , Config.RESOURCE_DOOBER_IMAGENAME_SUFFIX);
		else
			return Config.DEFAULT_DOOBER_IMAGENAME;
	}

	public void SetIconActive(bool yes=true) {
		panelTransform.gameObject.SetActive(yes);
	}

	public void SetPosition( Vector3 popoutWorldPosition, Vector3 restingWorldPosition) {
		this.restingWorldPosition = restingWorldPosition;
		this.popoutWorldPosition = popoutWorldPosition;
		AssignTerminationPoint();
		this.currentPosition = popoutScreenPosition;
		currentState = DooberState.POPPING_UP;
		SetIconActive();
	}
	
	private void AssignTerminationPoint() {
		if (gameResource.getGameResourceType() == GameResourceType.RESOURCE)
			this.terminationScreenPosition = ResourceManager.GetInstance().GetResourceIconScreenPos(gameResource as DbResource);
		else
			this.terminationScreenPosition = new Vector3(-Screen.width,Screen.height,0);
	}
	
	public void SetImage(string imageName) {
		Sprite iconSprite = Resources.Load<Sprite>(imageName);
		if(iconSprite == null) Debug.LogError("Image not found in resource "+imageName);
		iconImage.sprite = iconSprite;
	}
	
	public void SetText(string text) {
		iconText.text = text;
	}

	//TODO : use itween or write function based animation for poping of doobers
	void Update() {
		timeLapsed += Time.deltaTime;
		switch(currentState) {
		case DooberState.POPPING_UP:
			//currentPosition = Vector3.Lerp(popoutWorldPosition,restingWorldPosition, animTimePassed/10.0f);
			//currentPosition = Camera.main.WorldToScreenPoint(Vector3.Lerp(popoutWorldPosition,restingWorldPosition, animTimePassed/10.0f));	
			currentPosition = Vector3.Lerp(popoutScreenPosition,restingScreenPosition, timeLapsed/Config.DOOBER_POPUP_TIME);
			if(timeLapsed/Config.DOOBER_POPUP_TIME >= 1) {
				timeLapsed = 0;
				currentState = DooberState.RESTING;
			}
			break;
		case DooberState.RESTING:
			currentPosition = restingScreenPosition;
			if(timeLapsed>Config.DOOBER_REST_TIME) { currentState = DooberState.DISAPPEARING; timeLapsed = 0; }
			break;
		case DooberState.DISAPPEARING:
			currentPosition = Vector3.Lerp(restingScreenPosition,terminationScreenPosition,timeLapsed/Config.DOOBER_DISAPPEAR_TIME);
			if(timeLapsed/Config.DOOBER_DISAPPEAR_TIME >= 1) {
				timeLapsed = 0;
				SetIconActive(false);
				ResourceManager.GetInstance().AddResource(gameResource, quantity);
				Pool.Destroy(this.gameObject);
			}

			break;
		}
		panelTransform.position = currentPosition;
	}
	
	public void OnIconButtonClicked() {
		SetState(DooberState.DISAPPEARING);
	}

	private void SetState(DooberState state) {
		currentState = state;
		timeLapsed = 0;
	}
}
