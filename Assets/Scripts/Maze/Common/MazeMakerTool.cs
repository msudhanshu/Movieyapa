
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public class MazeMakerTool : EditorWindow
{
 
	[MenuItem("Tools/Maze Maker")]
	public static void OpenEditorWindow()
	{
		//MazeMakerTool editor = GetWindow<MazeMakerTool>();
		//editor.title = "Maze Maker";

		GameObject mazeTool;
		mazeTool = GameObject.Find ("MazeMapBuilderTool(Clone)");
		if (mazeTool == null) {
						mazeTool = (GameObject)Resources.Load ("MazeMapBuilderTool");
						if (mazeTool == null) {
								Debug.LogWarning ("MazeMapBuilderTool Prefab is missing");
						}
						mazeTool = Instantiate (mazeTool) as GameObject;
				}
		Selection.activeGameObject = mazeTool;
	}







	/*
    public GameObject[] blocks;
	public GameObject[] bases;
	public GameObject[] floors;
	public GameObject[] roofs;

	public bool useMapAsset;

	public TextAsset mapAsset;
	private TextAsset generatedMapAsset;
    
	public int blockXNum = 10;
    public int blockZNum = 10;
    public float blockSpan = 10f;

    public int buildingXNum = 4;
    public int buildingZNum = 2;
    public float buildingSpan = 10f;

    GameObject cityRoot;

	public bool BuildButton = false;

    int numBlocks;
    int numBuildings;

    float[] cardinals = { 0, 90, 180, 270 };

    bool building;

	private bool groupEnabled = false;

	private void OnGUI()
	{
		EditorGUILayout.BeginVertical();

		useMapAsset = EditorGUILayout.Toggle("Use Maze Map", useMapAsset);

		//RenderMazeMapAsset ();
		//EditorGUILayout.EndToggleGroup ();


		RenderMazeModelParam ();

		if (!useMapAsset)
				RenderMazeParam ();
		else 
				RenderMazeMapAsset ();
				

		EditorGUILayout.EndVertical();
	}

	private void RenderMazeModelParam() {
	//	blocks = (GameObject[])EditorGUILayout.ObjectField ("Maze Map", blocks,typeof(GameObject[]));

	}

	private void RenderMazeParam() {
		blockXNum = EditorGUILayout.IntField("blockXNum",blockXNum);
		blockZNum = EditorGUILayout.IntField("blockZNum",blockZNum);
		blockSpan = EditorGUILayout.FloatField("blockSpan",blockSpan);

		buildingXNum = EditorGUILayout.IntField("buildingXNum",buildingXNum);
		buildingZNum = EditorGUILayout.IntField("buildingZNum",buildingZNum);
		buildingSpan = EditorGUILayout.FloatField("buildingSpan",buildingSpan);
	}

	private void RenderMazeMapAsset() {
	//	EditorGUILayout.ObjectField(
		mapAsset = (TextAsset)EditorGUILayout.ObjectField ("Maze Map", mapAsset,typeof(TextAsset));
	}

	void Update() {
		if (BuildButton) {
			//delete old and create new

			BuildButton = false;
			if(cityRoot)
				Destroy(cityRoot);

			MazeMap mazeMap;
			if(useMapAsset) {
				mazeMap = new MazeMapLoader().Deserialize(mapAsset);
			    generatedMapAsset = mapAsset;
			//StartCoroutine(Generate(new MazeMap(mapAsset)));
			} else {
				mazeMap = new MazeMapGeneratorAlgo().GenerateMaze(blockXNum,blockZNum);
				generatedMapAsset = new MazeMapLoader().Serialize(mazeMap, "filename");
			}
			//StartCoroutine(Generate(mazeMap));
		//	Generate(mazeMap);
		}
	}
	*/
}


#endif
