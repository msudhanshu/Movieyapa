//
//  RpgCharacterClickListener.cs
//
//  Author:
//       Manjeet <msudhanshu@kiwiup.com>
//
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

//TEMP : Till charactergridobject movement issue fixed : currently only character models moves not the whole gameobject
//IT Transfered onclick event control to parent grid object.
public class RpgCharacterClickListener : MonoBehaviour, IPointerClickHandler
{

	RpgCharacterGridObject rpgCharacterGridObject;

	void Start() {
		rpgCharacterGridObject = transform.parent.GetComponent<RpgCharacterGridObject>();
	}


	virtual public void OnPointerClick (PointerEventData eventData){
		if (rpgCharacterGridObject != null)
			rpgCharacterGridObject.OnPointerClick(eventData);
	}
}

