using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Config {
	public static string USER_ID_KEY = "user_id";
	public static int USER_ID = 7;
	public static bool FIREBASE_SERVER = false;
	public static string CURRENT_BOOST_ITEM = "current_boost_item";

	public static string DEFAULT_CALLCOUT_ICON = "new_callout_default";
	public static bool ENABLE_CUTSCENE = false;
	/**
	 * Doober Enum Types
	 */
	public static string DooberPrefabName = "DooberIcon";
	public static string UI3DActivityStatusPrefabName = "UI3DActivityStatus";
	public static float DOOBER_SCALE = 0.8f;
	public static float DOOBER_FALL_SCALE_X = DOOBER_SCALE + 0.2f;
	public static float DOOBER_FALL_SCALE_Y = DOOBER_SCALE - 0.1f ;
	public static float DOOBER_JERK_SCALE = DOOBER_SCALE + 0.05f ;
	public static float DOOBER_SCALE_TIME = 0.05f;
	public static int DOOBER_WIDTH = 64 ;
	public static int DOOBER_DROP_MIN_SPACING = 5;
	
	public static float IDLE_DOOBER_SCALEX = DOOBER_SCALE + 0.1f;
	public static float IDLE_DOOBER_SCALEY = DOOBER_SCALE - 0.05f;
	public static float IDLE_DOOBER_SCALE_TIME = 0.75f ;
	public static float IDLE_DOOBER_WAIT_TIME = 0.5f ;
	
	
	public static int MAX_DOOBER_WIDTH = 64 ;
	
	public static int A_HUD_PROGRESSBAR_WIDTH = 143;
	public static string COLLECTABLE_DOOBER_IMAGENAME_SUFFIX = "doober";
	public static string RESOURCE_DOOBER_IMAGENAME_SUFFIX = "last";
	public static string DEFAULT_DOOBER_IMAGENAME = "callout_default";
	public static float DOOBER_VELOCITY = 0.25f;
	public static int DOOBER_DISAPPEAR_TIMEOUT = 5; //in sec

	public static float DOOBER_POPUP_TIME = 0.2f; //in sec
	public static float DOOBER_REST_TIME = 1.5f; //in sec
	public static float DOOBER_DISAPPEAR_TIME = 0.7f; //in sec

	public static bool HELPER_FORCE_TELEPORT = false;

	/**
	 * At a given time the level up cannot happen for more than 20 levels.
	 */
	public static int MAX_LEVEL_INCREMENT = 20;

	public static string SEPERATOR = "_";

	public static string AddSuffix(string baseString , string suffix) {
		return ( baseString + "" + (suffix==null?"":(SEPERATOR+suffix)) );
	}

	public static string DefaultAssetCategoryPrefabName = "Building3D";
	public static Dictionary<AssetCategoryEnum,string> CategoryPrefabNameMap = new  Dictionary<AssetCategoryEnum,string> 
	{
		{AssetCategoryEnum.BUILDING, "Building3D"},
		{AssetCategoryEnum.TOWNBLDG, "Building3D"},
		{AssetCategoryEnum.CROP , "Building3D"},
		{AssetCategoryEnum.DECORATION , "Building3D"},
		{AssetCategoryEnum.TREE , "Building3D"},
		{AssetCategoryEnum.HELPER, "Character3D"},
		{AssetCategoryEnum.RPGCHARACTER, "RpgCharacter3D"},
		{AssetCategoryEnum.EXPANSION, "Building3D"}
	};

}

