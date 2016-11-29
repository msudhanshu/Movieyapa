using UnityEngine;
using System.Collections;

public interface IGameItem {

	string getId();
	string getName();
	string getDescription();
	string getCamelNamePlural();
	string getAbsoluteName();
	string getCamelName();
	string getImageIconName();
	GameResourceType getGameResourceType();
}

public enum GameResourceType{
	COLLECTABLE,
	RESOURCE,
	ASSET
};