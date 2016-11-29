using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor (typeof(PlayerPrefsPersistenceManager))]
public class PlayerPrefsPersistenceManagerEditor : Editor {

	override public void OnInspectorGUI () {
		EditorGUILayout.BeginVertical();
		//GUILayout.BeginArea(new Rect(0, GUILayoutUtility.GetLastRect().y, Screen.width, 20));
		if (GUILayout.Button("Clear Saved Game")) {
			PlayerPrefs.DeleteKey(((PlayerPrefsPersistenceManager)target).GetPrefName());
			Debug.Log ("Deleting user ID :" + PlayerPrefs.GetInt(Config.USER_ID_KEY));
			PlayerPrefs.DeleteKey(Config.USER_ID_KEY);
		}

		if (GUILayout.Button("Print Saved Game ")) {
			Debug.Log (PlayerPrefs.GetString(((PlayerPrefsPersistenceManager)target).GetPrefName(), "NOT FOUND"));
			Debug.Log (PlayerPrefs.GetInt(Config.USER_ID_KEY, -1));
		}
		//GUILayout.EndArea();
		EditorGUILayout.EndHorizontal();
		
		
		DrawDefaultInspector();			
	}
}
