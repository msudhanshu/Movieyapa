//
//  CombatCharacter.cs
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

public class CombatCharacter : RpgCharacter
{	
	/// <summary>
	/// /*part of CombatPlaceableActor class
	/// </summary>
	private float health = 0;
	/**
	 * Full health of the actor
	 */
	protected float fullHealth = -1;
	/**
	 * Health boost for full health
	 */
	public float fullHealthBoost = 0;
	public bool isImmune = false;//is immune to damage
	/**
	 * Gets the current health
	 * @return
	 */
	public float getHealth() {
		return health;
	}
	
	/**
	 * Sets the health
	 * @param health
	 */
	public void setHealth(float health) {
		if(health > this.getFullHealth()) {
			health = this.getFullHealth();
		}
		this.health = health;
	}
	
	/**
	 * Gets the full health of this actor
	 * @return
	 */
	public float getFullHealth() {
		return this.fullHealth + this.fullHealthBoost;
	}
	
	/**
	 * Sets the full health of this actor
	 * @param health
	 */
	public void setFullHealth(float health) {
		this.fullHealth = health;
	}
	
	/**
	 * Removes all the damage
	 */
	public void removeDamage() {
		this.setHealth(this.getFullHealth());
	}
	
	/**
	 * Tells whether actor has complete health or not.
	 */
	public bool isCompleteHealth() {
		return this.getHealth() >= this.getFullHealth();
	}
	
	/**
	 * Gets health needed to repair.
	 */
	protected float getRepairHealth() {
		return (this.getFullHealth() - this.health);
	}
	
	/**
	 * Checks if this actor is damaged i.e. health is negative
	 * @return
	 */
	public bool isDamaged() {
		return this.getHealth() <= 0;
	}

	//from MyPlaceableActor
	protected static float DEFAULT_HEALTH = 10;
	//////// MyPlaceableActor.java also has health and damage related function ???????

	/* ++++++++++ plus few damaging funtions ++++++++++++++++++++ */
	/// ///


	protected CombatPropertyProvider combatPropertyProvider;
	protected bool isProjectile = false;
	Dictionary<string, Powerup> powerUps = new Dictionary<string, Powerup>();

    protected override void InitView(Asset type) {
		base.InitView(type);
		InitProperties();
	}

	/** Initilise all combat related properties, equiped item, powerups, abilitys.. 
	 * from asset properties, based on state, and user asset*/

	protected virtual void InitProperties() {
		
		//check powerups
		//this.updatePowerupLevels();
		
		this.resetCombatArm();
		
		this.applyBoostBasedOnItem();
		
	//	this.applyInherentPassiveAbilities();
	}

	public static  byte DEFAULT_BULLETS_PER_FIRE = 1;
	public static  float DEFAULT_DPB = 1f;
	public static  float DEFAULT_FIRE_INTERVAL = 2f;
	/**
	 * Resets the combat arm
	 */
	protected void resetCombatArm() {
		float range = data.GetStateFloatProperty("range", 2f);
		
		this.combatPropertyProvider.SetProperty(CombatProperty.range, range);
		
		this.combatPropertyProvider.SetProperty(CombatProperty.dpb, this.combatPropertyProvider.GetProperty(CombatProperty.dpb,UserAssetPropertyName.DPB.Get(data, DEFAULT_DPB)));
		this.combatPropertyProvider.SetProperty(CombatProperty.bulletsperfire, 
		                                        this.combatPropertyProvider.GetProperty(CombatProperty.bulletsperfire ,data.GetStateIntProperty("bulletsperfire", DEFAULT_BULLETS_PER_FIRE)));
		
		this.combatPropertyProvider.SetProperty(CombatProperty.fireinterval, 
		                                        this.combatPropertyProvider.GetProperty(CombatProperty.fireinterval, data.GetStateFloatProperty("fireinterval", DEFAULT_FIRE_INTERVAL)));
		this.combatPropertyProvider.SetProperty(CombatProperty.bulletspeed, this.combatPropertyProvider.GetProperty(CombatProperty.bulletspeed, data.GetStateFloatProperty("bulletspeed", 1000f)));
		this.combatPropertyProvider.SetProperty(CombatProperty.castingtime, this.combatPropertyProvider.GetProperty(CombatProperty.castingtime, data.GetStateFloatProperty("castingtime", 0f)));
		
		this.isProjectile = this.data.asset.HasProperty("bulletgravity") || 	this.combatPropertyProvider.GetProperty(CombatProperty.bulletgravity, 0f) > 0;
		
		
		if(this.combatPropertyProvider.GetProperty(CombatProperty.bulletgravity, 0f) == 0) {
			this.combatPropertyProvider.SetProperty(CombatProperty.bulletgravity,this.data.asset.GetFloatProperty(("bulletgravity"), 0));
		}
	}


	
	/**
     * Apply boost based on item
     */
	public void applyBoostBasedOnItem() {
		if(this.isItemApplied()){
			float heroDpb = this.combatPropertyProvider.GetProperty(CombatProperty.dpb, 1f);
			//update the critical chance probability
			UserAsset itemUserAsset = this.getCurrentItemApplied().data;
			
			this.combatPropertyProvider.SetProperty(CombatProperty.dpb, heroDpb + getItemBoostDamage(itemUserAsset));

			// Update current Health
			float fullHealthHealth = UserAssetPropertyName.HEALTH.Get(data, this.data.asset.GetLastState(), this.data.level, DEFAULT_HEALTH);
			if (UserAssetPropertyName.HEALTHBOOST.Get(itemUserAsset,0f) > 0f)
				this.fullHealthBoost = (fullHealthHealth*UserAssetPropertyName.HEALTHBOOST.Get(itemUserAsset))/100;
			else
				this.fullHealthBoost = UserAssetPropertyName.HEALTH.Get(itemUserAsset,0f);
			
			// Setting health as full health as our hero will be of full health on our own base.
			this.setHealth(this.getFullHealth());
			
			
			string bdpasset = itemUserAsset.GetStateProperty("bdpasset");
			if (bdpasset != null && ! Utility.StringEquals(bdpasset.Trim(),"")) {
				string[] detString = bdpasset.Split(',');
				foreach (string param in detString) {
					string[] paramString = param.Split(':');
					//TODO : FIXME : TEMP
					//this.updateBonusDamage(AssetHelper.getAsset(paramString[0].trim()), Float.parseFloat(paramString[1].trim())/100);
				}
			}
	
			this.applyItemBasedPassiveAbilities();
		}
	}
	
	public float getItemBoostDamage(UserAsset itemUserAsset) {
		float itemBoostDamage = 0;
		if (UserAssetPropertyName.DPBPERCENTAGE.Get(itemUserAsset,0f) > 0f) {
			itemBoostDamage = ( UserAssetPropertyName.DPB.Get(data, 1f)*(UserAssetPropertyName.DPBPERCENTAGE.Get(itemUserAsset)))/100;
		}
		else
			itemBoostDamage = UserAssetPropertyName.DPB.Get(itemUserAsset);
		return itemBoostDamage;
	}
	
	public float getItemArmorBoost(UserAsset itemUserAsset) {
		float armorBoost = 0;
		if (UserAssetPropertyName.ARMORPERCENTAGE.Get(itemUserAsset,0f) > 0f)
			armorBoost = ( UserAssetPropertyName.ARMOR.Get(data, 0f)*UserAssetPropertyName.ARMORPERCENTAGE.Get(itemUserAsset))/100;
		else
			armorBoost = UserAssetPropertyName.ARMOR.Get(itemUserAsset,0f);
		return armorBoost;
	}
	/**
     * Apply all the abilities 
     */
	public void applyPassiveAbilities() {
		// inherent abilities
		applyInherentPassiveAbilities();
		// equipped from items
		applyItemBasedPassiveAbilities();
	}

	
	private void applyInherentPassiveAbilities() {
		string abilityDetail = this.data.GetStateProperty("appliedability");
		/*		if(this instanceof DefensiveCombatActor)
			abilityDetail = "slow_passive_sabble:1"; //For testing 
		if(this instanceof AttackingCombatActor)
			abilityDetail = "apbooster_passive:1"; //For testing 
*/
		if (abilityDetail != null) {
			string[] ability = abilityDetail.Split(',');
			foreach (string abString in ability) {
				string[] abilityi = abString.Split(':');
				if (!hasAbility(abilityi[0])) {
					Asset abilityAsset = DatabaseManager.GetAsset(abilityi[0]);
					int level = Convert.ToInt32(abilityi[1]);
					this.createAbilityWithActor(abilityAsset, level);
				} else {
					Powerup powerup = this.getAbility(abilityi[0]);
					powerup.preload();
				}
			}
		}
	}
	
	private void applyItemBasedPassiveAbilities() {
		if(this.isItemApplied()){
			string abilityDetail = this.getCurrentItemApplied().getPassiveAssetId();
			//abilityDetail = "fire_aura_passive:1";
			if (abilityDetail != null) {
				if (!hasAbility(abilityDetail)) {
					Asset abilityAsset = DatabaseManager.GetAsset(abilityDetail);
					int level = this.getCurrentItemApplied().getPassiveLevel();
					this.createAbilityWithActor(abilityAsset, level);
				}
				else {
					Powerup powerup = this.getAbility(abilityDetail);
					powerup.preload();
				}
			}
        }
    }
    
    public bool hasAbility(string assetId) {
        return powerUps[assetId] != null;
    }
    
    public Powerup getAbility(string assetId) {
        return powerUps[assetId];
    }
    
    private bool removeAbility(string assetId) {
        if (powerUps[assetId] != null) {
            powerUps[assetId].deactivate();
            return true;
        }
        return false;
	}

	public void removeAppliedItemStats(){
		if(this.isItemApplied()){
			this.combatPropertyProvider.SetProperty(CombatProperty.dpb,data.GetStateFloatProperty("dpb", 1f));
			this.fullHealthBoost = 0f; 

			string abilityDetail = this.getCurrentItemApplied().getPassiveAssetId();
			if (abilityDetail != null) {
				string[] ability = abilityDetail.Split(':');
				this.removeAbility(ability[0]);
			}
		}
		
	}
	
	private Item itemApplied = null;
	
	public bool isItemApplied() {
		return this.itemApplied != null;
	}
	
	public void applyItem(Item item) {
		Item existingItem = this.getCurrentItemApplied();
		if(existingItem != null && existingItem.Equals(item)) return;
		this.removeAppliedItemStats();
		this.setCurrentItemApplied(item);
		// Boost can be applied when actor is equiped with item.
		if(item != null) {
			this.applyBoostBasedOnItem();
		}
		//this.checkAndDownloadMissingImagesForItems(item);
		//this.resetAssets();
		//this.loadAssets();
	}
	
	public Item getCurrentItemApplied() {
		return itemApplied;
	}
	
	private void setCurrentItemApplied(Item actor){
		this.itemApplied = actor;
		if(actor == null) 
			this.data.setProperty(Config.CURRENT_BOOST_ITEM, "");
		else 
			this.data.setProperty(Config.CURRENT_BOOST_ITEM, ""+actor.data.id);                
		this.data.saveProperties();
	}

	private Powerup createAbilityWithActor(Asset abilityAsset, int level) {
		Powerup powerup = Powerup.create(this, abilityAsset, level);
		powerup.preload();
		return powerup;
	}
	
	public static Powerup createAndPreloadAbility(Asset abilityAsset, int level) {
		// No need to send parent actor for active abilities as they are already associated with one actor
		Powerup powerup = Powerup.create(null, abilityAsset, level);
        powerup.preload();
        return powerup;
    }

}