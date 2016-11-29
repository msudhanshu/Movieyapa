using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;

public class UIQuestCompletePanel: UIGamePanel {

	public Text questTitle;
	public Text questFinishedDescription;
	private List<UIQuestRewardItem> uiQuestRewardItems;
	public GameObject questRewardItemPrefab;
	public ScrollRect scrollRect;
	public GridLayoutGroup gridLayoutGroup;
	public bool horizontal;
	public int rowColCount;
	private UserQuest quest;

	override protected void Init() {
		base.Init ();
		// Force content to normal position then update cancel button position
//		content.transform.position = showPosition;	
	}

	public void InitialiseWithQuest(UserQuest quest) {
		this.quest = quest;
	}

	virtual protected void PopulateData() {
		if(quest==null)
			quest = UserQuest.activeQuests[0];
		if(uiQuestRewardItems==null)
			uiQuestRewardItems = new List<UIQuestRewardItem>();
		ClearAllItems();

		questTitle.text = quest.quest.name;
		questFinishedDescription.text = quest.quest.finishedDescription;
		int i = 0;
		foreach(QuestReward questReward in quest.quest.getRewards()) {
			Transform go = CreateItemPrefab(questRewardItemPrefab,i);
			UIQuestRewardItem panel = go.GetComponent<UIQuestRewardItem>();
			panel.InitialiseWithData(questReward);
			uiQuestRewardItems.Add (panel);
			i++;
		}
		ResizeGridLayout(i);
	}

	private void ClearAllItems() {
		if(uiQuestRewardItems!=null) {
			foreach(UIQuestRewardItem item in uiQuestRewardItems) {
				Pool.Destroy(item.gameObject);
			}
			uiQuestRewardItems.Clear();
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

/*
	public void UpdateQuestText(){
		if(UserQuest.activeQuests!=null && UserQuest.activeQuests.Count > 0){
			questText.text = "<b>QUESTS</b>:-\n\n";
			int i = 1;
			foreach(UserQuest quest in UserQuest.activeQuests){
				questText.text += "<b>Quest "+i+" :- "+quest.quest.name+"</b>\n";
				questText.text += quest.quest.description+"\n\n";
				questText.text += "<b>Pending Tasks :-</b>\n";
				int j= 1;
				foreach(UserQuestTask task in quest.pendingQuestTasks){
					questText.text += "Task "+j+" :-\n";
					questText.text += task.questTask.frontDescription+"\n";
					questText.text += task.currentCount+"/"+task.questTask.requiredQuantity+"\n\n";
					j++;
				}
				questText.text+= "\n";
				i++;
			}
		}
	}
*/
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
	
}
