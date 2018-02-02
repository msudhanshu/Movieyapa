using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
/**
 * Manages characters
 */
public class MovieGameManager : Manager<MovieGameManager> {

	/*
	 * Parent Transform/gameObject which hold the whole playable arena with grid,building,characters. 
	 */
	public GameObject gameView;
	
	/**
	 * This is the main perspective camera used in the game.
	 */ 
	public Camera gameCamera; 


	override public void StartInit ()
	{
		Debug.Log ("Movie game manager started");
		CheckNInitFilmTypeChooserScreen ();
		//InitHomeMainScreen ();
		CapitalManager.GetInstance ().AddCurrency (CurrencyModel.CURRENCY_HINT ,21);
	}

	public virtual void Start() {
		//PlayerPrefs.DeleteAll ();
		if(gameCamera==null)
			gameCamera = Camera.main.GetComponent<Camera>();
	}

	override public void PopulateDependencies(){
		dependencies = new List<ManagerDependency>();
		dependencies.Add(ManagerDependency.SG_GAME_INIT);
	}

	//FIXME : It should go to some utils monobehavior class
	public void DestroyOnTimeOut(GameObject baseObject,float timeout) {
		StartCoroutine(DestroyOnTimeOutCoroutine(baseObject,timeout) );
	}

	private IEnumerator DestroyOnTimeOutCoroutine(GameObject baseObject,float timeout) {
		float elapsedTime = 0;
		while(elapsedTime<timeout) {
			elapsedTime += Time.deltaTime;
			//Debug.Log("timeout "+timeout +", elapsedTime "+elapsedTime);
			yield return new WaitForEndOfFrame();
		}
		Pool.Destroy(baseObject);
	}

	public void FilmTypeChoosen(string film) {
		//MANJEETODO ; FOR SAFE SIDE 
		FilmIndustryType.SetActiveFilmType(FilmTypeEnum.BOLLYWOOD);
		InitHomeMainScreen ();
	}

	public void CheckNInitFilmTypeChooserScreen() {
		if (!FilmIndustryType.IsFilmTypeSet ()) {
			//set the type or show a selector screen
			PopUpManager.GetInstance().ShowPanel (PopUpType.FILM_SELECTOR_POPUP);
		}
		else {
			InitHomeMainScreen ();
		}
	}

	public void InitHomeMainScreen() {
		PopUpManager.GetInstance().ShowPanel (PopUpType.HOME_MAIN_POPUP);
		if (UIGamePopUp.activePanel is UIHomeMainPopup)
			((UIHomeMainPopup)UIGamePopUp.activePanel).Initialise();
	}

    public void CarrierStart() {
		Debug.Log ("Carrier start clicked");
//		SgResourceManager.GetInstance ().AddResource (DbResource.RESOURCE_GOLD,20);
//		SgResourceManager.GetInstance ().AddResource (DbResource.RESOURCE_XP,20);
        CarrierManager.GetInstance().StartCarrier();
    }

    public void PackageStart() {
        PackageManager.GetInstance().showPackages();
    }

    public void MiniGameStart() {
        
    }

}