//
//  ActivityController.cs
//
//  Author:
//       Manjeet <msudhanshu@kiwiup.com>
//

using UnityEngine;

/*
 * PlaceableGridObject-StateTransition will call this to get helper and do the activity.
 * This class will only talk to charactermanager and charactergridobject
 */

public class ActivityController : CoreBehaviour,IHelperReachedListener {
	Transform equipment;

	IActivityTarget activityTarget;
	CharacterGridObject helper;
	Activity activity;

	public void RegisterAsTarget(IActivityTarget target) {
		this.activityTarget = target;
	}
	//DEPENDS ON ACITIVTY TYPE;
	public bool HelperRequired(){
		if(Utility.StringEquals(activity.id,"auto") ) return false;
		return true;
	}

	
	//Helper selection depends on Activity: WHEN SOME ACTIVITY CAN BE DONE BY SOME PARTICULAR HELPER ONLY..
	/**
	 * checks for pre conditions depending on the variable passed and then acts with the nearest helper
	 * Useful for user triggered State Transitions. Ex:- in case of upgrades
	 * @param checkPreConditions
	 * @return whether the helper was assigned or the transition started
	 */

	public bool CheckPreConditionsAndStartStateTransition(bool checkPreConditions,string activityName) {
		return CheckPreConditionsAndStartStateTransition (checkPreConditions, Activity.FindActivity (activityName));
	}

	public bool CheckPreConditionsAndStartStateTransition(bool checkPreConditions,ActivityName activityName) {
		return CheckPreConditionsAndStartStateTransition (checkPreConditions, Activity.FindActivity (activityName));
	}

	public bool CheckPreConditionsAndStartStateTransition(bool checkPreConditions, Activity activity) {
		if(activityTarget == null) {
			Debug.LogError("activityTarget not registered to activity controller");
			return false;
		}
		this.activity = activity;
		//TODO
		/*if(!checkPreConditionsOnly(checkPreConditions)) return false ; 
		//TODO
		//Because no helper is needed if the activity is auto or has zero duration
		if(this.userAsset.getNextActivity().isAuto() || this.getActivityDuration() == 0) {
			this.StartStateTransition(null);
			return true;
		}
		*/
		if (this.HelperRequired () && activityTarget.IsHelperRequired()) {
				//lets send the helper
				try {
					this.helper = CharacterManager.GetInstance().AssignNearestFreeHelper(activityTarget);
					MoveHelperToTarget(helper,false);
					return true;
				} catch (CharacterUnavailableException e) {
						Debug.LogWarning (e);
						PopupManager.GetInstance().ShowPanel (PanelType.HELPER_BUSY);
				}
		} else {
			activityTarget.StartActivity();
			//activityTarget.gameObject.GetComponent<TransitionController>().StartActivityTransition();
		}
		
		return false;
	}
	
	private void MoveHelperToTarget(CharacterGridObject helper, bool teleport) {
		//teleport = true;
		if(teleport || Config.HELPER_FORCE_TELEPORT){
			helper.TeleportTo(activityTarget,this);
		}else{
			//this.fade();
			//this.touchable = false;
			//if(activityStatus != null)
			//	activityStatus.touchable = false;
			//TODO : Pass the nearest free gridposition

			helper.MoveTo(activityTarget,this);
		}
	}

	public void OnActivityComplete() {
	 	Pool.Destroy(equipment);
		CharacterManager.GetInstance().FreeAssignedHelper(activityTarget);
	}

	/*
	 * callback from helper class
	 * 
	 */
	public void OnHelperReached() {
	
		EquipHelper();
		helper.StartActivity (activity);
		activityTarget.StartActivity();
		//activityTarget.gameObject.GetComponent<TransitionController>().StartActivityTransition();
	}

	//based on quipement name and bone assigned (they  come from acitivity table, or they may come from seperate Quipment table or helper table)
	private void EquipHelper() {
		Pool.Destroy(equipment);
		GameObject equimentPrefab = (GameObject) Resources.Load(activity.equipment, typeof(GameObject));
		if (equimentPrefab != null) 
			equipment = Pool.Instantiate( equimentPrefab, Vector3.zero, Quaternion.identity);
			//equipment = (GameObject)GameObject.Instantiate(equimentPrefab);

		Transform equipment_bone = Util.SearchHierarchyForBone(helper.transform, activity.characterBone);

		if( equipment != null && equipment_bone != null)
		{
			equipment.transform.position = equipment_bone.position;
			equipment.transform.rotation = equipment_bone.rotation;
			equipment.transform.parent = equipment_bone;
		}
	}

}
