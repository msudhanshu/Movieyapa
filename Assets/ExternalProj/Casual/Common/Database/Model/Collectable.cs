using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KiwiCommonDatabase;
using SimpleSQL;

[System.Serializable]
public class Collectable : BaseDbModel, IGameResource {

	[PrimaryKey]
	public string id {get; set;}
	public string name{get; set;}
	public string description{get; set;}
	
	/**
	 * Default Constructor
	 */
	public Collectable() {
		
	}

	public Collectable(string id, string name, string description) {
		this.id = id;
		this.name = name;
		this.description = description;
	}

	private static Dictionary<string, Collectable> Collectables = new Dictionary<string, Collectable>();

	public static Collectable GetCollectable(string id) {
		if (!Collectables.ContainsKey(id))
			Collectables[id] = DatabaseManager.GetInstance().GetDbHelper().QueryObjectById<Collectable>(id);
		return Collectables[id];
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
		return this.getName();
	}

	public string getCamelName ()
	{
		return this.getAbsoluteName();
	}

	public GameResourceType getGameResourceType ()
	{
		return GameResourceType.COLLECTABLE;
	}

	public string getImageIconName() {
		return Config.AddSuffix(getId() , Config.COLLECTABLE_DOOBER_IMAGENAME_SUFFIX);
	}

	#endregion
}
