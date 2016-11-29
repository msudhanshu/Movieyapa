using UnityEngine;
using System.Collections;
using KiwiCommonDatabase;
using SimpleSQL;


[System.Serializable]
public class QuestDialog : BaseDbModel{

	[PrimaryKey]
	public int id{get; set;}
	public string description{get; set;}
	public string name{get; set;}
	public string expression{get; set;}
	public int dialogOrder{get; set;}
	public string questId{get; set;}
	[Ignore]
	private Quest _quest;
	[Ignore]
	public Quest quest{
		get{
			if(questId!=null && _quest==null)
				_quest = DatabaseManager.GetQuest(questId);
			return _quest;
		}
		set{
			questId = value.id;
		}
	}
	public bool isIntro{get; set;}
	public bool exitPrevious{get; set;}
	public DialogAlignment announcerType{get; set;}
	public enum DialogAlignment{
		LEFT,
		RIGHT
	}

	public QuestDialog(){
	}
	
	
	public QuestDialog(string name, string description, string expression, int order, DialogAlignment announcerType) {
		this.name = name;
		this.description = description;
		this.expression = expression;
		this.announcerType = announcerType;
		this.dialogOrder = order;
	}

}
