using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIBuildingPopup: UIBasePopup {
	
	public override List<Asset> GetAssetData() {
		return BuildingManager3D.GetInstance().GetAllBuildingTypes().Where(b=>!b.isObstacle && !b.isPath && b.assetCategoryEnum==AssetCategoryEnum.BUILDING).ToList();
	}
}
