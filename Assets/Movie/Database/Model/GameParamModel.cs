using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

//data
using KiwiCommonDatabase;
using SimpleSQL;


[Serializable]
public class GameParamModel : BaseDbModel
{
	[PrimaryKey]
    public string key{get; set;}
    public string value{get; set;}
   
    public static string CARRIER_PLAY_COST_KEY = "carrier_play_cost";
    public static string CARRIER_QUESTION_COUNT_KEY = "carrier_question_count";
    public static string CARRIER_PLAY_REWARD_KEY = "carrier_play_reward";


    public static Dictionary<ICapitalCurrency,int> carrier_play_default_cost;
    public static Dictionary<ICapitalCurrency,int> carrier_play_default_reward;
    public static int carrier_question_default_count;

    public GameParamModel() {}
   
    public GameParamModel(string key, string value) {
        this.key = key;
        this.value = value;
    }

    public static void InitGameParams() {
        carrier_play_default_cost = CurrencyModel.ParseCurrency(GetGameParamValue(CARRIER_PLAY_COST_KEY));
        carrier_play_default_reward = CurrencyModel.ParseCurrency(GetGameParamValue(CARRIER_PLAY_REWARD_KEY));
        carrier_question_default_count = Int32.Parse (GetGameParamValue(CARRIER_QUESTION_COUNT_KEY) );
    }

    public static string GetGameParamValue(string key) {
        //TODO DANGER
        GameParamModel g = KiwiCommonDatabase.DataHandler.wrapper.gameParams.Find(x => x.key==key);
        if(g!=null) return g.value;
        return null;
    }

    public static GameParamModel GetGameParamModel(string key) {
        //TODO DANGER
        return KiwiCommonDatabase.DataHandler.wrapper.gameParams.Find(x => x.key==key);
    }



    public static List<GameParamModel> InitSelfNonDiffMarketTable() {
        List<GameParamModel> list = new List<GameParamModel>();
        list.Add(new GameParamModel(CARRIER_QUESTION_COUNT_KEY,"2"));
        list.Add(new GameParamModel(CARRIER_PLAY_COST_KEY,"gold:10;;ticket:2"));
        list.Add(new GameParamModel(CARRIER_PLAY_REWARD_KEY,"gold:50;;ticket:3;;hint:2"));
        return list;
    }

}