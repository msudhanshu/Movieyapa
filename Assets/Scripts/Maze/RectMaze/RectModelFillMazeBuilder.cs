using UnityEngine;
using System.Collections;

public enum BLOCKOBJECT_AXIS {
	CENTER,
	BOTTOM_CENTER,    // Y =  BOTTOM
	BOTTOM_LEFT_DOWN //Y=BOTTOM,X=LEFT,Z=DOWN
}

/*
 * ???todo : how will it interact with effect manager
1. individual texture/model builder

//for unlockdisc
2. collective textured plane builder
3. collective modeled and textured builder
*/
public class RectModelFillMazeBuilder : RectTextureFillMazeBuilder
{
	public GameObject blockObject;
	public BLOCKOBJECT_AXIS blockObjectAxis = BLOCKOBJECT_AXIS.BOTTOM_LEFT_DOWN;
	override protected float blockHeight {
		get {
			return blockObjectSize.y;// Mathf.Min(gridWidth,gridHeight);
		}
	}

	private Vector3? _blockObjectSize;
	protected Vector3 blockObjectSize {
		get {
			if(_blockObjectSize == null) {
				_blockObjectSize = blockObject.GetComponent<Renderer>().bounds.size;
				//GetComponent<MeshFilter>().mesh.bounds;
			}
			return _blockObjectSize??Vector3.one;
		}
	}

	override public void Generate(MazeMap mazeMap, GameObject mazeParent)
	{
		//TODO : HACK : 
		//NOT SURE IF SETTING GRIDWIDHT HERE IS OK. WHAT IF IT IS BEING USED BEFORE THIS AS WELL ?

		//****may be we can write a wrapper which will create mazegrid and builder as well.. It can controll this very well.

		//reset the size of mazegridmanager so that this model fits into that.
		//get the object/prefab width and height (boudary may be of its maximum enclosing box collider)
		MazeManager.GetInstance().maze3DGrid.gridWidth =blockObjectSize.x/MazeManager.GetInstance().maze3DGrid.gridSizeX;
		MazeManager.GetInstance().maze3DGrid.gridHeight =blockObjectSize.z/MazeManager.GetInstance().maze3DGrid.gridSizeY;
		base.Generate(mazeMap,mazeParent);
	}

	protected override GameObject CreateBlock(int mazeX, int mazeY)
	{
		GameObject mazeBlock = new GameObject();

		Vector3 gridLeftDownPosition =  MazeManager.GetInstance().maze3DGrid.gridOrigin - new Vector3(gridWidth/2.0f,0,gridHeight/2.0f);;
		Vector3 gridPositionOffset = Vector3.zero;
		switch(blockObjectAxis) {
		case BLOCKOBJECT_AXIS.BOTTOM_LEFT_DOWN:
			gridPositionOffset = Vector3.zero;
			break;
		case BLOCKOBJECT_AXIS.BOTTOM_CENTER:
			gridPositionOffset = new Vector3(blockObjectSize.x/2.0f,0,blockObjectSize.z/2.0f);
			break;
		case BLOCKOBJECT_AXIS.CENTER:
			gridPositionOffset = blockObjectSize/2.0f;
			break;
		}

		mazeBlock.transform.position = MazeManager.GetInstance().maze3DGrid.GridPositionToWorldPosition(
			new GridPosition(mazeX,mazeY));
		
		mazeBlock.transform.parent = mazeParent.transform;
		
		//COLLIDER WORK
		BoxCollider collider = mazeBlock.AddComponent<BoxCollider>();
		//TODO :blockHeight should depend on mesh avarge height 
		collider.center = new Vector3(0,blockHeight/2.0f-gridPositionOffset.y/2.0f,0);
		collider.size = new Vector3(gridWidth,blockHeight/2.0f,gridHeight);


		GameObject block = null;

		//collective modeled and textured
		block =  (GameObject)GameObject.Instantiate(blockObject);//new CreatePlane().Create();
		//BoxCollider blockcollider = block.AddComponent<BoxCollider>();
		//blockcollider.size = blockObjectSize;
		//block.transform.localScale = new Vector3((float)MazeManager.GetInstance().maze3DGrid.gridSizeX *gridWidth , blockHeight,
		  //                                       (float)MazeManager.GetInstance().maze3DGrid.gridSizeY*gridHeight);
		//block.RemoveComponent of all collider type;



		block.GetComponent<Renderer>().material = DefaultMaterial;
		block.GetComponent<Renderer>().material.SetFloat("startX",(float)mazeX*gridWidth - gridPositionOffset.x);
		block.GetComponent<Renderer>().material.SetFloat("startY",(float)mazeY*gridHeight - gridPositionOffset.z);
		block.GetComponent<Renderer>().material.SetFloat("endX",(float)mazeX*gridWidth+gridWidth - gridPositionOffset.x);
		block.GetComponent<Renderer>().material.SetFloat("endY",(float)mazeY*gridHeight+gridHeight - gridPositionOffset.z);
		//set boundary 
		block.transform.position = gridLeftDownPosition + gridPositionOffset;
		block.transform.parent = mazeBlock.transform;
		return mazeBlock;
	}

	override protected void AddDragableComponent(GameObject block) {
		if(block.GetComponent<RectDragableBlock>() == null)
			block.AddComponent<RectDragableBlock>();
	}
}
