//
//  RpgCharacterGridObject.cs
//
//  Author:
//       Manjeet <msudhanshu@kiwiup.com>
//
using UnityEngine;
using System.Collections;
using Pathfinding;
using UnityEngine.EventSystems;
/**
 * A Character view implementation
 */ 

public class RpgCharacterGridObject : CharacterGridObject ,ICallOutClickListner
{
	bool SW_STYLE = true;

	protected RpgCharacter rpgCharacter {
		get { return assetController as RpgCharacter;}
		set {}
	}

	/**
	 * Building state changed, update view.
	 */ 
	virtual public void UI_UpdateState() {
		Color testcolor = Color.red;
		if( Utility.StringEquals( rpgCharacter.State.state, "first" ) ) {
			isFree = false;
			testcolor  = Color.red;
		} else if( Utility.StringEquals( rpgCharacter.State.state, "place" ) ) {
			testcolor = Color.yellow;
			isFree = true;
			Debug.Log("RpgCharacter , Level ="+rpgCharacter.data.level+
			          ". Health: "+rpgCharacter.Health + 
			          " walkspeed: "+rpgCharacter.Walkspeed
			          );

		} else if( Utility.StringEquals( rpgCharacter.State.state, "upgrade" ) ) {
			testcolor = Color.green;
		}

		foreach(GameObject go in components) {
			go.GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");
			go.GetComponent<Renderer>().material.color = testcolor;
			go.SetActive (true);
		}

		if (!SW_STYLE) {
			GetActivityStatus().ShowCallOut(rpgCharacter.State.activity.GetActionIconSprite());
			GetActivityStatus().SetText(rpgCharacter.data.level + ":" + rpgCharacter.State.state);
		}
		
	}
	
	/**
	 * transition completed.
	 */ 
	virtual public void UI_StartTransition(StateTransition transition) {
		if (!SW_STYLE)
			GetActivityStatus().ShowProgressBar();	
	}
	
	
	/**
	 * Indicate progress on the progress ring.
	 */
	virtual public void UI_UpdateProgress(StateTransition transition) {
		if (!SW_STYLE) {
			GetActivityStatus().UpdateProgressUI(transition.PercentageComplete);
			GetActivityStatus().SetText(rpgCharacter.data.level + ":" + rpgCharacter.State.state + "-" + transition.PercentageComplete);
		}
	}
	
	/**
	 * transition completed.
	 */ 
	virtual public void UI_CompleteTransition(StateTransition transition) {
		if (!SW_STYLE) {
			GetActivityStatus().UpdateProgressUI(1);
			GetActivityStatus().ShowCallOut(transition.State.activity.GetActionIconSprite());
		}
	}

	public override void OnPointerClick (PointerEventData eventData){
		base.OnPointerClick(eventData);
		if( Utility.StringEquals( rpgCharacter.State.state, "place") )
			rpgCharacter.UpgradeToNextLevel();
	}

	public virtual void OnCallOutClick() {
		if( Utility.StringEquals( rpgCharacter.State.state, "place") )
			rpgCharacter.UpgradeToNextLevel();
	}
}

