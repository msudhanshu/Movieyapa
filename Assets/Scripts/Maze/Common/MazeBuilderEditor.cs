using UnityEngine;
using System.Collections;

#if UNITY_EDITOR

using UnityEditor;

[ExecuteInEditMode]
public class MazeBuilderEditor : MonoBehaviour
{
	public TextAsset mapAsset;

	public int mazeSizeX = 10;
	public int mazeSizeY = 10;
	public string MazeRootName = "mazeRoot";

	public bool useMapAsset;
	public bool BuildButton = false;
	public bool SaveButton = false;



	private MazeModelBuilder _mazeModelBuilder;
	private MazeModelBuilder mazeModelBuilder {
		get {
			if(_mazeModelBuilder==null)
					_mazeModelBuilder = GetComponent<MazeModelBuilder>();
			return _mazeModelBuilder;
		}
		set {

		}
	}

	MazeMap mazeMap;
	GameObject mazeRoot;

	void Awake() {
		if (EditorApplication.isPlaying) {
				Debug.Log ("Destroying itself on game run");
				DestroyImmediate(gameObject);
		}
	}

	void Update() {
		if (BuildButton) {
			BuildButton = false;

			if(useMapAsset) {
				mazeMap = new MazeMapLoader().Deserialize(mapAsset);
			} else {
				mazeMap = MazeManager.GetInstance().mazeMapGenerator.GenerateMaze(mazeSizeX,mazeSizeY);
			}

			mazeModelBuilder.Generate(mazeMap,GetMazeRootObject());
		}


		if (SaveButton) {
			SaveButton = false;
			if(mazeMap!=null) {
				MazeRootDetail mazeRootDetail = mazeRoot.AddComponent<MazeRootDetail> ();
			if(!useMapAsset) {
				TextAsset generatedMapAsset = new MazeMapLoader().Serialize(mazeMap,MazeRootName);
				mazeRootDetail.mazeMapAsset = generatedMapAsset;
			} else
					mazeRootDetail.mazeMapAsset = mapAsset;
			}
		}
	}

	private GameObject GetMazeRootObject() {
		mazeRoot = GameObject.Find (MazeRootName);
		if (mazeRoot)
			DestroyImmediate(mazeRoot);
		mazeRoot = new GameObject(MazeRootName);
		return mazeRoot;
	}

}

#endif
