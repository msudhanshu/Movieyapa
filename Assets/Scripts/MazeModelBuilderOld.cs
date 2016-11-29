using UnityEngine;
using System.Collections;

public class MazeModelBuilderOld : MonoBehaviour
{
    public Transform[] blocks;
    public Transform[] bases;
    public Transform[] floors;
    public Transform[] roofs;

	public bool ShouldCreateBuilding = false;

    public int blockXNum = 10;
    public int blockZNum = 10;
    public float blockSpan = 10f;
    public int buildingXNum = 4;
    public int buildingZNum = 2;
    public float buildingSpan = 10f;

	
	public float INTERBLOCKSPACE_FACTOR = 1.0f;

	GameObject mazeParent;
	
    int numBlocks;
    int numBuildings;
    float[] cardinals = { 0, 90, 180, 270 };
    bool building;

	public void Generate(MazeMap mazeMap, GameObject mazeParent)
	{
		this.mazeParent = mazeParent;
		StartCoroutine (GenerateCoroutine (mazeMap));
	}

	IEnumerator GenerateCoroutine(MazeMap mazeMap)
	{
		building = true;
		
		float blockXSize = buildingXNum * buildingSpan;
		float blockZSize = buildingZNum * buildingSpan;
		
		float blockXOffset = (((blockXNum - 1) * blockSpan) + ((blockXNum - 1) * blockXSize)) * 0.5f;
		float blockZOffset = (((blockZNum - 1) * blockSpan) + ((blockZNum - 1) * blockZSize)) * 0.5f;
		
		Vector3 blockPos = new Vector3(-blockXOffset, 0, -blockZOffset);
		
		
		for (var i = 0; i < mazeMap.mazeX; i ++) {
			for (var j = 0; j < mazeMap.mazeY; j ++) {

				switch(mazeMap.mazeGrid[i,j]) {
				case MazeBlockType.BLOCK1:
					GameObject block = CreateBlock(blockPos, i , j);
					MazeManager.GetInstance().maze3DGrid.mazeBlockOccupied.Add(block.name,block.GetComponent<MazeBlock>());
					//CreateBuilding(blockPos,cityRoot.transform);
					blockPos.z -= blockZSize + blockSpan;
					//yield return 0;//new WaitForEndOfFrame();
					break;
				case MazeBlockType.FREE:
				case MazeBlockType.NULL:
					//CreateBlock(blockPos);
					blockPos.z -= blockZSize + blockSpan;
					//yield return 0;//new WaitForEndOfFrame();
					break;
				}

				if(MazeBlock.IsSourceBlock(mazeMap.mazeGrid[i,j])) {
					GameObject block = CreateBlock(blockPos, i , j);
					MazeManager.GetInstance().maze3DGrid.mazeBlockOccupied.Add(block.name,block.GetComponent<MazeBlock>());
					//CreateBuilding(blockPos,cityRoot.transform);
					blockPos.z -= blockZSize + blockSpan;
				}

			}
			blockPos.x += blockXSize + blockSpan;
			blockPos.z = +blockZOffset;
		}
		
		/*	for (int x = 0; x < blockXNum; ++x)
		{
			for (int z = 0; z < blockZNum; ++z)
			{
				CreateBlock(blockPos);
                blockPos.z += blockZSize + blockSpan;
                yield return new WaitForEndOfFrame();
            }

            blockPos.x += blockXSize + blockSpan;
            blockPos.z = -blockZOffset;
        }
        */
		
		building = false;
		yield return 0;
	}

	GameObject CreateBlock(Vector3 position, int mazeX, int mazeY)
	{
		Transform block = null;
		
		if (blocks.Length > 0)
		{
			block = (Transform)Instantiate(blocks[Random.Range(0,blocks.Length)], position, Quaternion.identity);
		}
		else
		{
			block = new GameObject("block").transform;
			block.position = position;
		}// "Block" + ++numBlocks;
		block.name = MazeManager.GetMazeBlockName (mazeX, mazeY);
		MazeBlock mb = block.gameObject.AddComponent<MazeBlock> ();
		mb.mazeBlockType = MazeBlockType.BLOCK1;
		block.gameObject.layer = MazeManager.MAZE_BLOCK_LAYER;
		block.parent = mazeParent.transform;

		float blockXScale = ((buildingXNum * buildingSpan) + blockSpan) * INTERBLOCKSPACE_FACTOR;
		float blockZScale = ((buildingZNum * buildingSpan) + blockSpan) * INTERBLOCKSPACE_FACTOR;
		
		block.localScale = new Vector3(blockXScale, 1, blockZScale);
		
		float buildingXOffset = ((buildingXNum - 1) * buildingSpan) * 0.5f;
		float buildingZOffset = ((buildingZNum - 1) * buildingSpan) * 0.5f;
		
		Vector3 buildingPos = new Vector3(position.x - buildingXOffset, 0, position.z - buildingZOffset);

		if(ShouldCreateBuilding){
			for (int x = 0; x < buildingXNum; ++x)
			{
				for (int z = 0; z < buildingZNum; ++z)
				{
					CreateBuilding(buildingPos, block);
					buildingPos.z += buildingSpan;
				}
				
				buildingPos.x += buildingSpan;
				buildingPos.z = position.z - buildingZOffset;
			}
		}
		// block.gameObject.AddComponent("CombineChildren");
		return block.gameObject;
	}
	
	void CreateBuilding(Vector3 position, Transform block)
	{
		Vector3 curPosition = position;
		
		++numBuildings;
		
		Transform c_parent = null;
		
		if (bases.Length > 0)
		{
			Transform b = (Transform)Instantiate(bases[Random.Range(0, bases.Length)], curPosition, Quaternion.AngleAxis(cardinals[Random.Range(0,cardinals.Length)], Vector3.up));
			b.name = "Base" + numBuildings;
			b.parent = block;
			c_parent = b;
		}
		
		if (floors.Length > 0)
		{
			int numFloors = Random.Range(1, 10);
			
			for (int i = 0; i < numFloors; ++i)
			{
				curPosition.y += 4;
				Transform f = (Transform)Instantiate(floors[Random.Range(0, floors.Length)], curPosition, Quaternion.identity);
				f.name = "Floor" + numBuildings + "_" + i;
				f.parent = c_parent;
				c_parent = f;
			}
		}
		
		curPosition.y += 4;
		
		if (roofs.Length > 0)
		{
			Transform r = (Transform)Instantiate(roofs[Random.Range(0, roofs.Length)], curPosition, Quaternion.AngleAxis(cardinals[Random.Range(0, cardinals.Length)], Vector3.up));
			r.name = "Roof" + numBuildings;
			r.parent = c_parent;
		}
	}

    void OnGUI()
    {
		return;
        if (building)
        {
            GUILayout.Label("Building...");
        }
        else
        {
            //public int blockXNum = 10;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Block X Num: ");
            blockXNum = System.Int32.Parse(GUILayout.TextField(blockXNum.ToString(), GUILayout.Width(50)));
            GUILayout.EndHorizontal();

            //public int blockZNum = 10;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Block Z Num: ");
            blockZNum = System.Int32.Parse(GUILayout.TextField(blockZNum.ToString(), GUILayout.Width(50)));
            GUILayout.EndHorizontal();

            //public float blockSpan = 10f;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Block Span: ");
            blockSpan = (float)System.Double.Parse(GUILayout.TextField(blockSpan.ToString(), GUILayout.Width(50)));
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            //public int buildingXNum = 4;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Building X Num: ");
            buildingXNum = System.Int32.Parse(GUILayout.TextField(buildingXNum.ToString(), GUILayout.Width(50)));
            GUILayout.EndHorizontal();

            //public int buildingZNum = 2;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Building Z Num: ");
            buildingZNum = System.Int32.Parse(GUILayout.TextField(buildingZNum.ToString(), GUILayout.Width(50)));
            GUILayout.EndHorizontal();

            //public float buildingSpan = 10f;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Building Span: ");
            buildingSpan = (float)System.Double.Parse(GUILayout.TextField(buildingSpan.ToString(), GUILayout.Width(50)));
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

           // if (GUILayout.Button("Generate"))
           //     StartCoroutine(Generate());
        }
    }
}
