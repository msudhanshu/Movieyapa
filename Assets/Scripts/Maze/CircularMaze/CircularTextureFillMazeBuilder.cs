using UnityEngine;
using System.Collections;

public class CircularTextureFillMazeBuilder : MazeModelBuilder
{
	private CreateRingSegment _RingBuilder=null;
	public CreateRingSegment RingBuilder {
		get {
			if(_RingBuilder==null) {
				_RingBuilder = new CreateRingSegment();
            }
			return _RingBuilder;
		}
	}

	

	//cache
	float blockHeight {
		get {
			return Mathf.Min(gridWidth,gridHeight)*0.7f;
		}
	}
	Vector2 uvScale;
	float padding = 0.0f;

	//cache
	float discHoleRadius {
		get {
			return (MazeManager.GetInstance().maze3DGrid as CircularMazeGridManager).discHoleRadius ;
		}
	}
	float discInitialAngle {
		get {
			return (MazeManager.GetInstance().maze3DGrid as CircularMazeGridManager).discInitialAngle;
		}
	}
	float totalWidthRadius {
		get {
			return (MazeManager.GetInstance().maze3DGrid as CircularMazeGridManager).radius;
		}
	}

    void Init() {
		if( MazeManager.GetInstance().maze3DGrid is CircularMazeGridManager) {
//			discHoleRadius = (MazeManager.GetInstance().maze3DGrid as CircularMazeGridManager).discHoleRadius ;
//			discInitialAngle = (MazeManager.GetInstance().maze3DGrid as CircularMazeGridManager).discInitialAngle;
//			totalWidthRadius = (MazeManager.GetInstance().maze3DGrid as CircularMazeGridManager).radius;
        }
    }
    
	void Start() {
		Init ();
	}

	//TODO : TAKE ARENA ORIGIN INTO ACCOUNT : 

    protected override GameObject CreateBlock(int mazeX, int mazeY)
	{
		GameObject block = null;
		RingSegmentParam param = new RingSegmentParam();

		param.radius = totalWidthRadius;

		param.radiusStart = discHoleRadius + (float)mazeX * gridWidth - gridWidth/2.0f;
		param.radiusEnd = discHoleRadius + (float)mazeX * gridWidth + gridWidth/2.0f;

		param.angleStart = discInitialAngle + (float)mazeY * gridHeight - gridHeight/2.0f;
		param.angleEnd = discInitialAngle + (float)mazeY * gridHeight + gridHeight/2.0f;

		block = RingBuilder.Create(param);

		block.transform.parent = mazeParent.transform;
		//block.transform.localScale = new Vector3(gridWidth*(1-padding) , blockHeight, gridHeight*(1-padding));
		block.GetComponent<Renderer>().material = DefaultMaterial;
		//block.transform.position = MazeManager.GetInstance().maze3DGrid.GridPositionToWorldPosition(mb.Position);

		//POSITON ++ gridorigin

		return block;
	}

	override protected void AddDragableComponent(GameObject block) {
		if(block.GetComponent<CircularDragableBlock>() == null)
			block.AddComponent<CircularDragableBlock>();
	}

	public void UpdateMeshRadius(GameObject go, GridPosition originalMazePos, GridPosition lastMazePos, float radius) {
		int mazeX = originalMazePos.x;
		int mazeY = originalMazePos.y;

		RingSegmentParam param = new RingSegmentParam();
		param.radius = totalWidthRadius;
		param.radiusStart = discHoleRadius + radius - gridWidth/2.0f;
		param.radiusEnd = discHoleRadius + radius + gridWidth/2.0f;
		
		param.angleStart = discInitialAngle + (float)lastMazePos.y * gridHeight - gridHeight/2.0f;
		param.angleEnd = discInitialAngle + (float)lastMazePos.y * gridHeight + gridHeight/2.0f;
		//param.updateUV = false;


		RingSegmentParam uvParam = new RingSegmentParam();
		
		uvParam.radius = totalWidthRadius;
		
		uvParam.radiusStart = discHoleRadius + (float)mazeX * gridWidth - gridWidth/2.0f;
		uvParam.radiusEnd = discHoleRadius + (float)mazeX * gridWidth + gridWidth/2.0f;
		
		uvParam.angleStart = discInitialAngle + (float)mazeY * gridHeight - gridHeight/2.0f;
		uvParam.angleEnd = discInitialAngle + (float)mazeY * gridHeight + gridHeight/2.0f;

		//MeshFilter meshFilter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
		//meshFilter.sharedMesh = 
		RingBuilder.CreateMesh(go,param,uvParam);
		RingBuilder.CreateMeshCollider(go,param);
	}

	public void UpdateMeshAngle(GameObject go, GridPosition originalMazePos, GridPosition lastMazePos, float angle) {
		int mazeX = originalMazePos.x;
		int mazeY = originalMazePos.y;
		
		RingSegmentParam param = new RingSegmentParam();
		param.radius = totalWidthRadius;
		param.radiusStart = discHoleRadius + (float)lastMazePos.x * gridWidth - gridWidth/2.0f;
		param.radiusEnd = discHoleRadius + (float)lastMazePos.x * gridWidth + gridWidth/2.0f;
		
		param.angleStart = discInitialAngle + angle - gridHeight/2.0f;
		param.angleEnd = discInitialAngle + angle + gridHeight/2.0f;
		//param.updateUV = false;
		
		
		RingSegmentParam uvParam = new RingSegmentParam();
		
		uvParam.radius = totalWidthRadius;
		
		uvParam.radiusStart = discHoleRadius + (float)mazeX * gridWidth - gridWidth/2.0f;
		uvParam.radiusEnd = discHoleRadius + (float)mazeX * gridWidth + gridWidth/2.0f;
		
		uvParam.angleStart = discInitialAngle + (float)mazeY * gridHeight - gridHeight/2.0f;
		uvParam.angleEnd = discInitialAngle + (float)mazeY * gridHeight + gridHeight/2.0f;

		RingBuilder.CreateMesh(go,param,uvParam);
		RingBuilder.CreateMeshCollider(go,param);
       // MeshFilter meshFilter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
        //meshFilter.sharedMesh = RingBuilder.CreateMesh(param,uvParam);
	}
}
