using UnityEngine;
using System.Collections;


/*
 * ???todo : how will it interact with effect manager
1. individual texture/model builder

//for unlockdisc
2. collective textured plane builder
3. collective modeled and textured builder
*/
public class RectModelMazeBuilder : MazeModelBuilder
{
	public Material HintDummyBlockMat;

	virtual protected float blockHeight {
		get {
			return Mathf.Min(gridWidth,gridHeight)*0.7f;
		}
	}
	protected Vector2 uvScale;
	protected float padding = 0.2f;

	public override GameObject GetHintDummyBlock(int mazeX, int mazeY) {
		if(!_HintDummyBlock) {
			_HintDummyBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
			_HintDummyBlock.GetComponent<Renderer>().material = HintDummyBlockMat;
			_HintDummyBlock.transform.localScale = new Vector3(gridWidth*(1-padding) , blockHeight*0.3f, gridHeight*(1-padding));
		}
		return _HintDummyBlock;
	}

	protected override GameObject CreateBlock(int mazeX, int mazeY)
	{
		GameObject block = null;

		//cache
		//if(uvScale == null)
		{
			uvScale = new Vector2(1.0f/(float)MazeManager.GetInstance().maze3DGrid.gridSizeX,
			                      1.0f/(float)MazeManager.GetInstance().maze3DGrid.gridSizeY);
		}
		//block = GameObject.CreatePrimitive(PrimitiveType.Cube);
		Vector2 uvOffset = new Vector2(mazeX/(float)MazeManager.GetInstance().maze3DGrid.gridSizeX,
		                              mazeY/(float)MazeManager.GetInstance().maze3DGrid.gridSizeY);

		//seperate model builder .. it can be plane or any specified models
		//block = new CreatePlane().Create();

		block = GameObject.CreatePrimitive(PrimitiveType.Cube);

		//collective textured plane model
	//	block = new CreatePlane().Create(uvOffset,uvScale);

		block.transform.localScale = new Vector3(gridWidth*(1-padding) , blockHeight, gridHeight*(1-padding));
		block.GetComponent<Renderer>().material = DefaultMaterial;

		block.transform.parent = mazeParent.transform;
		block.transform.position = MazeManager.GetInstance().maze3DGrid.GridPositionToWorldPosition(
			new GridPosition(mazeX,mazeY));
		return block;
	}

	override protected void AddDragableComponent(GameObject block) {
		if(block.GetComponent<RectDragableBlock>() == null)
			block.AddComponent<RectDragableBlock>();
	}
}
