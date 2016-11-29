using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KiwiCommonDatabase;

namespace Expansion
{
	public class GameBoundary : Manager<GameBoundary>
	{
		PolygonCollider2D gameBoundary;
		//List of all active edges
		List<UserExpansionEdge> allEdges;
		//A list used to store information about the edges being updated while adding an expansion block
		Dictionary<string, List<UserExpansionEdge>> updateList;

		override public void StartInit ()
		{
			IoCContainer.GetInstance ().RegisterType (typeof(IExpansionGrid), typeof(BuildingModeGrid3D));
			IoCContainer.GetInstance ().RegisterType (typeof(IExpansionResource), typeof(ResourceManager));
			CreateGameBoundary ();
			ExpansionBlock.ClearExpandedArea();
			//TestAddEdges ();
		}

		override public void PopulateDependencies() {
			dependencies = new List<ManagerDependency>();
			dependencies.Add(ManagerDependency.DATA_LOADED);
		}

		private void TestAddEdges ()
		{
			int[][][] edges = new int[6][][]{new int[4][] {
				new int[4]{20,19,20,20},
				new int[4] {
					20,
					20,
					21,
					20
				},
				new int[4] {
					21,
					20,
					21,
					19
				},
				new int[4] {
					21,
					19,
					20,
					19
				}
			},
				new int[4][] {
				new int[4]{21,20,21,19},
				new int[4]{21,20,22,20},
				new int[4] {
					22,
					20,
					22,
					19
				},
				new int[4] {
					22,
					19,
					21,
					19
				}
			},
				new int[4][] {
				new int[4]{21,19,22,19},
				new int[4]{22,19,22,18},
				new int[4] {
					22,
					18,
					21,
					18
				},
				new int[4] {
					21,
					18,
					21,
					19
				}
			},
				new int[4][] {
				new int[4]{21,18,22,18},
				new int[4]{22,18,22,17},
				new int[4] {
					22,
					17,
					21,
					17
				},
				new int[4] {
					21,
					17,
					21,
					18
				}
			},
				new int[4][] {
				new int[4]{21,17,22,17},
				new int[4]{22,17,22,16},
				new int[4] {
					22,
					16,
					21,
					16
				},
				new int[4] {
					21,
					16,
					21,
					17
				}
			},
				new int[4][] {
				new int[4]{21,16,20,16},
				new int[4]{20,16,20,17},
				new int[4] {
					20,
					17,
					21,
					17
				},
				new int[4] {
					21,
					17,
					21,
					16
				}
			}
			};
			foreach (int[][] blockEdges in edges) {
				List<UserExpansionEdge> expansionBlock = new List<UserExpansionEdge> ();
				foreach (int[] blockEdge in blockEdges) {
					expansionBlock.Add (new UserExpansionEdge (198f - ((float)blockEdge [0] - 20f) * 4f, 118f + (20f - (float)blockEdge [1]) * 4f, 198f - ((float)blockEdge [2] - 20f) * 4f, 118f + (20f - (float)blockEdge [3]) * 4f, 0));
				}
				AddBlockToPolygon (expansionBlock, 4f, 4f);
			}
		}

		//test code
		private void Test ()
		{
			Vector2[] points = gameBoundary.GetPath (0);
			Vector2[] points1 = gameBoundary.GetPath (1);
			prettyPrintEdgeList (GetAllEdges ());
			bool check = this.IsInsideBoundary (new Vector2 (220.5f, 17.8f));
			bool check2 = this.IsInsideBoundary (new Vector2 (205.5f, 7.8f));
			bool check3 = this.IsInsideBoundary (new Vector2 (220f, 19f));
		}

		private void CreateGameBoundary ()
		{
			SetAllEdges (ReIndexPaths (GetAllEdges ()));
			SortedDictionary<int, List<Vector2>> paths = UserExpansionEdge.GetIndexedPathVectors (GetAllEdges ());
			gameBoundary = this.GetComponent <PolygonCollider2D> ();
			gameBoundary.pathCount = paths.Count;
			foreach (KeyValuePair<int, List<Vector2>> indexedPath in paths) {
				int index = indexedPath.Key;
				gameBoundary.SetPath (index, indexedPath.Value.ToArray ());
			}
		}

		private List<UserExpansionEdge> GetAllEdges ()
		{
			if (allEdges == null) {
				allEdges = UserExpansionEdge.GetAllUserExpansionEdge ();
			}
			return allEdges;
		}

		private void SetAllEdges (List<UserExpansionEdge> newEdgeList)
		{
			allEdges = newEdgeList;
		}

		public void AddBlockToPolygon (List<UserExpansionEdge> blockEdges, float sizeX, float sizeZ)
		{
			if (blockEdges.Count != 4) {
				Debug.LogError ("Can't add block with <> 4 edges");
				return;
			}
			this.updateList = new Dictionary<string, List<UserExpansionEdge>> ();
			foreach (UserExpansionEdge edge in GetAllEdges()) {
				Record ("existing", edge);
			}
			//Step: Mark edges for deletion, Delete marked edges and create new ones
			List <UserExpansionEdge> newEdges = new List<UserExpansionEdge> ();
			SplitOverlappingEdges (blockEdges, newEdges);
			//Step: Remove Edges Marked for deletion
			List<UserExpansionEdge> edges = GetAllEdges ();
			edges.RemoveAll (e => e.markedForDeletion);
			prettyPrintEdgeList (edges);
			prettyPrintEdgeList (newEdges);
			//Step: Merge redundant edges
			MergeEdges (edges, newEdges);
			//Step: Update pathVectors and new shape
			CreateGameBoundary ();
			//Update Data to server
			foreach (UserExpansionEdge edge in newEdges) {
				Record ("add", edge);
			}
			SyncData (sizeX, sizeZ);
		}

		private void Record (string action, UserExpansionEdge edge)
		{
			if (!updateList.ContainsKey (action)) {
				updateList [action] = new List<UserExpansionEdge> ();
			}
			updateList [action].Add (edge);

		}

		private List<UserExpansionEdge> GetUpdateList (string action)
		{
			if (updateList.ContainsKey (action))
				return updateList [action];
			return null;
		}

		private void SyncData (float sizeX, float sizeZ)
		{
			UserExpansionEdge.Sync (GetUpdateList ("existing"), GetUpdateList ("add"), GetUpdateList ("delete"), sizeX, sizeZ);
			updateList.Clear ();

		}

		//Splitting includes listing new edges and marking old edges for deletion
		private void SplitOverlappingEdges (List<UserExpansionEdge> blockEdges, List <UserExpansionEdge> newEdges)
		{
			foreach (UserExpansionEdge blockEdge in blockEdges) {
				//Find the exising edge that contains the blockEdge which is marked for deletion(there can be only one such overlapping edge)
				UpdateOverlappingEdge (blockEdge, newEdges);
				//If block Edge is non overlapping, add it to new edges
				if (!blockEdge.markedForDeletion) {
					newEdges.Add (blockEdge);
				}
			}
		}

		private void UpdateOverlappingEdge (UserExpansionEdge blockEdge, List<UserExpansionEdge> newEdges)
		{
			//If blockEdge Vertex doesn't exists then delete the old edge and add new edge to this new EndPoint
			foreach (UserExpansionEdge pathEdge in GetAllEdges()) {
				if (pathEdge.IsOverlappingEdge (blockEdge)) {
					blockEdge.markedForDeletion = true;
					pathEdge.markedForDeletion = true;
					pathEdge.Reorder ();
					blockEdge.Reorder ();
					if (pathEdge.x != blockEdge.x || pathEdge.y != blockEdge.y) {
						newEdges.Add (new UserExpansionEdge (pathEdge.x, pathEdge.y, blockEdge.x, blockEdge.y, pathEdge.pathIndex));
					}
					if (pathEdge.x1 != blockEdge.x1 || pathEdge.y1 != blockEdge.y1) {
						newEdges.Add (new UserExpansionEdge (pathEdge.x1, pathEdge.y1, blockEdge.x1, blockEdge.y1, pathEdge.pathIndex));
					}
					Record ("delete", pathEdge);
					return;
				}
			}
		}

		private void prettyPrintEdgeList (List <UserExpansionEdge> edges)
		{
			string cd = "Coordinate list ---- ";
			foreach (UserExpansionEdge edge in edges) {
				cd = cd + "--[ " + edge.pathIndex + " : (" + edge.x + ", " + edge.y + "),(" + edge.x1 + "," + edge.y1 + ")]--";
			}
			Debug.Log (cd);
		}

		private void MergeEdges (List <UserExpansionEdge> allEdges, List <UserExpansionEdge> newEdges)
		{
			allEdges.AddRange (newEdges);
			//index paths and update polygon
			List<UserExpansionEdge> indexedPaths = ReIndexPaths (allEdges);

			//Merge edges lying in a straight line, each edge is visited once only
			int pathCount = indexedPaths.Count;
			for (int i = 0; i < pathCount-1; i++) {
				if ((indexedPaths [i]).AlignsWith (indexedPaths [i + 1])) {
					UserExpansionEdge mergedEdge = indexedPaths [i].Merge (indexedPaths [i + 1]);
					foreach (int j in new int[2]{i , i+1}) {
						if (indexedPaths [j].id > 0)
							Record ("delete", indexedPaths [j]);
						else
							newEdges.Remove (indexedPaths [j]);
					}
					newEdges.Add (mergedEdge);
					indexedPaths [i] = mergedEdge;
					indexedPaths.RemoveAt (i + 1);
					pathCount--;

				}
			}
			SetAllEdges (indexedPaths);
		}

		private List<UserExpansionEdge> ReIndexPaths (List<UserExpansionEdge> allEdges)
		{
			List<UserExpansionEdge> indexedEdges = new List<UserExpansionEdge> ();
			UserExpansionEdge currentEdge = null, startEdge = null, nextEdge = null;
			int index = -1;
			while (allEdges.Count() > 0) {
				if (currentEdge == null) {
					index++;
					currentEdge = GetFirstBoundaryEdge (allEdges);
					startEdge = currentEdge;
				}
				currentEdge.pathIndex = index;
				allEdges.Remove (currentEdge);
				indexedEdges.Add (currentEdge);
				nextEdge = GetNextEdge (currentEdge, startEdge, allEdges);
				if (nextEdge != null) {
					currentEdge.ReorderWith (nextEdge);
				}
				currentEdge = nextEdge;
			}

			return indexedEdges;
		}

		private UserExpansionEdge GetNextEdge (UserExpansionEdge currentEdge, UserExpansionEdge startEdge, List<UserExpansionEdge> edges)
		{
			UserExpansionEdge nextEdge = null;
			foreach (UserExpansionEdge edge in edges) {
				if (edge.HasSharedVertex (currentEdge)) {
					nextEdge = edge;
					break;
				}
			}
			if ((nextEdge == null) && !startEdge.HasSharedVertex (currentEdge)) {
				throw new Exception ("Loop closing error while creating boundary");
			}
			return nextEdge;
		}

		private UserExpansionEdge GetFirstBoundaryEdge (List<UserExpansionEdge> edges)
		{
			if (edges.Count == 0)
				return null;
			UserExpansionEdge minXEdge = edges [0];
			foreach (UserExpansionEdge edge in edges) {
				if (edge.LowX () < minXEdge.LowX ()) {
					minXEdge = edge;
				}
			}
			return minXEdge;
		}

		public bool IsInsideBoundary (Vector2 point)
		{
			return this.gameBoundary.OverlapPoint (point);
		}

	}
}

