//
//  IUserAsset.cs
//
//  Author:
//       Manjeet <msudhanshu@kiwiup.com>
//
using UnityEngine;
using System.Collections;

public interface IUserAsset
{
	long id {get; set;}
	string assetId {get;set;}
	Asset asset {get;set;}
	AssetState assetState {get;set;}
	GridPosition position { get;set;}
	GridHeight height { get; set;}
	int level {get; set;}
	long stateStartTime {get; set;}
	long activityStartTime {get; set;}
	string GetViewPrefabName();
}

