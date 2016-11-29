using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KiwiCommonDatabase;
using SimpleSQL;

[System.Serializable]
public class DbResource : BaseDbModel, IGameResource {
	
	[PrimaryKey]
	public string id {get; set;}
	public string name{get; set;}
	public string description{get; set;}

	public static string RESOURCE_XP = "xp";
	public static string RESOURCE_HAPPINESS = "happiness";
	public static string RESOURCE_GOLD = "gold";
	public static string RESOURCE_SILVER = "silver";
	public static string RESOURCE_AXE = "axe";
	public static string RESOURCE_CHEER = "cheer";

	/**
	 * Default Constructor
	 */
	public DbResource() {
		
	}

	public DbResource(string id, string name, string description) {
		this.id = id;
		this.name = name;
		this.description = description;
	}

	#region IGameItem implementation

	public string getId ()
	{
		return id;
	}

	public string getName ()
	{
		return name;
	}

	public string getDescription ()
	{
		return description;
	}

	public string getCamelNamePlural ()
	{
		return this.getDescription(); 
	}

	public string getAbsoluteName ()
	{
		return this.getName().ToLower();
	}

	public string getCamelName ()
	{
		return this.getAbsoluteName();
	}

	public GameResourceType getGameResourceType ()
	{
		return GameResourceType.RESOURCE;
	}

	public string getImageIconName() {
		return Config.AddSuffix(getId() , Config.RESOURCE_DOOBER_IMAGENAME_SUFFIX);
	}

	#endregion
}
