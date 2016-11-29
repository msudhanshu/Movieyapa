using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/**
 * Manages activity data (activity types) as well as activities that are not tied to a building.
 */
public class ActivityManager : Manager<ActivityManager> {

	/**
	 * A list of activity files (resources) to load.
	 */ 
	public List<string> activityDataFiles;

	/**
	 * Activity types mapped to ids.
	 */ 
	private Dictionary <string, ActivityData> types;

	/**
	 * Loader for loading the data.
	 */ 
	private Loader<ActivityData> loader;

	/**
	 * Activities currently in progress;
	 */ 
	virtual protected List<StateTransition> currentActivities {get; set;}
	
	/**
	 * The completed activities awaiting acknowledgement;
	 */ 
	virtual protected List<StateTransition> completedActivities {get; set;}

	/**
	 * Initialise the instance.
	 */
	override public void StartInit() {
		types = new Dictionary<string, ActivityData>();
		
		if (activityDataFiles != null){
			foreach(string dataFile in activityDataFiles){	
				LoadActivityDataFromResource(dataFile, false);
			}
		}
		currentActivities = new List<StateTransition>();
		completedActivities = new List<StateTransition>();
	}

	override public void PopulateDependencies() {
		dependencies = new List<ManagerDependency>();
		dependencies.Add(ManagerDependency.DATA_LOADED);
	}

	/**
	 * Get a list of each building type.
	 */ 
	virtual public List<ActivityData> GetAllBuildingTypes() {
		return types.Values.ToList();
	}

	/**
	 * Load the activity type data from the given resource.
	 * 
	 * @param dataFile	Name of the resource to load data from.
	 * @param skipDuplicates	If false throw an exception if a duplicate is found.
	 */
	virtual public void LoadActivityDataFromResource(string dataFile, bool skipDuplicates) {
		if (loader == null) loader = new Loader<ActivityData>();
		List <ActivityData> data = loader.LoadXML(dataFile);
		foreach (ActivityData type in data) {
			try {
				types.Add(type.type, type);
			} catch (System.Exception ex) {
				if (!skipDuplicates) throw ex;
			}
		}
	}

	/**
	 * Return the  data for the given activity type. Returns null if the activity type is not found.
	 */ 
	virtual public ActivityData GetActivityData(string type) {
		if (types.ContainsKey(type)) {
			return types[type];
		}
		return null;
	}
	
	/**
	 * Return the data for a save game (i.e. all current and completed activity data).
	 */ 
	virtual public List<StateTransition> GetSaveData() {
		List<StateTransition> result = new List<StateTransition>();
		result.AddRange(completedActivities);
		result.AddRange(currentActivities);
		return result;
	}

	/**
	 * Loads the saved game data.
	 */ 
	virtual public void Load(SaveGameData data) {
		StartCoroutine (DoLoad(data));
	}

	/**
	 * Does the loading of a saved game
	 */ 
	virtual protected IEnumerator DoLoad(SaveGameData data) {
		// Wait one frame to ensure everything is initialised
		yield return true;
		// Activities
		if (data.activities != null) {
			foreach (StateTransition a in data.activities) {
				if (a.RemainingTime <= 0) {
					completedActivities.Add(a);
					GameManager.GetInstance().gameView.SendMessage("UI_CompleteActivity", a.Type);
				} else {
					StartCoroutine (GenericActivity (a));
				}
			}
		}
	}
	/**
	 * Start generic activity and subtract any cost.
	 * Returns the activity if cost can be paid, otherwise returns null and doesn't start the activity.
	 */ 
	virtual public StateTransition StartActivity(string type, System.DateTime startTime, List<string> supportingIds) {
		ActivityData data = GetActivityData (type);
		if (data != null && ResourceManager.GetInstance().Silver < data.activityCost) return null;
		StateTransition activity = new StateTransition (type, data.durationInSeconds, startTime, supportingIds);
		ResourceManager.GetInstance().RemoveSilver(data.activityCost);
		StartCoroutine (GenericActivity (activity));
		return activity;
	}
	
	/**
	 *  Acknowledge generic activity.
	 */ 
	virtual public void AcknowledgeActivity(StateTransition activity) {
		if (completedActivities.Contains(activity)) {
			ActivityData data = GetActivityData(activity.Type);
			if (data != null) {
				switch (data.reward) {
				case RewardType.RESOURCE :
					ResourceManager.GetInstance().AddSilver(data.rewardAmount);
					break;
				case RewardType.GOLD :
					ResourceManager.GetInstance().AddGold(data.rewardAmount);
					break;
				case RewardType.CUSTOM_RESOURCE :
					ResourceManager.GetInstance().AddCustomResource(data.rewardId, data.rewardAmount);
					break;
				case RewardType.CUSTOM :
					// You need to include a custom reward handler if you use the CUSTOM RewardType
					SendMessage("CustomReward", activity, SendMessageOptions.RequireReceiver);
					break;
				}
				completedActivities.Remove (activity);
				GameManager.GetInstance().gameView.SendMessage("UI_AcknowledgeActivity");
				ResourceManager.GetInstance().AddXp (GetXpForCompletingActivity(data));
				if ((int)BuildingManager3D.GetInstance ().saveMode < (int) SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
			} else {
				Debug.LogError("Couldn't find data for activity: " + activity.Type);
			}
		}
	}

	/**
	 * Check for an activity by one of the supporting Id's and return the activity if found
	 * or null if not found. Note this is not a copy of the activity. Checks both current
	 * and completed activites.
	 */ 
	virtual public StateTransition CheckForActivityById(string id) {
		foreach (StateTransition activity in currentActivities) {
			if (activity.SupportingIds.Contains(id)) return activity;
		}
		foreach (StateTransition activity in completedActivities) {
			if (activity.SupportingIds.Contains(id)) return activity;
		}
		return null;
	}

	/**
	 * Find all activities which have data of a given class. Useful for finding custom activity types.
	 */ 
	virtual public List<StateTransition> GetActivitiesOfDataClassType(System.Type type) {
		List<StateTransition> result = new List<StateTransition>();
		foreach (StateTransition activity in currentActivities) {
			ActivityData data = GetActivityData(activity.Type);
			if (type.IsAssignableFrom(data.GetType())) result.Add(activity);
		}
		foreach (StateTransition activity in completedActivities) {
			ActivityData data = GetActivityData(activity.Type);
			if (type.IsAssignableFrom(data.GetType())) result.Add(activity);
		}
		return result;
	}

	/**
	 * Start a generic activity.
	 */
	virtual protected IEnumerator GenericActivity(StateTransition activity) {
		currentActivities.Add(activity);
		if ((int)BuildingManager3D.GetInstance ().saveMode < (int) SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
		GameManager.GetInstance().gameView.SendMessage("UI_StartActivity", activity);
		while (activity != null && activity.PercentageComplete < 1.0f ) {
			GameManager.GetInstance().gameView.SendMessage ("UI_UpdateProgress", activity);
			// Check more frequently as time gets closer, but never more frequently than once per second
			yield return new WaitForSeconds(Mathf.Max (1.0f, (float)activity.RemainingTime / 15.0f));
		}
		// If this wasn't triggered by a speed-up
		if (currentActivities.Contains (activity)) {
			completedActivities.Add(activity);
			currentActivities.Remove(activity);
			if ((int)BuildingManager3D.GetInstance ().saveMode < (int) SaveMode.SAVE_MOSTLY) PersistenceManager.GetInstance().Save();
			GameManager.GetInstance().gameView.SendMessage("UI_CompleteActivity", activity.Type);
		}
	}

	/// <summary>
	/// Gets the xp for completing an activity with the provided activity data.
	/// </summary>
	/// <returns>The xp for completing the activity.</returns>
	/// <param name="activity">Activity.</param>
	virtual public int GetXpForCompletingActivity(ActivityData data) {
		// Obivously pretty simple you might want to use data files or a lookup table
		if (data != null) {
			return data.rewardAmount + data.durationInSeconds;
		}
		return 0;
	}

	/// <summary>
	/// Gets the xp for completing the provided activity.
	/// </summary>
	/// <returns>The xp for completing the activity.</returns>
	/// <param name="activity">Activity.</param>
	virtual public int GetXpForCompletingActivity(StateTransition activity) {
		return	GetXpForCompletingActivity(GetActivityData (activity.Type));
	}
}
