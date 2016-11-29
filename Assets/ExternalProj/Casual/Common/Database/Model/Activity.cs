
using System.Collections;
using KiwiCommonDatabase;
using SimpleSQL;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class Activity : BaseDbModel {

	[PrimaryKey]
	public string id {get; set;}

	public string action {get; set;}

	public string description {get; set;}

	public string helperAction {get; set;}

	public string helperPosition {get; set;}

	public int activityIndex {get; set;}

	public string equipment {get; set;}

	public string characterBone {get; set;}

	private static string DEFAULT_ACTIVITY_ACTION = "default";
	public static string UPGRADE = "upgrade";

	private static Dictionary<string, Activity> standardActivitiesMap = new Dictionary<string, Activity>();

	/**
	 * Get activities based on standard activities.
	 * @param activity
	 * @return
	 */
	public static Activity FindActivity(ActivityName activity) {
		if(!standardActivitiesMap.ContainsKey(activity.ToString().ToLower())) {
			standardActivitiesMap.Add(activity.ToString().ToLower(), DatabaseManager.GetActivity(activity.ToString().ToLower()));
		}
		return standardActivitiesMap[activity.ToString().ToLower()];
	}

	public static Activity FindActivity(string activity) {
		if(!standardActivitiesMap.ContainsKey(activity) ) {
			standardActivitiesMap.Add(activity, DatabaseManager.GetActivity(activity));
		}
		return standardActivitiesMap[activity];

		//return DatabaseManager.GetActivity(activity);
		//return FindActivity (ActivityName.CONSTRUCT);
	}

	//TODO : CALLOUT ICON ASSOCIATED WITH EACH ACTIVITY
	/**
	 * Gets the texture asset to be used for this activity in the action icon.
	 * 1. If the asset for this activity is not present then the default action icon asset is returned. 
	 * @return
	 */
	public Sprite GetActionIconSprite(string customImagePath = null) {
		Sprite iconSprite = Resources.Load<Sprite>(GetActionIconImageName(customImagePath));
		if(iconSprite == null) {
			Debug.LogError("Image not found in resource "+GetActionIconImageName(customImagePath)+". Loading default callout="+Config.DEFAULT_CALLCOUT_ICON);
			iconSprite = Resources.Load<Sprite>(Config.DEFAULT_CALLCOUT_ICON);
		}
		return iconSprite;
	}
	public string GetActionIconImageName(string customImagePath = null) {
		if(customImagePath ==  null) return "new_callout_" +this.getAction();
		return customImagePath+ "/new_callout_" +this.getAction();
	}

	public bool IsActivity(ActivityName name) {
		return string.Equals(this.id, name.ToString(), System.StringComparison.CurrentCultureIgnoreCase); 
	}
	
	public string getAction() {
		return this.action == null ? DEFAULT_ACTIVITY_ACTION : this.action;
	}

	public string ToString() {
		return "Activity: {id: " + this.id + "}";
	}
	
	public string getAbsoluteName() {
		return this.id.ToLower();
	}

	/* (non-Javadoc)
     * @see java.lang.Object#equals(java.lang.Object)
     */
	
	public bool Equals(object obj) {
		if (this.Equals(obj))
			return true;
		if (obj == null)
			return false;
		if (!(obj is Activity))
			return false;
		Activity other = obj as Activity;
		if (other == null)
			return false;
		if (id == null) {
			if (other.id != null)
				return false;
		} else if (!id.Equals(other.id, System.StringComparison.CurrentCultureIgnoreCase))
			return false;
		return true;
	}
	
	/**
     * Checks if the passed state has the same next activity as this
     * @param state
     * @return
     */
	public bool isNextActivity(AssetState state) {
		return state.activity.Equals(this);
	}

}

public enum ActivityName {
	AUTO=0, SELL, MOVE, CLICK, HAPPYHARVEST, HARVEST, LIMITEDHARVEST, UPGRADE,UPGRADE2, FREE, ZCONTRACT, POSTCONTRACT, START, EXPLORE, BREAK,TEND,TRANSFORM,CONSTRUCT
};