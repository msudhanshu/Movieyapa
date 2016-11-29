using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

//Base popup of all the shoppopup, which will have list (grid) of items(UIBaseItem).
public class UIBasePopup : UIGamePanel {

//	public GameObject scrollPanel;
	public ScrollRect scrollRect;
	public GridLayoutGroup gridLayoutGroup;
	public Button backButton;
	public bool horizontal;


	private bool initialised = false;

	override protected void Init() {
		base.Init();
	}

	public virtual List<Asset> GetAssetData() {
		return BuildingManager3D.GetInstance().GetAllBuildingTypes().ToList();
	}

	void Start() {
		StartCoroutine(WaitForDataAndStart());
	}

	IEnumerator WaitForDataAndStart() {
		while (!BuildingManager3D.GetInstance().IsReady())
			yield return 0;
		yield return 0;
		if (!initialised) {
			//FIXME: Anuj - Commented to make the build work
			//			buildingScrollPanel.GetComponent<UIScrollView>().ResetPosition();
			InitDefaultPopup();
			initialised = true;
		}
		// Force content to normal position then update cancel button position
	/*	content.transform.position = showPosition;	
		showPosition = cancelButton.transform.position;
		hidePosition = cancelButton.transform.position + new Vector3(0, UI_TRAVEL_DIST, 0);
		cancelButton.transform.position = hidePosition;*/
	}

	virtual protected void InitDefaultPopup() {

	}

	override public void Show() {
		if (activePanel != null) activePanel.Hide ();
		StartCoroutine(DoShow ());
		activePanel = this;
	}
	
	override public void Hide() {
		StartCoroutine(DoHide ());
	}
	
	new protected IEnumerator DoShow() {
		yield return new WaitForSeconds(UI_DELAY / 3.0f);
		content.SetActive(true);
//		yield return true;
//		GetComponent<UIPanel>().Refresh();
		//FIXME: Anuj - Commented to make the build work
//		buildingScrollPanel.GetComponent<UIScrollView>().ResetPosition();
//		cancelButton.SetActive (true);
		if(animator!=null)
			animator.SetTrigger("Opening");
		yield return true;
//		GetComponent<UIPanel>().Refresh();
//		iTween.MoveTo(cancelButton, showPosition, UI_DELAY);
	}
	
	new protected IEnumerator DoHide() {
		yield return new WaitForSeconds(UI_DELAY / 3.0f);
		content.SetActive(false);
//		iTween.MoveTo(cancelButton, hidePosition, UI_DELAY);
		//yield return new WaitForSeconds(UI_DELAY / 3.0f);
//		cancelButton.SetActive (false);
		yield return true;
	}
	
	virtual public void BackButtonClicked() {

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

	//TEMP HACK:
	public void ResizeGridLayout(int totalItem) {

		RectTransform t = gridLayoutGroup.GetComponent<RectTransform>();
		if(horizontal) {
			int rowCount = gridLayoutGroup.constraintCount;
			scrollRect.horizontal = true;
			scrollRect.vertical = false;
			gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Vertical;
			float size = scrollRect.GetComponent<RectTransform>().rect.height;
			float itemsize = (size-rowCount*gridLayoutGroup.spacing.y)/(float)rowCount ;
		//	gridLayoutGroup.cellSize = new Vector2( itemsize,itemsize);
			t.sizeDelta = new Vector2( (float)( Mathf.Ceil((float)totalItem/(float)rowCount) )* itemsize, 
			                          size );
		} else {
			int colCount = gridLayoutGroup.constraintCount;;
			scrollRect.horizontal = false;
			scrollRect.vertical = true;
			gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
			t.sizeDelta = new Vector2(colCount * (gridLayoutGroup.cellSize.x+gridLayoutGroup.spacing.x), (float)( Mathf.Ceil((float)totalItem/(float)colCount) )*(gridLayoutGroup.cellSize.y+gridLayoutGroup.spacing.y));  
        }
        //TODO : CASE WHEN ONE WANTS TO SCROLL BOTH THE WAY....  
    }

}
