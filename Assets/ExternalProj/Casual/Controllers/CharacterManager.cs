using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
/**
 * Manages characters
 */
public class CharacterManager : Manager<CharacterManager> {
	
	//fx to be played on buying new character
	public GameObject CharacterCreationFX ;
	/**
	 * Prefab to use when creating Character.
	 */ 
	public GameObject characterPrefab;
	public GameObject rpgCharacterPrefab;
	
	//list of all characters
	List<CharacterGridObject> allCharacters;
	public Dictionary<CharacterGridObject, IActivityTarget> assignedCharacters;
	
	override public void StartInit ()
	{
		allCharacters = new List<CharacterGridObject>();
		assignedCharacters = new Dictionary<CharacterGridObject, IActivityTarget>();
		//LoadUserCharacters();
		initialised = true;
	}
	
	override public void PopulateDependencies(){}
	
	public void LoadUserCharacters() {
		if (PersistenceManager.GetInstance() != null) {
			List<UserAnimalHelper> userAnimalHelpers = null;
			if(ServerConfig.SERVER_ENABLED){
				userAnimalHelpers = DataHandler.wrapper.userAnimalHelpers;
				foreach(UserAnimalHelper userAnimalHelper in userAnimalHelpers){
					CharacterManager.GetInstance().CreateAndLoadCharacter(userAnimalHelper);
				}
			}
			if (PersistenceManager.GetInstance().SaveGameExists() && !ServerConfig.SERVER_ENABLED) {
				SaveGameData savedGame = PersistenceManager.GetInstance().Load();
				if(!ServerConfig.SERVER_ENABLED){
					userAnimalHelpers = savedGame.userAnimalHelpers;
					foreach(UserAnimalHelper userAnimalHelper in userAnimalHelpers){
						CharacterManager.GetInstance().CreateAndLoadCharacter(userAnimalHelper);
					}
				}
				return;
			}
		}
	}
	
	public void addCharacterToList(CharacterGridObject character){
		allCharacters.Add(character);
	}
	
	public void removeCharacterFromList(CharacterGridObject character){
		allCharacters.Remove(character);
	}
	
	// load the character from user data
	public void CreateAndLoadCharacter(IUserAsset userAsset) {
		GameObject go;
		UserAssetController character;
		if(userAsset.asset.IsRpgCharacter()) {
			go = Util.InstantiatePrefab(userAsset.asset.assetCategory.PrefabName());
			//go = (GameObject) Instantiate(rpgCharacterPrefab);
			go.transform.parent = GameManager.GetInstance().gameView.transform;
			character = go.GetComponentInChildren<RpgCharacter>();
		} else{
			go = Util.InstantiatePrefab(userAsset.asset.assetCategory.PrefabName());
			go.transform.parent = GameManager.GetInstance().gameView.transform;
			character = go.GetComponentInChildren<Character>();
		}
		
		//FIXME: SetRandomPosition or Assigning it to Building should happen later after both are done loading.
		GridPosition charGridPosition = BuildingModeGrid3D.GetInstance ().getRandomFreeGridPosition (BuildingManager3D.GetInstance ().types [userAsset.assetId].shape.ToArray ());
		//go.SendMessage ("SetPosition", charGridPosition);
		//go.SendMessage ("SetHeight", BuildingModeGrid3D.GetInstance().GetTerrainHeightAtGridPosition(charGridPosition));
		userAsset.position = charGridPosition;
		userAsset.height = BuildingModeGrid3D.GetInstance ().GetTerrainHeightAtPosition (charGridPosition);
		
		character.Init(BuildingManager3D.GetInstance().GetBuildingTypeData(userAsset.assetId), userAsset);
		addCharacterToList (go.GetComponent<CharacterGridObject> ());
		//BuildingManager3D.GetInstance().AddBuilding(building);
		//building.Acknowledge();
	}
	
	//create/buy new character
	public void CreateCharacter(string assetId) {
		if (BuildingManager3D.GetInstance().CanBuildBuilding(assetId) && ResourceManager.GetInstance().CanBuild(BuildingManager3D.GetInstance().GetBuildingTypeData(assetId))) {
			GameObject go;
			UserAssetController character;
			if(BuildingManager3D.GetInstance().types[assetId].IsRpgCharacter()) {
				go = (GameObject) Instantiate(rpgCharacterPrefab);
				go.transform.parent = GameManager.GetInstance().gameView.transform;
				character = go.GetComponentInChildren<RpgCharacter>();
			}else {
				go = (GameObject) Instantiate(characterPrefab);
				go.transform.parent = GameManager.GetInstance().gameView.transform;
				character = go.GetComponentInChildren<Character>();
			}
			
			//	building.Init(BuildingManager3D.GetInstance().types[assetId],
			//	              BuildingModeGrid3D.GetInstance().getRandomFreeGridPosition(BuildingManager3D.GetInstance().types[assetId].shape.ToArray()));
			GridPosition charGridPosition  =   BuildingModeGrid3D.GetInstance().getRandomFreeGridPosition(BuildingManager3D.GetInstance().types[assetId].shape.ToArray());
			character.Init(BuildingManager3D.GetInstance().types[assetId],
			               charGridPosition, 
			               BuildingModeGrid3D.GetInstance().GetTerrainHeightAtPosition(charGridPosition) );
			
			
			StartCoroutine(PlayCreateCharacterEffect(character.transform));
			ResourceManager.GetInstance().RemoveSilver(character.asset.cost);

			GameEventTask.notifyAction("buy_"+assetId);
			if ((int)BuildingManager3D.GetInstance().saveMode < (int) SaveMode.SAVE_MOSTLY) PersistenceManager.GetInstance().Save();

			addCharacterToList (go.GetComponent<CharacterGridObject> ());

			if(BuildingManager3D.GetInstance().types[assetId].IsRpgCharacter()) 
				ServerAction.takeAction(EAction.ADD, (character as RpgCharacter).data, ServerSyncManager.GetInstance().serverNotifier);
			else
				ServerAction.takeAction(EAction.HELPER_PURCHASE, character);

			//BuildingManager3D.GetInstance().AddBuilding(character);
			//building.Acknowledge();
		} else {
			if (BuildingManager3D.GetInstance().CanBuildBuilding (assetId)) {
				Debug.LogWarning("This is where you bring up your in app purchase screen");
			} else {
				Debug.LogError("Tried to build unbuildable building");
			}
		}
	}

	private IEnumerator PlayCreateCharacterEffect(Transform trans) {
		//	Transform t = Pool.Instantiate(AvatarEffect,pos.position,Quaternion.identity);
		GameObject t = (GameObject)Instantiate(CharacterCreationFX,trans.position,Quaternion.identity);
		t.transform.parent = trans;
		yield return new WaitForSeconds (2);
		//Pool.Destroy(t);
		Destroy (t);
	}
	
	/*	public bool SellCharacter(Building building, bool fullRefund = false){
		if (BuildingManager3D.GetInstance().buildings.ContainsKey (building.uid)) BuildingManager3D.GetInstance().buildings.Remove (building.uid);
		ResourceManager.GetInstance().AddResources (fullRefund ? building.Type.cost : (int)(building.Type.cost * BuildingManager3D.RECLAIM_PERCENTAGE));
		if (building.Type.additionalCosts != null) {
			foreach (CustomResource cost in building.Type.additionalCosts) {
				ResourceManager.GetInstance().AddCustomResource (cost.id, fullRefund ? cost.amount : (int)(cost.amount * BuildingManager3D.RECLAIM_PERCENTAGE));
			}
		}
		if (building.State != BuildingState.PLACING && building.State != BuildingState.PLACING_INVALID )
			BuildingModeGrid.GetInstance().RemoveObject (building);
		if (BuildingManager3D.ActiveBuilding == building) BuildingManager3D.ActiveBuilding = null;
		removeCharacterFromList(building.gameObject.GetComponent<CharacterGridObject>());
		Destroy (building.gameObject);
		if ((int)BuildingManager3D.GetInstance().saveMode < (int) SaveMode.SAVE_NEVER) PersistenceManager.GetInstance().Save();
		return true;
	}*/
	
	/**
	 * Assigns the helper to the given actor.
	 * @param helperActor
	 * @param activityTarget
	 */
	public void AssignHelper(CharacterGridObject helperActor, IActivityTarget activityTarget) {
		assignedCharacters.Add(helperActor, activityTarget);
		helperActor.lastTargetActor = activityTarget;
	}
	
	/**
	 * Gets the nearest free helper
	 * @param targetTile
	 * @return
	 */
	public CharacterGridObject GetNearestFreeHelper(GridPosition targetTile) {
		CharacterGridObject nearestFreeHelper = null;
		float curDistance = 0f;
		float minDistance = -1f;
		if(allCharacters != null) {
			foreach (CharacterGridObject helper in allCharacters) {
				if(!helper.isFree) continue;
				if (assignedCharacters.ContainsKey(helper) ) continue;//this helper is not free
				if(helper.GetBasePrimaryTile() != null && targetTile != null) {
					curDistance = targetTile.SqrDistanceFrom(helper.GetBasePrimaryTile().Position);
					
					//Debug.Log(helper.characterAIPath.ToString());
					//Debug.Log ( helper.GetBasePrimaryTile().Position.x + "," + helper.GetBasePrimaryTile().Position.y + ":" +curDistance);
					
					if (minDistance < 0 || curDistance < minDistance) {
						nearestFreeHelper = helper;
						minDistance = curDistance;
					}
				} else {
					//either the helper's base primary tile is null or the target tile is null, return the first free helper
					nearestFreeHelper = helper;
					break;
				}
			}
		}
		return nearestFreeHelper;
	}
	
	public CharacterGridObject GetFreeHelper(){
		foreach (CharacterGridObject helper in allCharacters) {
			if (assignedCharacters.ContainsKey(helper)) 
				continue;//this helper is not free
			return helper;
		}
		return null;	
	}
	
	
	
	public List<CharacterGridObject> GetAllCharacter() {
		return allCharacters ;
	}
	
	/**
	 * Gets the helper actor assigned to the given actor.
	 * @param actor
	 * @return
	 */
	public CharacterGridObject GetAssignedHelper(IActivityTarget activityTarget) {
		foreach (KeyValuePair<CharacterGridObject, IActivityTarget> entry in assignedCharacters) {
			CharacterGridObject helper = entry.Key;
			IActivityTarget pActor = entry.Value;
			if (pActor == activityTarget)
				return helper;
		}
		return null;
	}
	
	/**
	 * Gets the nearest free helper walk to the base primary tile of the given actor. Blocks the helper
	 * @param actor
	 * @return
	 * @throws HelperUnavailableException
	 */
	public CharacterGridObject AssignNearestFreeHelper(IActivityTarget activityTarget) {
		//check if the actor has already been assigned a helper
		CharacterGridObject assignedHelper = GetAssignedHelper(activityTarget);
		if(assignedHelper == null) {//if null, get the nearest free helper and initialize the act
			assignedHelper = GetNearestFreeHelper(activityTarget.targetGridPosition);
			if(assignedHelper != null) {
				AssignHelper(assignedHelper, activityTarget);
			} else
				throw new CharacterUnavailableException();
		}
		return assignedHelper;
	}
	
	/**
	 * Callback to free the helper assigned to the given actor. 
	 * @param actor
	 */
	public void FreeAssignedHelper(IActivityTarget activityTarget) {
		CharacterGridObject assignedHelper = GetAssignedHelper(activityTarget);
		if(assignedHelper != null)
			FreeHelper(assignedHelper);
	}

	//FIXME : MANJEET
	public void FreeAssignedHelper(Building building) {
		CharacterGridObject assignedHelper = GetAssignedHelper(building.transitionController);
		if(assignedHelper != null)
			FreeHelper(assignedHelper);
	}
	
	/**
	 * @param helperActor
	 */
	private void FreeHelper(CharacterGridObject helperActor) {
		assignedCharacters.Remove(helperActor);
		
		//FIXME : remove this character anim control from here
		helperActor.SetToIdle();
		helperActor.lastTargetActor = null;
		//TODO :
		/* 
		helperActor.lastTargetActor = null;
		//		helperActor.getUserAnimalHelper().userAssetId = -1;
		helperActor.asyncSetState(ActionActorState.IDLE);
		//free the helper
		helperActor.getUserAnimalHelper().free();
		//no need to send event to server as helper start act has also been removed
		//		ServerApi.takeAction(ServerAction.HELPER_FINISH_ACT, helperActor, (activityTarget)null, null, true);
		*/
	}

	public bool GridPositionHasCharacter(GridPosition pos) {
		foreach (CharacterGridObject character in GetAllCharacter()) {
			if (character.IsGridPositionOccupied(pos) )
				return true;
		}
		return false;
	}
	
}


public class CharacterUnavailableException : Exception {
	public override string Message {
		get {
			return base.Message;
		}
	}
}

