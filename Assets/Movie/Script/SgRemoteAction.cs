using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SgUnityConfig;

namespace SgUnity
{
  

    public class SgRemoteAction {
		private static FirebaseDbRemoteAction db = new FirebaseDbRemoteAction();

        public static Action getAction(ActionEnum actionEnum) {
                return new Action(actionEnum.ToString());
        }

		private static string GetRestAPI(ActionEnum action) {
			return SgUnityConfig.ServerConfig.GetBaseUrl + Urls [action];
		}


    	public static Dictionary<ActionEnum, String> Urls = new Dictionary<ActionEnum,String>() {
    		{ActionEnum.GET_NEXT_QUESTION, "/movie_get?"},
            {ActionEnum.CURRENCY_UPDATE, "/movie_currency_update?"},
            {ActionEnum.CURRENCY_ADD, "/movie_currency_add?"},
            {ActionEnum.LEVEL_UPDATE, "/movie_level_update?"},
            {ActionEnum.USER_PACKAGE_UPDATE, "/movie_user_package_update?"},
            {ActionEnum.GET_QUESTION_BY_ID, "/movie_get_by_id?"}
    	};
                
    	public static void takeAction(ActionEnum action, ServerNotifier notifier){
    		if(!ServerConfig.SERVER_ENABLED)
    			return;
    		string url = GetRestAPI(action);
            ServerSyncManager.GetInstance().GetRawResponse(getAction(action), notifier, url);
    	}
    	public static void takeAction(ActionEnum action, string urlString, ServerNotifier notifier, Boolean sync = false){
    		if(!ServerConfig.SERVER_ENABLED)
    			return;
    		string url = GetRestAPI(action) + urlString;
                MakeCallToServer(action, ServerSyncManager.GetInstance().serverNotifier, url, null, sync);
    	}
    	
    	public static void MakeCallToServer(ActionEnum action, ServerNotifier notifier, string url, string batchUrlData, bool sync) {
    		if(sync)
                    ServerSyncManager.GetInstance().GetResponseSync(getAction(action), notifier, url);
    		else
                    ServerSyncManager.GetInstance().GetResponseAsync(getAction(action), notifier,url);
    	}


		public static void GetQuestionByIdAction(int question_id, ServerNotifier notifier){
			db.GetQuestionByIdAction (question_id, notifier);
		}
			
		public static void UpdateCurrencyAction(Dictionary<ICapitalCurrency, int> diffResources, ServerNotifier notifier){
			db.UpdateCurrencyAction (diffResources,notifier);
    	}

//        public static void AddCurrencyAction(Dictionary<ICapitalCurrency, int> diffResources, Boolean sync = false){
//            if (! ServerConfig.SERVER_ENABLED) return;
//			String url= GetRestAPI(ActionEnum.CURRENCY_ADD) +"user_id="+Config.USER_ID;
//            url += CapitalManager.GetInstance().GetCurrencyString(diffResources);   
//            MakeCallToServer(ActionEnum.CURRENCY_ADD, ServerSyncManager.GetInstance().serverNotifier, url, null, false);
//        }
//
//        public static void UpdateLevelAction(int newLevel, Dictionary<ICapitalCurrency, int> diffResources, Boolean sync = false){
//            if (! ServerConfig.SERVER_ENABLED) return;
//			String url= GetRestAPI(ActionEnum.LEVEL_UPDATE) +"user_id="+Config.USER_ID + "&new_level="+newLevel;
//            url += CapitalManager.GetInstance().GetCurrencyString(diffResources);   
//            MakeCallToServer(ActionEnum.LEVEL_UPDATE, ServerSyncManager.GetInstance().serverNotifier, url, null, false);
//        }

        public static void UpdateUserPackageAction(UserPackage userPackage, Boolean sync = false){
			db.UpdateUserPackageAction (userPackage);
        }

		public static void CarrierQSolved (QSolvedItemModel s, Boolean sync = false){
			db.CarrierQSolved (s);
		}

}
}