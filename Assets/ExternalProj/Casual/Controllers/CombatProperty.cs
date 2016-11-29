//
//  CombatProperty.cs
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
using System;


/**
	 * Combat Properties
	 * Assumption - All daproperties must be floats and no string 
	 *
	 */
// Do not include "misc" in this enum as it might contain string
public enum CombatProperty {
	bulletsperfire,
	fireinterval,
	bulletspeed,
	range,
	dpb,
	dpbpercentage,
	healthboost,
	sdradius,
	sddrfraction,
	sdtotaldpb,
	bulletgravity,
	criticalchanceprobability,
	castingtime, 
	rebounddamage,
	duration,
	angle,	
	aoewidth,
	aoelength,
	slowduration,
	slowpercent
}

static class CombatPropertyExtension
{
	public static String GetName(this CombatProperty s1)
	{
		return s1.ToString();
	}
	public static float Get(this CombatProperty s1, Asset asset)
	{
		return asset.GetFloatProperty(s1.GetName(), 0f);
    }
	public static float Get(this CombatProperty s1, Asset asset, float defaultValue)
	{
		return asset.GetFloatProperty(s1.GetName(), 0f);
    }
	public static int Ordinal(this CombatProperty s1)
	{
		return (int)s1;
	}
}

/**
	 * CombatPropertyProvider
	 * @author vinayks
	 *
	 */
public class CombatPropertyProvider {
	
	private float[] floats;
	
	private bool[] isSet;
	
	/**
		 * Constructor
		 */
	public CombatPropertyProvider() {
		/* GetNames is 10X faster than GetValues*/
		int CombatPropertyEnumLength  = Enum.GetNames(typeof(CombatProperty)).Length;
		floats = new float[CombatPropertyEnumLength];
		isSet = new bool[CombatPropertyEnumLength];
	}
	
	/**
		 * Sets an bool property
		 * @param combatProperty
		 * @param value
		 */
	public void SetProperty(CombatProperty combatProperty, bool value) {
		int index = combatProperty.Ordinal();
		floats[index] = (value ? 1 : 0);
		isSet[index] = true;
	}
	
	/**
		 * Sets a float property
		 * @param combatProperty
		 * @param value
		 */
	public void SetProperty(CombatProperty combatProperty, float value) {
		int index = combatProperty.Ordinal();
		floats[index] = value;
		isSet[index] = true;
	}
	
	/**
		 * Gets a float property
		 * @param combatProperty
		 * @param defaultValue
		 * @return
		 */
	public float GetProperty(CombatProperty combatProperty, float defaultValue) {
		int index = combatProperty.Ordinal();
		if(isSet[index]) {
			return floats[index];
		}
		
		return defaultValue;
	}
	
	/**
		 * Gets an int property
		 * @param combatProperty
		 * @param defaultValue
		 * @return
		 */
	public int GetProperty(CombatProperty combatProperty, int defaultValue) {
		int index = combatProperty.Ordinal();
		if(isSet[index]) {
			return (int)floats[index];
		}
		
		return defaultValue;
	}
	
	/**
		 * Gets a bool property
		 * @param combatProperty
		 * @param defaultValue
		 * @return
		 */
	public bool GetProperty(CombatProperty combatProperty, bool defaultValue) {
		int index = combatProperty.Ordinal();
		if(isSet[index]) {
			return floats[index] > 0;
		}
		
		return defaultValue;
	}
	
	/**
		 * Has property
		 * @param combatProperty
		 * @return
		 */
	public bool hasProperty(CombatProperty combatProperty) {
		int index = combatProperty.Ordinal();
		return isSet[index];
    }
    
    
    public void reset() {
		foreach (CombatProperty combatProperty in Enum.GetValues(typeof(CombatProperty)) ) {
            int index = combatProperty.Ordinal();
            isSet[index] = false;
            floats[index] = 0;
        }
    }
    
}


//Use this enum to save/truncate temporary combat property and actionsmap 
public enum TempCombatProperty{
	INVINCIBLE,
	REVIVAL,
	STUN,
	DAMAGE_STEAL,
	EXTRA_DAMAGE,
	POISON,
	IS_BOSS,
	THRESHOLD_DAMAGE,
	POWERUP_EXPIRED,
	QUEST_NOTIFIED,
	IS_PRIMARY_TARGET
}

static class TempCombatPropertyExtension
{
	public static String GetString(this TempCombatProperty c){
		return Utility.StringToLower("temp_"+c.ToString());
	//	return Utility.toLowerCase("temp_"+this.toString());
	}
}