//
//  Character.cs
//
//  Author:
//       Manjeet <msudhanshu@kiwiup.com>
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Represents a building in the game. 
 */
public class Character : UserAssetController, IGridObject
{
	protected override void CreateEmptyUserAsset() {
		userAsset = new UserAnimalHelper();
	}

}

