using UnityEngine;
using System;
using UnityEditor;
[CustomEditor(typeof(ExpansionBuilderScript))]
public class ExpansionBuilderEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		ExpansionBuilderScript myScript = (ExpansionBuilderScript)target;
		//myScript.selGridInt = GUILayout.SelectionGrid(myScript.selGridInt, myScript.selStrings, 2);
		if (GUILayout.Button ("Create")) {
			myScript.CreateExpansionBlock ();
		} else if (GUILayout.Button ("Clear")) {
			myScript.ClearExpansionBlock();
		}
	}
}

