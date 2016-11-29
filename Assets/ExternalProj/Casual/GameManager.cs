using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
/**
 * Manages characters
 */
public class GameManager : Manager<GameManager> {

	public static int TERRAIN_LAYER = 11;
	/*
	 * Parent Transform/gameObject which hold the whole playable arena with grid,building,characters. 
	 */
	public GameObject gameView;
	
	/**
	 * This is the main perspective camera used in the game.
	 */ 
	public Camera gameCamera; 

	public Terrain terrain; 

	override public void StartInit ()
	{
	}

	public virtual void Start() {
		if(gameCamera==null)
			gameCamera = Camera.main.GetComponent<Camera>();
	}

	override public void PopulateDependencies(){}

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
}