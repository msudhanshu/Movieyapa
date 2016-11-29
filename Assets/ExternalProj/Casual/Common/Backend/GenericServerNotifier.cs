using UnityEngine;
using System.Collections;

public class GenericServerNotifier : ServerNotifier {

	public void OnSuccess(GameResponse response){
		Debug.Log("Succesfull Response");
	}

	public void OnFailure(string error){
		Debug.Log("Failed Response");
	}
}
