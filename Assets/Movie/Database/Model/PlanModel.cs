using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

//data
using KiwiCommonDatabase;
using SimpleSQL;


[Serializable]
public class PlanModel : BaseDbModel
{
	[PrimaryKey]
    public string id {get; set;}
    public string name{get; set;}
    public string description{get; set;}

    public PlanModel() {

    }

    public PlanModel(string id, string name, string description) {
        this.id = id;
        this.name = name;
        this.description = description;
    }

    #region ICapitalItem implementation

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

    public CapitalType getCapitalType ()
    {
        return CapitalType.CURRENCY;
    }

    public string getImageIconName() {
        return Config.AddSuffix(getId() , Config.RESOURCE_DOOBER_IMAGENAME_SUFFIX);
    }

    #endregion

    public static CurrencyModel GetCurrencyModel(String id) {
        //TODO DANGER
        //return KiwiCommonDatabase.DataHandler.wrapper.currencies.Find(x => x.id==id);
		return CurrencyModel.currencies.Find (x => x.id == id);
    }
 
    public static  Dictionary<ICapitalCurrency,int> ParseCurrency(String cur) {
        Dictionary<ICapitalCurrency,int> curmap = new Dictionary<ICapitalCurrency,int>();
		String[] rewardvals = cur.Split(SgUnityConfig.ServerConfig.LIST_DELIMETER);
        for(int i=0; i< rewardvals.Length; i++) {
            try {
				String[] s = rewardvals[i].Split(SgUnityConfig.ServerConfig.MAP_KEY_VALUE_DELIMETER);
                CurrencyModel c =  CurrencyModel.GetCurrencyModel(s[0]);
                if(c!=null)
                    curmap.Add(c,Int32.Parse(s[1]));
            } catch(Exception e) {
                Debug.LogError("Package currency : "+rewardvals[i]+", error="+e.Message);
            }
        }
        return curmap;
    }

}