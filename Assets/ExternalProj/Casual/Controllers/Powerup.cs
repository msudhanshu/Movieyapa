//
//  Powerup.cs
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

public class Powerup 
{	
	public void deactivate() {
	}
	public void preload() {

	}
	public static Powerup create(CombatCharacter parentCombatActor, Asset powerupAsset, int level) {
		return null;
	}
}