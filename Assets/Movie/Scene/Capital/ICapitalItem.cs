using UnityEngine;
using System.Collections;

public interface ICapitalItem {

	string getId();
	string getName();
	string getDescription();
	string getCamelNamePlural();
	string getAbsoluteName();
	string getCamelName();
	string getImageIconName();
	CapitalType getCapitalType();
	bool isGlobalCurrency();
}

public enum CapitalType{
	COLLECTABLE,
	CURRENCY,
	ASSET
};