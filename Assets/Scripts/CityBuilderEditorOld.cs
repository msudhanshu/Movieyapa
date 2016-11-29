using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CityBuilderEditor : MonoBehaviour
{
 
    public Transform[] blocks;
    public Transform[] bases;
    public Transform[] floors;
    public Transform[] roofs;

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

    void Start()
    {
        //StartCoroutine(Generate());
    }

	void Update() {
		if (BuildButton) {
			//delete old and create new

			BuildButton = false;
			if(cityRoot)
				Destroy(cityRoot);
			StartCoroutine(Generate());
				}
	}

    IEnumerator Generate()
    {
        building = true;

        if (cityRoot)
            Destroy(cityRoot);

        cityRoot = new GameObject("CityRoot");

        float blockXSize = buildingXNum * buildingSpan;
        float blockZSize = buildingZNum * buildingSpan;

        float blockXOffset = (((blockXNum - 1) * blockSpan) + ((blockXNum - 1) * blockXSize)) * 0.5f;
        float blockZOffset = (((blockZNum - 1) * blockSpan) + ((blockZNum - 1) * blockZSize)) * 0.5f;

        Vector3 blockPos = new Vector3(-blockXOffset, 0, -blockZOffset);

        for (int x = 0; x < blockXNum; ++x)
        {
            for (int z = 0; z < blockZNum; ++z)
            {
                CreateBlock(blockPos);
                blockPos.z += blockZSize + blockSpan;
				//yield return new WaitForSeconds(0);//new WaitForEndOfFrame();
            }

            blockPos.x += blockXSize + blockSpan;
            blockPos.z = -blockZOffset;
        }

        building = false;
		yield return 0;
    }

    void CreateBlock(Vector3 position)
    {
        Transform block = null;

        if (blocks.Length > 0)
        {
            block = (Transform)Instantiate(blocks[Random.Range(0,blocks.Length)], position, Quaternion.identity);
            block.name = "Block" + ++numBlocks;
        }
        else
        {
            block = new GameObject("Block" + ++numBlocks).transform;
            block.position = position;
        }
        
        block.parent = cityRoot.transform;

        float blockXScale = ((buildingXNum * buildingSpan) + blockSpan) * 0.1f;
        float blockZScale = ((buildingZNum * buildingSpan) + blockSpan) * 0.1f;

        block.localScale = new Vector3(blockXScale, 1, blockZScale);

        float buildingXOffset = ((buildingXNum - 1) * buildingSpan) * 0.5f;
        float buildingZOffset = ((buildingZNum - 1) * buildingSpan) * 0.5f;

        Vector3 buildingPos = new Vector3(position.x - buildingXOffset, 0, position.z - buildingZOffset);

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

        block.gameObject.AddComponent<CombineChildren>();
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

            if (GUILayout.Button("Generate"))
                StartCoroutine(Generate());
        }
    }
}
