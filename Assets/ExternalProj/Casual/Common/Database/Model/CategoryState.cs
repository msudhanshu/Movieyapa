using UnityEngine;
using System.Collections;
using KiwiCommonDatabase;
using SimpleSQL;

[System.Serializable]
public class CategoryState : BaseDbModel {

	[PrimaryKey]
	public string categoryStateId {get; set;}
	public string assetCategoryId {get; set;}
	public string stateId {get; set;}
	public string name {get; set;}
	public string nextId {get; set;}
	public string activityId {get; set;}
}
