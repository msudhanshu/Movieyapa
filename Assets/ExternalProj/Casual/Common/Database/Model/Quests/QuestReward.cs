using UnityEngine;
using System.Collections;
using KiwiCommonDatabase;
using SimpleSQL;


[System.Serializable]
public class QuestReward : BaseDbModel, IResourceUpdate {
	[PrimaryKey]
	public int id{get; set;}
	public string questId{get; set;}
	private Quest _quest;
	[Ignore]
	public Quest quest {
		get {
			if (_quest == null)
				_quest = DatabaseManager.GetQuest(questId);
			return _quest;
		}
		set {
			questId = value.id;
		}
	}

	public string resourceId {get; set;}
	[Ignore]
	private DbResource _resource;
	[Ignore]
	public DbResource resource {
		get {
			if (_resource == null)
				_resource = DatabaseManager.GetDbResource(resourceId);
			return _resource;
		}
		set {
			resourceId = value.id;
		}
	}

	public int quantity{get; set;}
	
	/**
	 * Default Constructor
	 */
	public QuestReward() {
		
	}
	
	public QuestReward(int id, Quest quest, DbResource resource, int quantity) {
		this.id = id;
		this.quest = quest;		
		this.resource = resource;
		this.quantity = quantity;		
	}

	#region IResourceUpdate implementation
	
	public IGameResource GetResource ()
	{
		return resource;
	}
	
	public int GetQuantity ()
	{
		return quantity;
	}
	
	#endregion

}
