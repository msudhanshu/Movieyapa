using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;

/**
 * Panel for selling a building.
 */ 
public class UIBuildingCompletePanel: UIGamePanel {

	public Text title;
	private List<UIBuildingCompleteItem> uiBuildingCompleteItems;
	public GameObject buildingCompleteItemPrefab;
	public ScrollRect scrollRect;
	public GridLayoutGroup gridLayoutGroup;
	public bool horizontal;
	public int rowColCount;
	private Building building;
	private AssetState assetState;

	public Button completeBuildingButton;

	public void InitialiseWithData(Building userAsset, AssetState assetState) {
		this.building = userAsset;
		this.assetState = this.assetState;
	}

	virtual protected void PopulateData() {

		if (this.assetState == null) {
			this.assetState = DatabaseManager.GetAssetState(212);
		}

		if(uiBuildingCompleteItems==null)
			uiBuildingCompleteItems = new List<UIBuildingCompleteItem>();
		ClearAllItems();

		title.text = "Complete " + this.assetState.asset.name;
		int i = 0;

		int level = this.building.BuildingData == null ? 1 : this.building.BuildingData.level;
		List<AssetStateCollectable> assetStateCollectables = this.assetState.GetAllCollectables (level);


		bool collectableSufficient = true;

		foreach(AssetStateCollectable assetStateCollectable in assetStateCollectables) {
			Transform go = CreateItemPrefab(buildingCompleteItemPrefab, i);
			UIBuildingCompleteItem panel = go.GetComponent<UIBuildingCompleteItem>();
			panel.InitialiseWithData(assetStateCollectable);
			uiBuildingCompleteItems.Add (panel);

			collectableSufficient = collectableSufficient && panel.collectableSufficient;

			i++;
		}

		if (collectableSufficient) 
			completeBuildingButton.enabled = true;
		else
			completeBuildingButton.enabled = false;

		ResizeGridLayout(i);
	}

	private void ClearAllItems() {
		if(uiBuildingCompleteItems!=null) {
			foreach(UIBuildingCompleteItem item in uiBuildingCompleteItems) {
				Pool.Destroy(item.gameObject);
			}
			uiBuildingCompleteItems.Clear();
		}
	}

	protected Transform CreateItemPrefab(GameObject prefab, int i) {
		Transform go =  Pool.Instantiate(prefab) as Transform;
		if (go != null && gridLayoutGroup != null)
		{
			RectTransform t = go.GetComponent<RectTransform>();
			t.SetParent(gridLayoutGroup.transform);
			t.SetSiblingIndex(i);
			//go.gameObject.layer = gridLayoutGroup.gameObject.layer;
		}
		return go;
	}
	
	public void ResizeGridLayout(int totalItem) {
		RectTransform t = gridLayoutGroup.GetComponent<RectTransform>();
		if(horizontal) {
			scrollRect.horizontal = true;
			scrollRect.vertical = false;
			gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Vertical;
			t.sizeDelta = new Vector2( (float)( Mathf.Ceil((float)totalItem/(float)rowColCount) )* (gridLayoutGroup.cellSize.x+gridLayoutGroup.spacing.x), 
			                          rowColCount*(gridLayoutGroup.cellSize.y+gridLayoutGroup.spacing.y) );
		} else {
			scrollRect.horizontal = false;
			scrollRect.vertical = true;
			gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
			t.sizeDelta = new Vector2(rowColCount * (gridLayoutGroup.cellSize.x+gridLayoutGroup.spacing.x), (float)( Mathf.Ceil((float)totalItem/(float)rowColCount) )*(gridLayoutGroup.cellSize.y+gridLayoutGroup.spacing.y));  
		}
		//TODO : CASE WHEN ONE WANTS TO SCROLL BOTH THE WAY....  
	}

	override public void Show() {
		if (activePanel == null || activePanel.panelType == openFromPanelOnly || openFromPanelOnly == PanelType.NONE) {
			if (activePanel != null) activePanel.Hide ();
			StartCoroutine(DoShow());
			activePanel = this;
		}
	}

	override public void Hide() {
		StartCoroutine(DoHide());
	}
	
	new protected IEnumerator DoShow() {
	//	UpdateQuestText();
		yield return new WaitForSeconds(UI_DELAY / 3.0f);
		PopulateData();
		content.SetActive(true);
		if(animator!=null)
			animator.SetTrigger("Opening");
	}
	
	new protected IEnumerator DoHide() {
		content.SetActive(false);
		yield return true;
	}

	public void OnCompleteButtonClick(){

		int level = this.building.BuildingData == null ? 1 : this.building.BuildingData.level;
		List<AssetStateCollectable> assetStateCollectables = this.assetState.GetAllCollectables (level);
		foreach (AssetStateCollectable assetStateCollectable in assetStateCollectables) {
			ResourceManager.GetInstance().SetCollectableValueDiff(assetStateCollectable.collectable, -assetStateCollectable.GetQuantity());
		}

		this.building.AcknowledgeAfterPreConditionsMet ();
	}
	
}
