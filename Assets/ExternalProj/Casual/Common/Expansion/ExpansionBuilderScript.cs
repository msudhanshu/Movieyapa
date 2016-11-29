using UnityEngine;
using System.Collections;
using System;
using Expansion;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class ExpansionBuilderScript : MonoBehaviour
{
	public GameObject expansionBlock;
	public bool inXDirection;
	public int fixCoordinate;
	public int start;
	public int end;
	public int sizeX = 1;
	public int sizeZ = 1;
	public int clearX;
	public int clearY;
		
	public void ClearExpansionBlock ()
	{
		GameObject obj = GameObject.Find ("ExpansionBlock_" + clearX + "_" + clearY);
		if (obj != null) {
			ExpansionBlock clearObj = obj.GetComponent<ExpansionBlock> ();
			clearObj.OnClear ();
		}
	}
	#if UNITY_EDITOR
	public void CreateExpansionBlock()
		{
		CreateExpansionHolder ();
		int delta = 1;
		int initialFixCoordinate = fixCoordinate;
		float x = 0f, y = 0f, z = 0f;
		for(int j = 0; j< 8; j++){
			fixCoordinate = initialFixCoordinate + j*12;
			if (inXDirection) {
				x = start;
				z = fixCoordinate;
				delta = sizeX;
			} else {
				x = fixCoordinate;
				z = start;
				delta=sizeZ;
			}
		for (int i = start; i < end; i += delta) {
				if(!(x >= 30 && x <= 54 && z >= 30 && z <= 54)){
				
			Vector3 location = new Vector3 (x, 0, z);
			UnityEngine.Object prefab =  AssetDatabase.LoadAssetAtPath("Assets/Kiwi/Content/Prefabs/ExpansionBlock.prefab", typeof(GameObject));
			GameObject newObject = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab);
			newObject.transform.parent = GameObject.Find ("ExpansionBlocks").transform;
			newObject.transform.localPosition = location;
			location.y = Terrain.activeTerrain.SampleHeight(newObject.transform.position) + Terrain.activeTerrain.transform.position.y;
			newObject.transform.localPosition = location;
			newObject.transform.name = "ExpansionBlock_" + x + "_" + z;
			var boxCollider = expansionBlock.GetComponent<BoxCollider>();
			boxCollider.size = new Vector3(sizeX,4,sizeZ);
				}
			if (inXDirection)
				x += delta;
			else
				z += delta;
		}
		}
		}
	#endif

	private void CreateExpansionHolder ()
	{
		if (GameObject.Find ("ExpansionBlocks") == null) {
			GameObject expansionBlocks = new GameObject ();
			expansionBlocks.transform.parent = GameObject.Find ("ArenaGameView").transform;
			expansionBlocks.name = "ExpansionBlocks";
			expansionBlocks.transform.rotation = Quaternion.identity;
			expansionBlocks.transform.localPosition = new Vector3 (-1.5f, 0f, -1.5f);
		}
	}
}

