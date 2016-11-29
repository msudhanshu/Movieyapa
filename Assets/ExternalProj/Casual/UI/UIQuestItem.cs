using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//Item fo base/shop popup. It will have only look (name,image,clicklistner)/IBaseItemInfo of the item.
public class UIQuestItem : MonoBehaviour
{
	public Text titleLabel;
	public Button taskButton;

	[HideInInspector]
	public UserQuest quest;

	public virtual void taskButtonCallback() {
		PopupManager.GetInstance().ShowPanel (PanelType.QUEST_TASKS);
		if (UIGamePanel.activePanel is UIQuestTaskPanel) ((UIQuestTaskPanel)UIGamePanel.activePanel).InitialiseWithQuest(quest);
	}

	virtual public void InitialiseWithData(UserQuest quest) {
		this.quest = quest;
		titleLabel.text = quest.quest.name;

	}
}
