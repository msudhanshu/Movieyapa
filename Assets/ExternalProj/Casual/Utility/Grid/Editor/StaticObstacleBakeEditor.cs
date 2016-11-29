using UnityEngine;
#if (UNITY_EDITOR)
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor (typeof(StaticObstacleBake))]
public class StaticObstacleBakeEditor : Editor {

	StaticObstacleBake staticObstacleBake;

	public void OnEnable()
	{
		staticObstacleBake = (StaticObstacleBake)target;
		SceneView.onSceneGUIDelegate += GridUpdate;
	}

	public void OnDisable()
	{
		SceneView.onSceneGUIDelegate -= GridUpdate;
	}

	void GridUpdate(SceneView sceneview)
	{
		Event e = Event.current;

	//	Ray r = Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
	//	Vector3 mousePos = r.origin;
	//	if (e.isKey && e.character == 'a')
				
	}
	
	public override void OnInspectorGUI()
	{
	
		if (staticObstacleBake.editMap == false) {
					if (GUILayout.Button ("EditMap", GUILayout.Width (210))) { 
							staticObstacleBake.editMap = true;
						Debug.Log ("Edit Tick on");
					}
			} else
		{
			if (GUILayout.Button ("Editing the Map..", GUILayout.Width (200))) { 
				staticObstacleBake.editMap = false;
				Debug.Log ("Edit Tick Off");
			}
		}

		EditorGUILayout.HelpBox ("Auto Bake with object in Obstacle layer", MessageType.Info);
		if (staticObstacleBake.autoBakeMap == false) {
			if (GUILayout.Button ("AutoBake", GUILayout.Width (210))) { 
				staticObstacleBake.autoBakeMap = true;
				Debug.Log ("Tick on");
			}
		} else
		{
			if (GUILayout.Button ("AutoBaking...", GUILayout.Width (200))) { 
				staticObstacleBake.autoBakeMap = false;
				Debug.Log ("Tick Off");
			}
		}

		if (staticObstacleBake.clearMap == false) {
			if (GUILayout.Button ("Clear", GUILayout.Width (210))) { 
				staticObstacleBake.clearMap = true;
				Debug.Log ("Tick on");
			}
		} else
		{
			if (GUILayout.Button ("Clearing...", GUILayout.Width (200))) { 
				staticObstacleBake.clearMap = false;
				Debug.Log ("Tick Off");
			}
		}

		EditorGUILayout.HelpBox ("Setting Parameter", MessageType.Info);
		DrawDefaultInspector ();
		DrawBrush ();

		EditorGUILayout.HelpBox ("Bake and Save Now", MessageType.Info);
		if (staticObstacleBake.bakeMap == false) {
			if (GUILayout.Button ("Bake & Save", GUILayout.Width (210))) { 
				staticObstacleBake.bakeMap = true;
				Debug.Log ("Tick on");
			}
		} else
		{
			if (GUILayout.Button ("Bake & Save ...", GUILayout.Width (200))) { 
				staticObstacleBake.bakeMap = false;
				Debug.Log ("Tick Off");
			}
		}

		//	DrawGridView ();
		//	DrawToolCamera ();
		SceneView.RepaintAll();
	}

	private void DrawBrush() {

	}
	
	#region GridView
	private void DrawGridView() {
				EditorGUILayout.HelpBox ("Grid View Param", MessageType.Info);
	}
	#endregion


	#region ToolCamera
	private void DrawToolCamera() {
		EditorGUILayout.HelpBox ("Camera Param", MessageType.Info);
	}
	#endregion

}
#endif