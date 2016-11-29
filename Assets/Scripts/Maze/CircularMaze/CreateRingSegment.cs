using UnityEngine;
using System.Collections;


public class CreateRingSegment 
{
	
	public enum Orientation
	{
		Horizontal,
		Vertical
	}
	
	public enum AnchorPoint
	{
		TopLeft,
		TopHalf,
		TopRight,
		RightHalf,
		BottomRight,
		BottomHalf,
		BottomLeft,
		LeftHalf,
		Center
	}
	
	public int radiusSegments = 1;
	public int angleSegments = 1;
	public float width = 1.0f;
	public float length = 1.0f;
	public Orientation orientation = Orientation.Horizontal;
	public AnchorPoint anchor = AnchorPoint.Center;
	public bool addCollider = true;
	public bool createAtOrigin = true;
	public string optionalName;
	
	static Camera cam;
	static Camera lastUsedCam;

	void OnWizardUpdate()
	{
		radiusSegments = Mathf.Clamp(radiusSegments, 1, 254);
		angleSegments = Mathf.Clamp(angleSegments, 1, 254);
	}

	public GameObject Create()
    {
		return Create(new RingSegmentParam());
	}
	
	public GameObject Create(RingSegmentParam param)
	{
		GameObject plane = new GameObject();
		
		if (!string.IsNullOrEmpty(optionalName))
			plane.name = optionalName;
		else
			plane.name = "Plane";
		
		if (!createAtOrigin && cam)
			plane.transform.position = cam.transform.position + cam.transform.forward*5.0f;
		else
			plane.transform.position = Vector3.zero;
		
		Vector2 anchorOffset;
		string anchorId;
		switch (anchor)
		{
		case AnchorPoint.TopLeft:
			anchorOffset = new Vector2(-width/2.0f,length/2.0f);
			anchorId = "TL";
			break;
		case AnchorPoint.TopHalf:
			anchorOffset = new Vector2(0.0f,length/2.0f);
			anchorId = "TH";
			break;
		case AnchorPoint.TopRight:
			anchorOffset = new Vector2(width/2.0f,length/2.0f);
			anchorId = "TR";
			break;
		case AnchorPoint.RightHalf:
			anchorOffset = new Vector2(width/2.0f,0.0f);
			anchorId = "RH";
			break;
		case AnchorPoint.BottomRight:
			anchorOffset = new Vector2(width/2.0f,-length/2.0f);
			anchorId = "BR";
			break;
		case AnchorPoint.BottomHalf:
			anchorOffset = new Vector2(0.0f,-length/2.0f);
			anchorId = "BH";
			break;
		case AnchorPoint.BottomLeft:
			anchorOffset = new Vector2(-width/2.0f,-length/2.0f);
			anchorId = "BL";
			break;			
		case AnchorPoint.LeftHalf:
			anchorOffset = new Vector2(-width/2.0f,0.0f);
			anchorId = "LH";
			break;			
		case AnchorPoint.Center:
		default:
			anchorOffset = Vector2.zero;
			anchorId = "C";
			break;
		}
		
		MeshFilter meshFilter = (MeshFilter)plane.AddComponent(typeof(MeshFilter));
		plane.AddComponent(typeof(MeshRenderer));
		
	string planeAssetName = plane.name + radiusSegments + "x" + angleSegments + "W" + width + "L" + length + (orientation == Orientation.Horizontal? "H" : "V") + anchorId + ".asset";
    
		CreateMesh(plane,param);
		CreateMeshCollider(plane,param);
  
    
    //Selection.activeObject = plane;
	return plane;
	}


	public void CreateMeshCollider(GameObject go, RingSegmentParam param) {
		if(go.GetComponent<BoxCollider>() !=null) {
			Object.Destroy(go.GetComponent<Collider>());
		}

		if (addCollider)
			go.AddComponent(typeof(BoxCollider));

		return;

		Mesh m = null;//(Mesh)AssetDatabase.LoadAssetAtPath("Assets/UnlockDisk/Models/" + planeAssetName,typeof(Mesh));
		
		if (m == null)
		{
			m = new Mesh();
			m.name = "plane.name";
			
			
			int radiusCount = radiusSegments+1;
			int angleCount = angleSegments+1;
			
			//				int numTriangles = radiusSegments * angleSegments * 6;
			//				int numVertices = (radiusCount) * (angleCount); //+1 to complete the loop
			//				Vector3[] vertices = new Vector3[numVertices];
			//				Vector2[] uvs = new Vector2[numVertices];
			//				int[] triangles = new int[numTriangles];
			
			
			float uvFactorX = 1.0f/radiusSegments;
			float uvFactorY = 1.0f/angleSegments;
			float scaleX = width/radiusSegments;
			float scaleY = length/angleSegments;
			
			
			
			
			
			Vector3[] vertices = new Vector3[param.numVertices];
			Vector2[] uvs = new Vector2[param.numVertices];
			int[] triangles = new int[param.numTriangles];
			
			int index = 0;
			for (int i = 0; i < param.radiusNodeCount; i=i+1)
			{
				float r = param.radiusStart + i*param.radiusDensity;
				for (int j = 0; j < param.angleNodeCount; j=j+1)
				{
					float a = param.angleStart + j*param.angleDensity;
					//Setting vertex
					float x = r * Mathf.Cos(Mathf.Deg2Rad * a);
					float y = r * Mathf.Sin(Mathf.Deg2Rad * a);
					if (true || orientation == Orientation.Horizontal)
					{
						vertices[index] = new Vector3(x, 0.0f,y);
					}
					else
					{
						//	vertices[index] = new Vector3(radius*r/radiusCount*Mathf.Sin(2 * Mathf.PI * a/angleCount), 0.0f, radius*r/radiusCount*Mathf.Sin(2 * Mathf.PI * a/angleCount));
					}
					
		
					index++;
				}
			}
			
			int triangleIndex = 0;
			for (int i = 0; i < param.radiusNodeCount-1; i=i+1)
			{
				for (int j = 0; j < param.angleNodeCount-1; j=j+1)
				{
					//setting index
					triangles[triangleIndex]   = (i * param.angleNodeCount) + j;
					triangles[triangleIndex+2] = ((i+1) * param.angleNodeCount) + j;
					triangles[triangleIndex+1] = (i * param.angleNodeCount) + j + 1;
					
					triangles[triangleIndex+3] = ((i+1) * param.angleNodeCount) + j;
					triangles[triangleIndex+5] = ((i+1) * param.angleNodeCount) + j + 1;
					triangles[triangleIndex+4] = (i * param.angleNodeCount) + j + 1;
					triangleIndex += 6;
				}
			}
			
			m.vertices = vertices;
			m.uv = uvs;
			m.triangles = triangles;
			m.RecalculateNormals();
			
			//  AssetDatabase.CreateAsset(m, "Assets/UnlockDisk/Models/" + planeAssetName);
			//  AssetDatabase.SaveAssets();
		}
		m.RecalculateBounds();
		
		
		MeshFilter meshFilter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
		meshFilter.sharedMesh = m;
		return;
	}


	public void CreateMesh(GameObject go, RingSegmentParam param, RingSegmentParam uvParam=null) {
		Mesh m = null;//(Mesh)AssetDatabase.LoadAssetAtPath("Assets/UnlockDisk/Models/" + planeAssetName,typeof(Mesh));
		
		if (m == null)
		{
			m = new Mesh();
			m.name = "plane.name";


			int radiusCount = radiusSegments+1;
			int angleCount = angleSegments+1;
			
			//				int numTriangles = radiusSegments * angleSegments * 6;
			//				int numVertices = (radiusCount) * (angleCount); //+1 to complete the loop
			//				Vector3[] vertices = new Vector3[numVertices];
			//				Vector2[] uvs = new Vector2[numVertices];
			//				int[] triangles = new int[numTriangles];
			
			
			float uvFactorX = 1.0f/radiusSegments;
			float uvFactorY = 1.0f/angleSegments;
			float scaleX = width/radiusSegments;
			float scaleY = length/angleSegments;
			
			
			
			
			
			Vector3[] vertices = new Vector3[param.numVertices];
			Vector2[] uvs = new Vector2[param.numVertices];
			int[] triangles = new int[param.numTriangles];
			
			int index = 0;
			for (int i = 0; i < param.radiusNodeCount; i=i+1)
			{
				float r = param.radiusStart + i*param.radiusDensity;
				for (int j = 0; j < param.angleNodeCount; j=j+1)
				{
					float a = param.angleStart + j*param.angleDensity;
					//Setting vertex
					float x = r * Mathf.Cos(Mathf.Deg2Rad * a);
					float y = r * Mathf.Sin(Mathf.Deg2Rad * a);
					if (true || orientation == Orientation.Horizontal)
					{
						vertices[index] = new Vector3(x, 0.0f,y);
					}
					else
					{
						//	vertices[index] = new Vector3(radius*r/radiusCount*Mathf.Sin(2 * Mathf.PI * a/angleCount), 0.0f, radius*r/radiusCount*Mathf.Sin(2 * Mathf.PI * a/angleCount));
					}
					
					//Setting UV
					//for U => (x,y)=(u,v) : (0,0)=(0.5,0.5) , (radius,0), (1,0.5)
					if(uvParam==null) {
						float u = (x / param.radius + 1 )*0.5f;
						float v = (y / param.radius + 1)*0.5f;
						uvs[index] = new Vector2(u, v);
					}

					index++;
				}
			}


			if(uvParam!=null) {
				index = 0;
				for (int i = 0; i < param.radiusNodeCount; i=i+1)
				{
					float r = uvParam.radiusStart + i*param.radiusDensity;
					for (int j = 0; j < param.angleNodeCount; j=j+1)
					{
						float a = uvParam.angleStart + j*param.angleDensity;
						//Setting vertex
						float x = r * Mathf.Cos(Mathf.Deg2Rad * a);
						float y = r * Mathf.Sin(Mathf.Deg2Rad * a);
			
	                    //Setting UV
	                    //for U => (x,y)=(u,v) : (0,0)=(0.5,0.5) , (radius,0), (1,0.5)
						float u = (x / uvParam.radius + 1 )*0.5f;
						float v = (y / uvParam.radius + 1)*0.5f;
                        uvs[index++] = new Vector2(u, v);
	                }
	            }
			}

            int triangleIndex = 0;
			for (int i = 0; i < param.radiusNodeCount-1; i=i+1)
			{
				for (int j = 0; j < param.angleNodeCount-1; j=j+1)
				{
                    //setting index
                    triangles[triangleIndex]   = (i * param.angleNodeCount) + j;
                    triangles[triangleIndex+2] = ((i+1) * param.angleNodeCount) + j;
                    triangles[triangleIndex+1] = (i * param.angleNodeCount) + j + 1;
                    
                    triangles[triangleIndex+3] = ((i+1) * param.angleNodeCount) + j;
                    triangles[triangleIndex+5] = ((i+1) * param.angleNodeCount) + j + 1;
                    triangles[triangleIndex+4] = (i * param.angleNodeCount) + j + 1;
                    triangleIndex += 6;
                }
            }
            
            m.vertices = vertices;
            m.uv = uvs;
            m.triangles = triangles;
            m.RecalculateNormals();
            
            //  AssetDatabase.CreateAsset(m, "Assets/UnlockDisk/Models/" + planeAssetName);
			//  AssetDatabase.SaveAssets();
        }
		m.RecalculateBounds();

		
		 MeshFilter meshFilter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
		 meshFilter.sharedMesh = m;
		return;
    }

}


public class RingSegmentParam {
//	public bool updateUV = true;
	public float radius = 100;
	//parametric coord equation of circle
	//( x + rsin , y+rcos)
	public float radiusStart = 50;
	public float radiusEnd = 100;
	public float radiusDensity = 1.0f;
	public int radiusNodeCount {get { return (int)((radiusEnd-radiusStart)/radiusDensity) + 1;}}
	//Indegree
	public float angleStart = 20;
	public float angleEnd = 110;
	public float angleDensity = 5;
	public int angleNodeCount  {get { return  (int)( (angleEnd-angleStart)/angleDensity) + 1; }}
	
	
	//				int numTriangles = radiusSegments * angleSegments * 6;
	//				int numVertices = (radiusCount) * (angleCount); //+1 to complete the loop
	
	public int numVertices {get { return radiusNodeCount*angleNodeCount;  }}
	public int numTriangles {get { return  (radiusNodeCount) * (angleNodeCount) * 6;  }}
}