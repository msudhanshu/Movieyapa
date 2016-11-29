#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(EffectTool), true)]
public class EffectToolEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		EffectTool myScript = (EffectTool)target;
		if(GUILayout.Button("Reload"))
		{
			myScript.ReloadEffect();
		}
	}
}
#endif