//
//  Item.cs
//
//  Author:
//       Manjeet <msudhanshu@kiwiup.com>
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/** Rpg Actor who does combat, doing damage,taking damage and lossing health.
 * It equips with items.
 * Has powerups/spells/abilities.
 */

public class Item //: IReward
{	
	public UserAsset userAsset;
	public UserAsset data {
		get {
			return userAsset;
		}
	}
	virtual public long userAssetId {
		get { return userAsset.id; }
		set { userAsset.id = value; }
	}
	
	protected Asset _asset;
	
	/**
	 * The data defining the type of this building.
	 */ 
	virtual public Asset asset {
		get { return _asset; }
		protected set {
			_asset = value;
			userAsset.asset = value;
            userAsset.assetId = value.id;
        }
    }
    
	public string getPassiveAssetId() {
		return "";
	}
	public int getPassiveLevel() {
		return 1;
	}
}