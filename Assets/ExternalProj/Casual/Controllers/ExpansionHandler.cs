using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Handles expansion
 * 1. Randomly places assets in the forest from the epic debris category
 * 2. Has a prioritized queue which can be used to add assets which can be found in the forest after a specified number of expansions. 
 */
public class ExpansionHandler
{
	private static List<Asset> expansionAssets;
	private static MaxRangeProbabilityModel maxRangeModel;
	private static DistributedProbabilityModel distributedModel;
	public static List<float> autoGenerationProbability;
	
	private class ExpansionData {
		public AssetState assetState { set; get;}
		public int quantity { set; get;}
		public int expansionIndex { set; get;}
		
		public void Decrement(int quantity) {
			this.quantity = this.quantity - quantity;
			if(this.quantity <= 0) {
				//TODO: Might have to remove the particular asset state and not the last element
				expansionAssetPriorityQueue.Dequeue();
				assetStateExpansionDataMap.Remove(this.assetState);
			}
		}
		
	}
	
	private static Queue<ExpansionData> expansionAssetPriorityQueue = new Queue<ExpansionData>();
	private static Dictionary<AssetState, ExpansionData> assetStateExpansionDataMap = new Dictionary<AssetState, ExpansionData>();
	
	private static string CURRENT_EXPANSION_INDEX = "expansionCount";
	private static string LAST_ASSET_EXPANSION_INDEX = "lastAssetExpansionIndex";
	
	private static int[] modelParams;
	
	/**
     * Deprioritizes the given asset state expansion by the given quantity
     * @param assetState
     * @param quantity
     */
	public static void Deprioritize(AssetState assetState, int quantity) {
		ExpansionData eData = assetStateExpansionDataMap[assetState];
		if(eData != null)
			eData.Decrement(quantity);
	}
	
	/**
     * Adds the given asset state to the priority queue. The user gets this asset state after the given number of expansions.
     * @param assetState
     * @param quantity : number of assets to be found in the state
     * @param expansionIndex : number of expansions after which this asset will be found.
     */
	public static void Prioritize(AssetState assetState,
	                              int quantity,
	                              int expansionIndex) {
		//safe case
		if(assetStateExpansionDataMap.ContainsKey(assetState) || quantity <= 0) return;
		var enumerator = expansionAssetPriorityQueue.GetEnumerator ();
		while(enumerator.MoveNext()){
			ExpansionData data = enumerator.Current;
			if(data.assetState == assetState)
				return;
		}
		
		ExpansionData eData = new ExpansionData();
		eData.assetState = assetState;
		eData.quantity = quantity;
		eData.expansionIndex = expansionIndex;
		
		expansionAssetPriorityQueue.Enqueue(eData);
		assetStateExpansionDataMap.Add(assetState, eData);
	}
	
	/**
     * Gets the last expansion index in which an asset was found
     * @return
     */
	private static int GetLastAssetExpansionIndex() {
		int lastAssetExpansionIndex = PlayerPrefs.GetInt(LAST_ASSET_EXPANSION_INDEX, -1);
		return lastAssetExpansionIndex;
	}
	
	/**
     * Sets the last expansion index in which an asset was found
     * @param lastAssetExpansionIndex
     */
	private static void SetLastAssetExpansionIndex(int lastAssetExpansionIndex) {
		PlayerPrefs.SetInt(LAST_ASSET_EXPANSION_INDEX, lastAssetExpansionIndex);
	}
	
	/**
     * Gets the current expansion index.
     * @return
     */
	private static int GetCurrentExpansionIndex() {
		int currentExpansionIndex = PlayerPrefs.GetInt(CURRENT_EXPANSION_INDEX, -1);
		return currentExpansionIndex;
	}
	
	/**
     * incremens the current expansion index.
     * @return
     */
	private static int IncrementCurrentExpansionIndex() {
		int currentExpansionIndex = GetCurrentExpansionIndex();
		PlayerPrefs.SetInt(CURRENT_EXPANSION_INDEX, currentExpansionIndex + 1);
		return currentExpansionIndex + 1;
	}
	
	private static bool epicDepriDiscovered = false;
	
	/**
     * Callback after an expansion happens. 
     * @param tileActor
     * @return true if an actor was added
     */
	public static bool AfterExpansion(GridPosition buildingGridPosition) {
		int currentExpansionIndex = IncrementCurrentExpansionIndex();
	
		BuildingData buildingData = null;
		//check if the priority queue has an asset state.
		if(expansionAssetPriorityQueue.Count > 0) {
			ExpansionData eData = expansionAssetPriorityQueue.Peek();
			int expansionsDoneSinceLastAsset = GetCurrentExpansionIndex() - GetLastAssetExpansionIndex();
			if(expansionsDoneSinceLastAsset >= eData.expansionIndex){
				buildingData = new BuildingData();
				buildingData.id = DataHandler.wrapper.nextUserAssetId;
				buildingData.assetStateId = eData.assetState.id;
				buildingData.xpos = buildingGridPosition.x;
				buildingData.ypos = buildingGridPosition.y;
				buildingData.stateStartTime = Utility.GetServerTime();

				BuildingManager3D.GetInstance().CreateAndLoadBuilding(buildingData);
			}
		}
		if(buildingData != null) {
			Dictionary<string, string> extraParams = new Dictionary<string, string>();
			extraParams.Add("expansionindex", GetCurrentExpansionIndex() - GetLastAssetExpansionIndex() + "");
			extraParams.Add("source", "expansion");
			//call the server api
			ServerAction.takeAction(EAction.ADD, buildingData, ServerSyncManager.GetInstance().serverNotifier, null, false);

			//notify the quest about the new asset
			UserQuestTask.notifyAction(ActivityTaskType.EXPANSION, buildingData.assetState.asset, buildingData.assetState);
			
			//increment last expansion count only if the actor is not an epic debri
			if(!epicDepriDiscovered)
				SetLastAssetExpansionIndex(currentExpansionIndex);
			else
				epicDepriDiscovered = false;
			return true;
		}
		
		return false;
	}
	
	/**
     * Gets the next random expansion asset from the epic debris category.
     * @return
     */
	private static Asset GetNextExpansionAsset() {
		Asset asset = null;
		if(expansionAssets == null || expansionAssets.Count == 0){
			expansionAssets = new List<Asset>();
			/* expansionAssets = new ArrayList<Asset>(AssetCategory.getAllAssets(AssetCategoryName.EPICDEBRIS));
            
            List<Asset> notSupported = new ArrayList<Asset>();
            //Get location specific expansion assets
            for(Asset expansionAsset:expansionAssets){
                if(!Config.CURRENT_LOCATION.isSupported(expansionAsset))
                    notSupported.add(expansionAsset);
            }
            expansionAssets.removeAll(notSupported);
            
            autoGenerationProbability = AssetHelper.getAssetGenerationProbability(expansionAssets);
            distributedModel = new DistributedProbabilityModel(autoGenerationProbability, true);*/
		}
		int nextIndex = distributedModel.getNextIndex();
		
		if(expansionAssets.Count > 0){
			if(nextIndex!=-1)
				asset = expansionAssets[nextIndex];
			else 
				asset = expansionAssets[0];
		}
		return asset;
	}
	
	public static void DisposeOnFinish(){
		expansionAssetPriorityQueue.Clear();
		assetStateExpansionDataMap.Clear();
		if(expansionAssets != null)
			expansionAssets.Clear();
		if(autoGenerationProbability != null)
			autoGenerationProbability.Clear();
		maxRangeModel = null;
		modelParams = null;
		distributedModel = null;
	}    
	
}