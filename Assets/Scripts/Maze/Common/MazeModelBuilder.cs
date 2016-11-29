using UnityEngine;
using System.Collections;

public abstract class MazeModelBuilder : MonoBehaviour
{
	bool building;
    protected GameObject mazeParent;
	protected MazeMap mazeMap;
	//this thing should bcontrolled from level Effect manager
	public Material DefaultMaterial;


	protected float gridWidth {
		get {
			return MazeManager.GetInstance().maze3DGrid.gridWidth;
		}
	}
	protected float gridHeight {
		get {
			return MazeManager.GetInstance().maze3DGrid.gridHeight;
		}
	}

	virtual public void Generate(MazeMap mazeMap, GameObject mazeParent)
	{
		this.mazeMap = mazeMap;
		this.mazeParent = mazeParent;
		StartCoroutine (GenerateCoroutine ());
	}

	//should be part of effect manager
	protected GameObject _HintDummyBlock=null;
	public virtual GameObject GetHintDummyBlock(int mazeX, int mazeY) {
		if(!_HintDummyBlock)
			_HintDummyBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
		return _HintDummyBlock;
	}


	IEnumerator GenerateCoroutine()
	{
		building = true;
		for (var i = 0; i < mazeMap.mazeX; i ++) {
			for (var j = 0; j < mazeMap.mazeY; j ++) {
				switch(mazeMap.mazeGrid[i,j]) {
					case MazeBlockType.FREE:
					case MazeBlockType.NULL:
						//CreateBlock(blockPos);
						break;
					 default:
						GameObject block = CreateAnyBlock(i , j,mazeMap.mazeGrid[i,j]);
						break;
				}
			}
		}
		building = false;
		yield return 0;
	}

	GameObject CreateAnyBlock(int mazeX, int mazeY, MazeBlockType blockType)
	{
		GameObject block = CreateBlock(mazeX , mazeY);
		block.name = MazeManager.GetMazeBlockName (mazeX, mazeY);

		MazeBlock mb = block.gameObject.GetComponent<MazeBlock>();
		if(mb==null) 
			mb = block.gameObject.AddComponent<MazeBlock>();

		AddDragableComponent(block);

//		MazeManager.GetInstance().maze3DGrid.blockGrid[mazeX,mazeY] = mb;
		MazeManager.GetInstance().maze3DGrid.AddObjectAtPosition(mb,mb.Position);
		MazeManager.GetInstance().maze3DGrid.mazeBlockOccupied.Add(block.name,mb);


		mb.Init(blockType);
		//temp
		block.gameObject.layer = MazeManager.MAZE_BLOCK_LAYER;
	//	if(BaseBlock.IsSinkBlock(blockType)) {
		if(mazeMap.IsSinkTarget(new MazePoint(mazeX,mazeY))) {
			mb.renderer.material.color = MazeBlockEffect.GetSinkTargetEffect().color;
			//block.gameObject.layer = MazeManager.MAZE_BLOCK_LAYER;
		} else if(MazeBlock.IsSourceBlock(blockType)) {
			mb.SetBlockEffect();
		}

		return block;
	}

	abstract protected void AddDragableComponent(GameObject block);

	abstract protected  GameObject CreateBlock(int mazeX, int mazeY);

	//protected virtual Vector3 GetBlockWorldPosition(int mazeX, int mazeY) {}
	//should go to utils type framwork
	
	protected IEnumerator LoadTexture() {
		string imageFile = MazeManager.GetInstance().levelSceneData.imageFile;
		LoadDefaultMaterial(MazeManager.GetInstance().levelSceneData.mapFile);
			if(imageFile.Contains("://")) {
				yield return StartCoroutine(DownloadFromUrl(imageFile));
			} else {
				DefaultMaterial.mainTexture = Resources.Load<Texture2D>(imageFile);
			}
	//	MazeManager.GetInstance().maze3DGrid.gridWidth =DefaultMaterial.mainTexture.width/MazeManager.GetInstance().maze3DGrid.gridSizeX;
	//	MazeManager.GetInstance().maze3DGrid.gridHeight =DefaultMaterial.mainTexture.height/MazeManager.GetInstance().maze3DGrid.gridSizeY;
	}

	private void LoadDefaultMaterial(string matFile) {
		if(DefaultMaterial==null || DefaultMaterial.name!=matFile)
			DefaultMaterial =  new Material(Shader.Find("Diffuse")) ;// Resources.Load<Material>(matFile));
	}

	protected IEnumerator DownloadFromUrl(string imageFile) {
		WWW www= new WWW (imageFile);
		while(!www.isDone) {
			yield return 0;
		}
		// Wait for download to complete
		//  yield www;
		Texture2D texTmp = new Texture2D(1024, 1024, TextureFormat.DXT5, false); //LoadImageIntoTexture compresses JPGs by DXT1 and PNGs by DXT5    
		if(texTmp!=null) {
			try {
				www.LoadImageIntoTexture(texTmp);
				DefaultMaterial.mainTexture = texTmp;
			} catch (System.Exception e) {
				Debug.Log(e.ToString());
			}
		}
		yield break;
	}

}
