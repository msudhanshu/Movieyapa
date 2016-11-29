using System;
using UnityEngine;
namespace Expansion
{
	public interface IExpansionGrid{
		void BeforeExpansion(Vector3 center);
		void AfterExpansion(Vector3 center);
		void OccupyGrid(Vector3 position, float x,  float y, float z);
		bool IsInExpansionBlock(Vector2 gridCoordinates, Vector3 worldCoordinates);
	}
}

