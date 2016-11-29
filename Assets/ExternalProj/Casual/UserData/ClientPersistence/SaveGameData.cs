using System.Collections.Generic;

/**
 * All data required for a saved game.
 */ 
[System.Serializable]
public class SaveGameData  {

	public virtual List<BuildingData> buildings {get; set;}

	public virtual List<UserAnimalHelper> userAnimalHelpers {get; set;}

	public virtual int silver {get; set;}
	
	public virtual int gold {get; set;}

	public virtual int xp {get; set;}

	public virtual List<StateTransition> activities {get; set; }

	public virtual List<CustomResource> otherResources {get; set;}
}