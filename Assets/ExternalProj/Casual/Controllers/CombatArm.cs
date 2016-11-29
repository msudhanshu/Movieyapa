//
//  CombatArm.cs
//
//  Author:
//       Manjeet <msudhanshu@kiwiup.com>
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent (typeof(CombatCharacter))]
public class CombatArm : MonoBehaviour
{	
	/**
	 * Boolean to check whether the arm is firing
	 */
	protected bool isFiring = false;
	
	/**
	 * Owner of the arm
	 */
	protected CombatCharacter owner;
	
	/**
	 * Current target of the arm
	 */
//	public MyPlaceableActor currentTarget;
	
	/**
	 * number of bullets per fire
	 */
	public int bulletsPerFire;
	
	/**
	 * Damage per bullet
	 */
	public float dpb;
	
	/**
	 * Bonus Damage (varies depending on the target)
	 */
	private float bonusDamage = 0f;
	
	/**
	 * Extra bonus damage
	 */
	public float extraBonusDamage = 0f;
	
	public float passiveAbilityDamageModifier = 0f;
	
	public float activeAbilityDamageModifier = 0f;
	
	public float getExtraBonusDamage() {
		return extraBonusDamage;
	}
	
	public void setExtraBonusDamage(float extraBonusDamage) {
		this.extraBonusDamage = extraBonusDamage;
	}
	
	/*
	 * Boost in the dpb based on probability  
	 * */
	public float critBoostFraction = 0f;
	
	public float currentCritDamage = 0f;
	
	public int criticalChanceProbability = 0;
	
	/**
	 * Armor for damage
	 */
	public float armor = 0f;
	
	/**
	 * Current position of the owner
	 */
	protected Vector2 currentPosition = new Vector2();
	
	/**
	 * Current position of the target
	 */
	protected Vector2 targetPosition = new Vector2();
	
	private const float DEFAULT_SPEED = 200;
	
	/**
	 * Speed of the bullet
	 */
	protected float bulletSpeed = DEFAULT_SPEED;
	
	/**
	 * Time interval between two firing batches
	 */
	protected float fireInterval = 1;//1 sec
	
	/**
	 * Height from ground
	 */
	protected int elevation = 0;
	
	/**
	 * Explosion effect
	 */
//	protected IPooledEffect explosionEffect;
	
	/**
	 * Bullet asset
	 */
//	protected TextureAsset bulletAsset;
	
	/**
	 * Splash Damage Handler
	 */
//	public ISplashDamageHandler splashDamageHandler;
	
	protected CombatPropertyProvider combatPropertyProvider;
}