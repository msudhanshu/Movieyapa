	#if UNITY_EDITOR
	using UnityEngine;
	using UnityEditor;
	using System.Collections;
	
	
public class CreateRingSegmentEditor : ScriptableWizard
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
		public bool addCollider = false;
		public bool createAtOrigin = true;
		public string optionalName;
		
		static Camera cam;
		static Camera lastUsedCam;
		
		
		[MenuItem("GameObject/Create Other/Ring Segment...")]
		static void CreateWizard()
		{
			cam = Camera.current;
			// Hack because camera.current doesn't return editor camera if scene view doesn't have focus
			if (!cam)
				cam = lastUsedCam;
			else
				lastUsedCam = cam;
		ScriptableWizard.DisplayWizard("Create RingSegment",typeof(CreateRingSegment));
		}
		
		
		void OnWizardUpdate()
		{
			radiusSegments = Mathf.Clamp(radiusSegments, 1, 254);
			angleSegments = Mathf.Clamp(angleSegments, 1, 254);
		}
		
		
		void OnWizardCreate()
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
		Mesh m = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/UnlockDisk/Models/" + planeAssetName,typeof(Mesh));
			
			if (m == null)
			{
				m = new Mesh();
				m.name = plane.name;
				
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



			float radius = 100;
			//parametric coord equation of circle
			//( x + rsin , y+rcos)
			float radiusStart = 50;
			float radiusEnd = 100;
			float radiusDensity = 1.0f;
			int radiusNodeCount = (int)((radiusEnd- radiusStart)/radiusDensity);
			//Indegree
			float angleStart = 20;
			float angleEnd = 110;
			float angleDensity = 5;
			int angleNodeCount =  (int)( (angleEnd-angleStart)/angleDensity);


			//				int numTriangles = radiusSegments * angleSegments * 6;
			//				int numVertices = (radiusCount) * (angleCount); //+1 to complete the loop

			int numVertices = radiusNodeCount*angleNodeCount;
			int numTriangles = (radiusNodeCount) * (angleNodeCount) * 6;

			Vector3[] vertices = new Vector3[numVertices];
			Vector2[] uvs = new Vector2[numVertices];
			int[] triangles = new int[numTriangles];

			int index = 0;
			for (int i = 0; i < radiusNodeCount; i=i+1)
          		{	
				float r = radiusStart + i*radiusDensity;
				for (int j = 0; j < angleNodeCount; j=j+1)
					{
					float a = angleStart + j*angleDensity;
					//Setting vertex
					float x = r * Mathf.Cos(2 * Mathf.PI * a/360.0f);
					float y = r *Mathf.Sin(2 * Mathf.PI * a/360.0f);
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
					float u = (x / radius + 1 )*0.5f;
					float v = (y / radius + 1)*0.5f;
					uvs[index++] = new Vector2(u, v);
				}
			}

			int triangleIndex = 0;
			for (int i = 0; i < radiusNodeCount-1; i=i+1)
			{
				for (int j = 0; j < angleNodeCount-1; j=j+1)
				{
					//setting index
					triangles[triangleIndex]   = (i * angleNodeCount) + j;
					triangles[triangleIndex+1] = ((i+1) * angleNodeCount) + j;
					triangles[triangleIndex+2] = (i * angleNodeCount) + j + 1;
					
					triangles[triangleIndex+3] = ((i+1) * angleNodeCount) + j;
					triangles[triangleIndex+4] = ((i+1) * angleNodeCount) + j + 1;
					triangles[triangleIndex+5] = (i * angleNodeCount) + j + 1;
					triangleIndex += 6;
				}
			}

				/*
                index = 0;
                for (int y = 0; y < angleSegments; y++)
                {
                    for (int x = 0; x < radiusSegments; x++)
                    {
                        triangles[index]   = (y     * radiusCount) + x;
                        triangles[index+1] = ((y+1) * radiusCount) + x;
                        triangles[index+2] = (y     * radiusCount) + x + 1;
                        
                        triangles[index+3] = ((y+1) * radiusCount) + x;
                        triangles[index+4] = ((y+1) * radiusCount) + x + 1;
                        triangles[index+5] = (y     * radiusCount) + x + 1;
                        index += 6;
                    }
                }
                */
                m.vertices = vertices;
                m.uv = uvs;
                m.triangles = triangles;
                m.RecalculateNormals();
                
                AssetDatabase.CreateAsset(m, "Assets/UnlockDisk/Models/" + planeAssetName);
                AssetDatabase.SaveAssets();
            }
            
            meshFilter.sharedMesh = m;
            m.RecalculateBounds();
            
            if (addCollider)
                plane.AddComponent(typeof(BoxCollider));
            
            Selection.activeObject = plane;
        }
    }
    #endif