#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(QATool), true)]
public class QAToolEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		QATool myScript = (QATool)target;
		if(GUILayout.Button("Reload"))
		{
			myScript.ReloadEffect();
		}
	}
}
#endif