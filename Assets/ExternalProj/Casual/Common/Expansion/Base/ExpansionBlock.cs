using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
namespace Expansion
{
	public class ExpansionBlock  : MonoBehaviour, IPointerClickHandler 
	{

		public static ExpansionBlock ActiveExpansion = null;
		private string EXPANSION_OBSTACLE_TAG = "EXPANSION_OBSTACLE";
		private IExpansionResource expansionResource = null;
		private IExpansionGrid expansionGrid = null;

		private BoxCollider _boxCollider;
		
		protected BoxCollider boxCollider {
			get {
				if (_boxCollider == null) {
					_boxCollider = gameObject.GetComponent<BoxCollider>();
					if(_boxCollider == null)
						_boxCollider = gameObject.AddComponent<BoxCollider> ();
				}
				return _boxCollider;
			}
		}

		public void OnPointerClick (PointerEventData eventData){
			ActiveExpansion = this;
			PopupManager.GetInstance().ShowPanel(PanelType.EXPAND);
		//	this.RequestExpansion ();
		}
		/*
		void OnDrawGizmos() {
			if (boxCollider != null) {
				// Draw a yellow sphere at the transform's position
				Gizmos.color = Color.green;
				Gizmos.DrawWireCube( transform.position + boxCollider.center , boxCollider.size);
			}
		}
		*/
		public static void ClearExpandedArea(){
			ExpansionBlock[] blocks = FindObjectsOfType(typeof(ExpansionBlock)) as ExpansionBlock[];
			foreach (ExpansionBlock block in blocks) {
				if(GameBoundary.GetInstance().IsInsideBoundary(new Vector2(block.boxCollider.transform.position.x, block.boxCollider.transform.position.z))){
					block.ClearTaggedObstacles ();
					Destroy (block.gameObject);
				}
				else{
					//Occupy grid
					block.GetExpansionGrid ().OccupyGrid (block.GetWorldPosition(), block.boxCollider.size.x, block.boxCollider.size.y, block.boxCollider.size.z);
				}
			}
		}

		private IExpansionGrid GetExpansionGrid(){
			if(expansionGrid == null)
				expansionGrid = IoCContainer.GetInstance ().GetImpl<IExpansionGrid> ();
			return expansionGrid;
		}

		private IExpansionResource GetExpansionResource(){
			if(expansionResource == null)
				expansionResource = IoCContainer.GetInstance ().GetImpl<IExpansionResource> ();
			return expansionResource;
		}
		public void RequestExpansion(){
			//Deduct Cost
			if (GetExpansionResource() != null && !GetExpansionResource().ChargeExpansionCost())
				return;
			//Notify Grid
			if(GetExpansionGrid() != null)
				GetExpansionGrid().BeforeExpansion (GetWorldPosition());

		}

		public static void ClearAt(Vector2 gridPosition){
			ExpansionBlock block = ExpansionBlock.GetBlockAtPosition (gridPosition);
			block.OnClear ();
		}

		public static ExpansionBlock GetBlockAtPosition (Vector2 gridPosition){
			ExpansionBlock[] blocks = FindObjectsOfType(typeof(ExpansionBlock)) as ExpansionBlock[];
			foreach (ExpansionBlock block in blocks) {
				if(block.GetExpansionGrid().IsInExpansionBlock(gridPosition, block.boxCollider.transform.position))
					return block;
			}
			return null;
		}

		public void OnClear(){
			//Clear the objects on the expansion block
			ClearTaggedObstacles ();
			//Recreate Game boundary
			UpdateGameBoundary ();
			//Destroy this Gameobject
			Destroy (this.gameObject);
		}

		//Clear all the Obstacles tagged with EXPANSION_OBSTACLE
		//The obstacle centre should be inside the collider of expansion block.
		private void ClearTaggedObstacles(){
			BoxCollider collider = this.GetComponent<BoxCollider> ();
			GameObject[] gos;
			gos = GameObject.FindGameObjectsWithTag(EXPANSION_OBSTACLE_TAG);
			foreach (GameObject go in gos) {
				Vector3 distance = go.transform.position - collider.transform.position;
				if(Math.Abs(distance.x) < collider.size.x && Math.Abs(distance.z) < collider.size.z){
					Destroy(go);
				}
			}
		}

		private Vector3 GetWorldPosition(){
			BoxCollider collider = this.boxCollider;
			float deltaX = collider.size.x / 2;
			float deltaY = collider.size.z / 2;
			return new Vector3(collider.transform.position.x, collider.transform.position.y, collider.transform.position.z);
		}

		private void UpdateGameBoundary(){
			BoxCollider collider = this.GetComponent<BoxCollider> ();
			float deltaX = collider.size.x / 2;
			float deltaY = collider.size.z / 2;
			Vector2 center = new Vector2 (collider.transform.position.x, collider.transform.position.z);
			List<UserExpansionEdge> edges = new List<UserExpansionEdge> ();
			edges.Add (new UserExpansionEdge (center.x - deltaX, center.y - deltaY, center.x - deltaX, center.y + deltaY, 0));
			edges.Add (new UserExpansionEdge (center.x - deltaX, center.y + deltaY, center.x + deltaX, center.y + deltaY, 0));
			edges.Add (new UserExpansionEdge (center.x + deltaX, center.y + deltaY, center.x + deltaX, center.y - deltaY, 0));
			edges.Add (new UserExpansionEdge (center.x + deltaX, center.y - deltaY, center.x - deltaX, center.y - deltaY, 0));
			GameBoundary.GetInstance().AddBlockToPolygon( edges, collider.size.x, collider.size.z);
		}
	}
}