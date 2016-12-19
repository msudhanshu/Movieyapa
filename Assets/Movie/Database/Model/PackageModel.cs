using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

//data
using KiwiCommonDatabase;
using SimpleSQL;


[Serializable]
public class PackageModel : BaseDbModel
//, ICurrencyCost<ICurrencyUpdate>, ICurrencyReward<ICurrencyUpdate>
{
	[PrimaryKey]
	public string _id {get; set;}
	public string name {get; set;}
    public string logo {get; set;}
    public int minLevel {get; set;} //it will be locked till this level. after this can be bought,
    public int unlockTime {get; set;} //in days // after this much time after minLevel reached, it will be free to play
    public int unlockLevel {get; set;} //if 0 duration will be considered. or else afther this many level from minLevel it wil become free.
    public int timeLockGoldPrice {get;set;}
    public int size {get; set;}
    //cost , reward , question wise and package complete wise
    //public
    public string costs {get; set;}
    public string rewards {get; set;}

    public List<string> questionIds;
//    public Dictionary<string, int> costs {get; set;}
//    public Dictionary<string,int> rewards {get; set;}

 
    private Dictionary<ICapitalCurrency,int> tcostsMap;
    private Dictionary<ICapitalCurrency,int> trewardsMap;
    private bool isInit = false;

    private void Init() {
        if(isInit) return;

        tcostsMap = CurrencyModel.ParseCurrency(costs);

//            new Dictionary<ICapitalCurrency,int>();
//        String[] costvals = costs.Split(SgUnity.ServerConfig.LIST_DELIMETER);
//        for(int i=0; i< costvals.Length; i++) {
//            try{
//                String[] s = costvals[i].Split(SgUnity.ServerConfig.MAP_KEY_VALUE_DELIMETER);
//                CurrencyModel c =  CurrencyModel.GetCurrencyModel(s[0]);
//                if(c!=null)
//                    tcostsMap.Add(c,1);
//            } catch(Exception e) {
//                Debug.LogError("Package currency : "+costvals[i]+", error="+e.Message);
//            }
//        }

        trewardsMap = CurrencyModel.ParseCurrency(rewards);

//        new Dictionary<ICapitalCurrency,int>();
//        String[] rewardvals = rewards.Split(SgUnity.ServerConfig.LIST_DELIMETER);
//        for(int i=0; i< rewardvals.Length; i++) {
//            try {
//                String[] s = rewardvals[i].Split(SgUnity.ServerConfig.MAP_KEY_VALUE_DELIMETER);
//                CurrencyModel c =  CurrencyModel.GetCurrencyModel(s[0]);
//                if(c!=null)
//                    trewardsMap.Add(c,Int32.Parse(s[1]));
//            } catch(Exception e) {
//                Debug.LogError("Package currency : "+costvals[i]+", error="+e.Message);
//            }
//        }

        isInit = true;
    }

    public Dictionary<ICapitalCurrency,int> costsMap {
        get {
            Init();
            return tcostsMap;
        }
    }

    public Dictionary<ICapitalCurrency,int> rewardsMap {
        get {
            Init();
            return trewardsMap;
        }
    }
        

//    #region IResourceUpdate implementation
//
//    public Dictionary<ICapitalCurrency,int> GetCost() {
//        return costsMap ;  
//    }
//
//    public Dictionary<ICapitalCurrency,int>  GetReward() {
//        return rewardsMap;
//    }
//
//    #endregion
}