using System;
using KiwiCommonDatabase;
using SimpleSQL;
using System.Collections.Generic;

[System.Serializable]
public class Level: BaseDbModel
{
	[PrimaryKey]
	public int id {get; set;}

	public String name {get; set;}

	public int level {get; set;}

	public int maxHouseCount {get; set;}

	public int minQuantity {get; set;}

	public String levelAnnouncer {get; set;}
	
	private List<LevelReward> rewards;

	private List<LevelSpecialReward> specialRewards;

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

	/**
	 * Default Constructor
	 */
	public Level() {
		
	}
	
	Level(int id, String levelName, int level, int maxHouseCount, DbResource res, int minQuantity, String levelAnnouncer) {
		this.id = id ;
		this.name = levelName ;
		this.level = level ;
		this.maxHouseCount = maxHouseCount;
		this.resource = res ;
		this.minQuantity = minQuantity ;
		this.levelAnnouncer = levelAnnouncer ;
	}
	
	public Level getNextLevel() {
		return DatabaseManager.GetLevelObject (this.level + 1, this.resource);
	}
	
	public List<LevelReward> getRewards(){
		if (this.rewards == null)
						this.rewards = DatabaseManager.GetLevelRewardFC (this);
		return this.rewards;
	}
	
	public List<LevelSpecialReward> getSpecialRewards() {
		if(this.specialRewards == null)
			this.specialRewards = DatabaseManager.GetLevelSpecialRewardFC(this);
		return this.specialRewards;
		
	}

	public LevelReward getReward(DbResource res) {
		foreach(LevelReward reward in this.getRewards()) {
			if(res == reward.resource)
				return reward ;
		}
		return null ;
	}

	public string getMD5HashString() {
		return ""+ id+level+resource.id+minQuantity;
	}

}


