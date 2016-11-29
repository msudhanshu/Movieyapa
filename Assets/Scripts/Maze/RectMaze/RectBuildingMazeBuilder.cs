using UnityEngine;
using System.Collections;


/*
 * ???todo : how will it interact with effect manager
1. individual texture/model builder

//for unlockdisc
2. collective textured plane builder
3. collective modeled and textured builder
*/
public class RectBuildingMazeBuilder : RectModelMazeBuilder
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
	
	int numBlocks;
	int numBuildings;
	float[] cardinals = { 0, 90, 180, 270 };


	private float blockXSize;
	private float blockZSize ;
	private float blockXOffset;
	private float blockZOffset;
	private Vector3 blockPos;

	private void InitBlockPosForGrid() {
		
		 blockXSize = buildingXNum * buildingSpan;
		 blockZSize = buildingZNum * buildingSpan;
		
		 blockXOffset = (((blockXNum - 1) * blockSpan) + ((blockXNum - 1) * blockXSize)) * 0.5f;
		 blockZOffset = (((blockZNum - 1) * blockSpan) + ((blockZNum - 1) * blockZSize)) * 0.5f;
		
		 blockPos = new Vector3(-blockXOffset, 0, -blockZOffset);
    }

	override public void Generate(MazeMap mazeMap, GameObject mazeParent)
	{
		InitBlockPosForGrid();
        base.Generate(mazeMap,mazeParent);
	}

	protected Vector3 GetBlockWorldPosition(int mazeX, int mazeY) {
//		for (var i = 0; i < mazeMap.mazeX; i ++) {
//			for (var j = 0; j < mazeMap.mazeY; j ++) {
//				switch(mazeMap.mazeGrid[i,j]) {
//				case MazeBlockType.FREE:
//				case MazeBlockType.NULL:
//					//CreateBlock(blockPos);
//					 ;
//					//yield return 0;//new WaitForEndOfFrame();
//					break;
//				default:
//					GameObject block = CreateAnyBlock(i , j,mazeMap.mazeGrid[i,j]);
//					mazeMap.mazeBlockOccupied.Add(block.name,block.GetComponent<MazeBlock>());
//					//CreateBuilding(blockPos,cityRoot.transform);
//					blockPos.z -= blockZSize + blockSpan;
//                    //yield return 0;//new WaitForEndOfFrame();
//                    break;
//                }
//            }
//            blockPos.x += blockXSize + blockSpan;
//            blockPos.z = +blockZOffset;
//        }

		return new Vector3(blockPos.x + mazeX*(blockXSize + blockSpan) , 0 ,  blockPos.z - mazeY*(blockZSize + blockSpan));
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

	protected override GameObject CreateBlock(int mazeX, int mazeY)
	{
		Vector3 position = GetBlockWorldPosition(mazeX,mazeY);
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

}
