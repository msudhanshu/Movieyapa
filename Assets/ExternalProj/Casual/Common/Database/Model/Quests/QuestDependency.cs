using UnityEngine;
using System.Collections;
using KiwiCommonDatabase;
using SimpleSQL;


[System.Serializable]
public class QuestDependency : BaseDbModel{
	[PrimaryKey]
	public int id{get; set;}
	public string quest{get; set;}
	public string dependsonQuest{get; set;}
	public int version{get; set;}
		
	public static string DEPENDS_ON_QUEST_ID_COL = "dependsonQuest";
		
		
		public string getDependsOnQuestId()
		{
			return this.dependsonQuest;
		}
		
		/**
	 * Default Constructor
	 */
		public QuestDependency(){
			
		}
		
		public QuestDependency(int id, string questId, string dependsOnQuestId){
			this.id = id;
			this.quest = questId;
			this.dependsonQuest = dependsOnQuestId;
		}
	}
