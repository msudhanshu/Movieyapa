using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
/**
 * Manages characters
 */
public class SgGameManager : Manager<SgGameManager> {

	public BaseConfig config {
		get {
			return SgFirebase.GetInstance ().config;
		}
	}

	public SgFirebaseDataInitialiser  wrapper{
		get {
			return SgFirebase.GetInstance ().dataInitialiser;
		}
	}

	public BaseRestAPI api {
		get {
			return SgFirebase.GetInstance ().api;
		}
	}

	public string userId {
		get {
			return SgFirebase.GetInstance ().auth.userId;
		}
	}
		
	public delegate void GameInitAction();
	public static event GameInitAction OnGameInited;

	override public void StartInit ()
	{
		Finished ();
	}

	override protected void Finished() {
		Debug.Log("Game Init Finished");
		if(OnGameInited != null)
			OnGameInited();
		transform.parent.gameObject.BroadcastMessage("DependencyCompleted", ManagerDependency.SG_GAME_INIT);
	}

	override public void PopulateDependencies(){
		dependencies = new List<ManagerDependency>();
		dependencies.Add(ManagerDependency.FIREBASE);
		dependencies.Add(ManagerDependency.CURRENCY_INIT);
	}
}