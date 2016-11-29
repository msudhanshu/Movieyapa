using UnityEngine;
using System.Collections;
using SimpleSQL;
using System;

public enum UserAssetPropertyName {
	DPB,	
	HEALTH,
	HEALTHBOOST,
	ARMOR,
	MAXENERGY,
	CAPACITY,
	PRODUCTIONRATE, 
	GENERATORCOSTDISCOUNT, 
	GENERATIONTIMER, 
	RANGE,
	CRITICALCHANCEBOOSTPERCENTAGE,
	CRITICALCHANCEPROBABILITY,
	DPBPERCENTAGE,
	ARMORPERCENTAGE,
	EVOLVECOLLECTABLE, 
	EVOLVERESOURCE,
	NEXTEVOLVE,
	ENHANCEXP
}

public static class UserAssetPropertyNameExtension {
	
	public static float Get(this UserAssetPropertyName userAssetProp, UserAsset uAsset){
		return userAssetProp.Get(uAsset, uAsset.assetState, uAsset.level, 0f);
	}
	
	public static float Get(this UserAssetPropertyName userAssetProp, UserAsset uAsset, float defaultValue) {
		return userAssetProp.Get(uAsset, uAsset.assetState, uAsset.level, defaultValue);
	}

	public static float Get(this UserAssetPropertyName userAssetProp, AssetState state, int level, float defaultValue) {
		float bonus = 0;
		//bonus from any charge
		//bonus += UserCharge.getCharge(state, this);
		return state.GetFloatProperty(userAssetProp.getName(), defaultValue, level) * (1 + bonus);
	}
	
	public static float Get(this UserAssetPropertyName userAssetProp, UserAsset uAsset, AssetState state, int level, float defaultValue) {
		float bonus = 0;
		//bonus from any charge
		//bonus += UserCharge.getCharge(uAsset, this);
		return state.GetFloatProperty(userAssetProp.getName(), defaultValue, level)*(1 + bonus);
	}

	public static string getName(this UserAssetPropertyName userAssetProp) {
		//return Utility.StringToLower(userAssetProp.ToString());
		return Utility.StringToLower(Enum.GetName(typeof(UserAssetPropertyName),userAssetProp));
	}
	
	/**Gets the property from string
		 * @param propertyName
		 * @return
	*/
	public static UserAssetPropertyName getProperty(string propertyName) {
		return (UserAssetPropertyName)Enum.Parse(typeof(UserAssetPropertyName), propertyName);
	}
}