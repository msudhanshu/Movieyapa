using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SgUnity;

namespace SgUnity
{
    public enum ActionEnum
    {
      ADD,
      DELETE,
      PURCHASE,
      UPDATE,
      LEVEL_UPDATE,
      CURRENCY_UPDATE,
      CURRENCY_ADD,
      USER_PACKAGE_UPDATE,
      GET_NEXT_QUESTION,
      GET_QUESTION_BY_ID
    };

    public class ServerAction {


        public static Action getAction(ActionEnum actionEnum) {
                return new Action(actionEnum.ToString());
        }

    	public static Dictionary<ActionEnum, String> Urls = new Dictionary<ActionEnum,String>() {
    		{ActionEnum.GET_NEXT_QUESTION, "/movie/get?"},
            {ActionEnum.CURRENCY_UPDATE, "/movie/currency_update?"},
            {ActionEnum.CURRENCY_ADD, "/movie/currency_add?"},
            {ActionEnum.LEVEL_UPDATE, "/movie/level_update?"},
            {ActionEnum.USER_PACKAGE_UPDATE, "/movie/user_package_update?"},
            {ActionEnum.GET_QUESTION_BY_ID, "/movie/get_by_id?"}
    	};
                
    	public static void takeAction(ActionEnum action, ServerNotifier notifier){
    		if(!ServerConfig.SERVER_ENABLED)
    			return;
    		string url = ServerConfig.BASE_URL + Urls[action];
                ServerSyncManager.GetInstance().GetRawResponse(getAction(action), notifier, url);
    	}

        public static void GetQuestionByIdAction(ActionEnum action, string question_id, ServerNotifier notifier){
            if(!ServerConfig.SERVER_ENABLED)
                return;
            string url = ServerConfig.BASE_URL + Urls[action] + "user_id="+Config.USER_ID + "&question_id="+question_id ;
            ServerSyncManager.GetInstance().GetRawResponse(getAction(action), notifier, url);
        }

    	public static void takeAction(ActionEnum action, string urlString, ServerNotifier notifier, Boolean sync = false){
    		if(!ServerConfig.SERVER_ENABLED)
    			return;
    		string url = ServerConfig.BASE_URL + Urls [action] + urlString;
                MakeCallToServer(action, ServerSyncManager.GetInstance().serverNotifier, url, null, sync);
    	}
    	
    	public static void MakeCallToServer(ActionEnum action, ServerNotifier notifier, string url, string batchUrlData, bool sync) {
    		if(sync)
                    ServerSyncManager.GetInstance().GetResponseSync(getAction(action), notifier, url);
    		else
                    ServerSyncManager.GetInstance().GetResponseAsync(getAction(action), notifier,url);
    	}

    	public static void UpdateCurrencyAction(Dictionary<ICapitalCurrency, int> diffResources, Boolean sync = false){
    		if (! ServerConfig.SERVER_ENABLED) return;
            String url= ServerConfig.BASE_URL + Urls[ActionEnum.CURRENCY_UPDATE] +"user_id="+Config.USER_ID;
            url += CapitalManager.GetInstance().GetCurrencyString(diffResources);	
            MakeCallToServer(ActionEnum.CURRENCY_UPDATE, ServerSyncManager.GetInstance().serverNotifier, url, null, false);
    	}

        public static void AddCurrencyAction(Dictionary<ICapitalCurrency, int> diffResources, Boolean sync = false){
            if (! ServerConfig.SERVER_ENABLED) return;
            String url= ServerConfig.BASE_URL + Urls[ActionEnum.CURRENCY_ADD] +"user_id="+Config.USER_ID;
            url += CapitalManager.GetInstance().GetCurrencyString(diffResources);   
            MakeCallToServer(ActionEnum.CURRENCY_ADD, ServerSyncManager.GetInstance().serverNotifier, url, null, false);
        }

        public static void UpdateLevelAction(int newLevel, Dictionary<ICapitalCurrency, int> diffResources, Boolean sync = false){
            if (! ServerConfig.SERVER_ENABLED) return;
            String url= ServerConfig.BASE_URL + Urls[ActionEnum.LEVEL_UPDATE] +"user_id="+Config.USER_ID + "&new_level="+newLevel;
            url += CapitalManager.GetInstance().GetCurrencyString(diffResources);   
            MakeCallToServer(ActionEnum.LEVEL_UPDATE, ServerSyncManager.GetInstance().serverNotifier, url, null, false);
        }

        public static void UpdateUserPackageAction(UserPackage userPackage, Boolean sync = false){
            if (! ServerConfig.SERVER_ENABLED) return;
            String url= ServerConfig.BASE_URL + Urls[ActionEnum.USER_PACKAGE_UPDATE] +"user_id="+Config.USER_ID + "&package_id="+userPackage.package_id+
                "&solved="+userPackage.solved+"&score="+userPackage.score+"&status="+userPackage.status+"&TimeUnlockEndTime="+userPackage.TimeUnlockEndTime;
            MakeCallToServer(ActionEnum.USER_PACKAGE_UPDATE, ServerSyncManager.GetInstance().serverNotifier, url, null, false);
        }

//        public static void takeAction(ActionEnum action, int newLevel,
//            Dictionary<ICapitalCurrency, int> diffResources, Boolean sync = false){
//            if (! ServerConfig.SERVER_ENABLED) return;
//            String url= ServerConfig.BASE_URL + Urls[ActionEnum.LEVEL_UPDATE] +"user_id="+Config.USER_ID+
//                "&level_xp="+CapitalManager.GetInstance().Level+
//                "&new_level=" + newLevel;
//            //url += ResourceManager.GetInstance().GetResourceString(diffResources);    
//            MakeCallToServer(ActionEnum.LEVEL_UPDATE, ServerSyncManager.GetInstance().serverNotifier, url, null, false);
//        }
}
}
