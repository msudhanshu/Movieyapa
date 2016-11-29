using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Expansion
{
	public class UserExpansionEdge
	{
		public static List<UserExpansionEdge> userExpansionEdges = null;

		public int id { get; set; }

		public float x { get; set; }

		public float y { get; set; }

		public float x1{ get; set; }

		public float y1 { get; set; }

		public string location { get; set; }

		public int pathIndex{ get; set; }
	
		public bool markedForDeletion{ get; set; }

		public UserExpansionEdge ()
		{
		}

		public static void Init (List<UserExpansionEdge> edges)
		{
			userExpansionEdges = edges;
		}

		public UserExpansionEdge (float x, float y, float x1, float y1, int pathIndex)
		{
			this.x = x;
			this.y = y;
			this.x1 = x1;
			this.y1 = y1;
			this.pathIndex = pathIndex;
		}

		public static List<UserExpansionEdge> GetAllUserExpansionEdge ()
		{
			/*if (userExpansionEdges == null) {
				userExpansionEdges = new List<UserExpansionEdge> ();
				int[][] edges = new int[4][] {
				new int[4]{198,213,198,249},
				new int[4] {
					198,
					249,
					235,
					249
				},
				new int[4] {
					235,
					249,
					235,
					213
				},
				new int[4] {
					235,
					213,
					198,
					213
				}
			};
				int index = 1;
				foreach (int[] blockEdge in edges) {
					UserExpansionEdge edge = new UserExpansionEdge ((float)blockEdge [0], (float)blockEdge [1], (float)blockEdge [2], (float)blockEdge [3], 0);
					edge.id = index;
					userExpansionEdges.Add (edge);
					index++;
				}
			}*/
			return userExpansionEdges;
		}
		//Edges passed here should be in a cyclic order to render the shape correctly
		public static SortedDictionary<int, List<Vector2>> GetIndexedPathVectors (List<UserExpansionEdge> edges)
		{
			SortedDictionary<int, List<Vector2>> paths = new SortedDictionary<int, List<Vector2>> ();
			foreach (UserExpansionEdge edge in edges) {
				if (!paths.ContainsKey (edge.pathIndex)) {
					paths.Add (edge.pathIndex, new List<Vector2> ());
				}
				edge.AddToVertexList (paths [edge.pathIndex]);
			}
			return paths;
		}

		private void AddToVertexList (List<Vector2> vertices)
		{
			float x, y;
			if (vertices.FindIndex (e => e.x == this.x && e.y == this.y) == -1) {
				x = this.x;
				y = this.y;
				vertices.Add (new Vector2 (x, y));
			}
			if (vertices.FindIndex (e => e.x == this.x1 && e.y == this.y1) == -1) {
				x = this.x1;
				y = this.y1;
				vertices.Add (new Vector2 (x, y));
			}
		}

		public List<float> GetSharedVertex (UserExpansionEdge edge)
		{
			List<float> shared = new List<float> ();
			if ((this.x == edge.x && this.y == edge.y) || (this.x == edge.x1 && this.y == edge.y1)) {
				shared.Add (x);
				shared.Add (y);
			}

			if ((this.x1 == edge.x && this.y1 == edge.y) || (this.x1 == edge.x1 && this.y1 == edge.y1)) {
				shared.Add (this.x1);
				shared.Add (this.y1);
			}
			return shared;
		}
		//Orders s.t x1y1 of this edge is shared with xy of next edge
		public void ReorderWith (UserExpansionEdge next)
		{
			List<float> sharedPoint = this.GetSharedVertex (next);
			if (sharedPoint.Count > 0) {
				if (this.x == sharedPoint [0] && this.y == sharedPoint [1]) {
					this.SwapDirection ();
				}
				if (next.x1 == sharedPoint [0] && next.y1 == sharedPoint [1]) {
					next.SwapDirection ();
				}
			}
		}

		private void SwapDirection ()
		{
			SwapX ();
			SwapY ();
		}

		private void SwapX ()
		{
			float temp = this.x1;
			this.x1 = this.x;
			this.x = temp;
		}

		private void SwapY ()
		{
			float temp = this.y1;
			this.y1 = this.y;
			this.y = temp;
		}

		//Orders edge suct that x1,y1 > x,y
		public void Reorder ()
		{
			if (this.InYDirection ()) {
				if (this.y > this.y1) {
					this.SwapY ();
				}
			} else {
				if (this.x > this.x1) {
					this.SwapX ();
				}
			}

		}

		public bool IsOverlappingEdge (UserExpansionEdge testEdge)
		{
			if (this.InYDirection () && testEdge.InYDirection () && this.x == testEdge.x) {
				return (this.LowY () <= testEdge.LowY () && this.HighY () >= testEdge.HighY ());
			} else if (this.InXDirection () && testEdge.InXDirection () && this.y == testEdge.y) {
				return (this.LowX () <= testEdge.LowX () && this.HighX () >= testEdge.HighX ());
			}
			return false;
		}

		public bool AlignsWith (UserExpansionEdge edge)
		{
			return this.HasSharedVertex (edge) && ((this.InXDirection () && edge.InXDirection ()) ||
				this.InYDirection () && edge.InYDirection ());
		}

		public bool HasSharedVertex (UserExpansionEdge edge)
		{
			return (this.GetSharedVertex (edge).Count > 0);
		}

		public UserExpansionEdge Merge (UserExpansionEdge edge)
		{
			float high, low = 0;
			UserExpansionEdge mergeEdge = (UserExpansionEdge)this.MemberwiseClone ();
			mergeEdge.id = 0;
			if (this.InXDirection ()) {
				high = Math.Max (this.HighX (), edge.HighX ());
				low = Math.Min (this.LowX (), edge.LowX ());
				mergeEdge.x = low;
				mergeEdge.x1 = high;
			} else {
				high = Math.Max (this.HighY (), edge.HighY ());
				low = Math.Min (this.LowY (), edge.LowY ());
				mergeEdge.y = low;
				mergeEdge.y1 = high;
			}
			return mergeEdge;
		}

		private bool InYDirection ()
		{
			return this.x == this.x1;
		}

		private bool InXDirection ()
		{
			return this.y == this.y1;
		}

		public int LowX ()
		{
			return Math.Min ((int)this.x, (int)this.x1);
		}

		public int LowY ()
		{
			return Math.Min ((int)this.y, (int)this.y1);
		}

		public int HighX ()
		{
			return Math.Max ((int)this.x, (int)this.x1);
		}
	
		public int HighY ()
		{
			return Math.Max ((int)this.y, (int)this.y1);
		}

		private static string EDGE_DELIMITER = ":";
		private static string COORDINATE_DELIMITER = "_";

		public static string GetString (List<UserExpansionEdge> edges)
		{
			if (edges == null)
				return "";
			return edges.Select (edge => edge.ToString ()).Aggregate ((curr, next) => curr + EDGE_DELIMITER + next);
		}

		public override string ToString ()
		{
			return this.x + COORDINATE_DELIMITER + this.y + COORDINATE_DELIMITER + this.x1 + COORDINATE_DELIMITER + this.y1;
		}

		public static void Sync (List<UserExpansionEdge> existingEdges, List<UserExpansionEdge> newEdges, List<UserExpansionEdge> deletedEdges, float sizeX, float sizeZ)
		{
			string urlString = UserExpansionActionURL (existingEdges, newEdges, deletedEdges, sizeX + "*" + sizeZ, sizeX * sizeZ);
			Debug.LogWarning ("Expansion Url update: " + urlString);
			ServerAction.takeAction (EAction.EXPANSION, urlString, new GenericServerNotifier (), false);
		}

		private static string UserExpansionActionURL (List<UserExpansionEdge> existingEdges, List<UserExpansionEdge> addedList, List<UserExpansionEdge> deletedList
	                                             , String dimension, float tilesUnlocked)
		{
			String url = "";
			string existingString = GetString (existingEdges);
			string addedString = GetString (addedList);
			string deletedString = GetString (deletedList);
		
			url = "user_id=" + Config.USER_ID + "&payer_flag=0&level_xp=0&dimension=" + dimension + "&tiles_unlocked=" + tilesUnlocked +
				"&utm_source=utmSource&utm_campaign=utmCampaign&utm_medium=utmMedium&utm_content=utmContent&is_first_time=0&location=default"
				+ "&existing=" + existingString + "&max_integer_value=" + ServerConfig.MAX_INTEGER_VALUE;
			if (deletedString.Length >= 1) {
				url += "&deleted=" + deletedString;
			}	
			if (addedString.Length >= 1) {
				url += "&added=" + addedString;
			}
			url += "&url_timestamp=" + System.DateTime.Now.ToString ("yyyyMMddHHmmssffff");
			return url;
		}	
	}
}