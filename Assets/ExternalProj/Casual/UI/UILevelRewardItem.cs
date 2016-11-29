using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//Item fo base/shop popup. It will have only look (name,image,clicklistner)/IBaseItemInfo of the item.
public class UILevelRewardItem : MonoBehaviour
{
	public Text count;
	public Image image;
	
	private string CreateImageName(IGameResource gameResource) {

		if( gameResource.getGameResourceType() == GameResourceType.COLLECTABLE ) 
			return Config.AddSuffix(gameResource.getId() , Config.COLLECTABLE_DOOBER_IMAGENAME_SUFFIX);
		else if ( gameResource.getGameResourceType() == GameResourceType.RESOURCE ) 
			return Config.AddSuffix(gameResource.getId() , Config.RESOURCE_DOOBER_IMAGENAME_SUFFIX);
		else
			return Config.DEFAULT_DOOBER_IMAGENAME;
	}

	public void SetImage(string imageName) {
		Sprite iconSprite = Resources.Load<Sprite>(imageName);
		if(iconSprite == null) Debug.LogError("Image not found in resource "+imageName);
		image.sprite = iconSprite;
	}

	virtual public void InitialiseWithData(LevelReward levelReward) {
		count.text = "+" + levelReward.quantity;
		SetImage(CreateImageName(levelReward.resource));
	}
}
