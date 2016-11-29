using System;
using KiwiCommonDatabase;
using SimpleSQL;

[System.Serializable]
public class LevelSpecialReward : BaseDbModel
{
	[PrimaryKey]
	public int id {get; set;}
	
	public int levelId;
	[Ignore]
	private Level _level;
	[Ignore]
	public Level level{
		get{
			if(_level == null){
				_level = DatabaseManager.GetLevel(levelId);
			}
			return _level;
		}
		set{
			levelId = value.id;
		}
	}

	public String itemType {get; set;}

	public String itemId {get; set;}

	public int quantity {get; set;}

	public int version {get; set;}

	public static string LEVEL_COLUMN = "levelId";

	public LevelSpecialReward() {
		
	}
	
	public LevelSpecialReward(int id, Level level, String itemType, String itemId, int quantity) {
		this.id = id ;
		this.level = level ;
		this.itemType = itemType;
		this.itemId = itemId;
		this.quantity = quantity ;
	}

	public String getMD5HashString() {
		return ""+ id+level+itemType+itemId+quantity;
	}

}


