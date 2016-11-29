using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Expansion;
using SgUnity;
/**
 * Handles currencies, gold, etc.
 */ 
public class CapitalManager : Manager<CapitalManager> {
	
	/**
	 * View of currencies.
	 */
	public GameObject view;

	//TODO : manjeet : once unity is upgraded
//	public UnityEvent;
    public delegate void CurrencyUpdateAction(ICapitalCurrency currency);
	public static event CurrencyUpdateAction OnCurrencyUpdated;

    public delegate void LevelUpAction();
    public static event LevelUpAction OnLevelUpAction;

	/**
	 * Default for gold when new game is started.
	 */ 
    private int defaultGold = 20;
    private int defaultLevel = 1;
    private int defaultHint = 3;
    private int defaultTicket = 4;

	private Dictionary<ICapitalCurrency, int> currencies;
	

    override public void StartInit() {
        currencies = new Dictionary<ICapitalCurrency, int>();
        Gold = defaultGold;
        Hint = defaultHint;
        Ticket = defaultTicket;
        Level = defaultLevel;
        Load();
    }

    override public void PopulateDependencies() {
        dependencies = new List<ManagerDependency>();
        dependencies.Add(ManagerDependency.DATA_LOADED);
    }

    /**
     * Load currencies from save game data.
     */ 
    //Deprecated
    virtual public void Load(SaveGameData data) {
        Gold = data.gold;
        view.SendMessage ("UpdateCurrency", true, SendMessageOptions.DontRequireReceiver);
        view.SendMessage ("UpdateGold", true, SendMessageOptions.DontRequireReceiver);
        view.SendMessage("UpdateLevel", false, SendMessageOptions.DontRequireReceiver);
    }

    virtual public void Load() {
        UserCurrency userCurrency = KiwiCommonDatabase.DataHandler.wrapper.userCurrency;
        Gold = userCurrency.gold;
        Hint = userCurrency.hint;
        Ticket = userCurrency.ticket;
        Level = userCurrency.level;
        Xp = userCurrency.xp;
        foreach(KeyValuePair<ICapitalCurrency,int> item in currencies){
            view.SendMessage("UpdateGenericCurrencyInstant", item.Key, SendMessageOptions.DontRequireReceiver);
        }
    }
//
//    virtual public void Load() {
//        List<UserCurrency> data = KiwiCommonDatabase.DataHandler.wrapper.userCurrencies;
//        foreach(UserCurrency userCurrency in data ){
//            switch(userCurrency.id){
//            case "currency_gold":
//                Gold = userCurrency.quantity;
//                break;
//            case "currency_hint":
//                Hint = userCurrency.quantity;
//                break;
//            case "currency_ticket":
//                Ticket = userCurrency.quantity;
//                break;
//            case "currency_level":
//                Level = userCurrency.quantity;
//                break;
//            }
//            view.SendMessage("UpdateGenericCurrencyInstant", userCurrency.currency, SendMessageOptions.DontRequireReceiver);
//        }
//    }

	virtual public int Gold {
		get {
            return GetCurrencyValue(CurrencyModel.CURRENCY_GOLD);
		}
		protected set {
            SetCurrencyValue(CurrencyModel.CURRENCY_GOLD, value);
		}
	}
       
    virtual public int Xp {
        get {
            return GetCurrencyValue(CurrencyModel.CURRENCY_XP);
        }
        protected set {
            SetCurrencyValue(CurrencyModel.CURRENCY_XP, value);
        }
    }

    virtual public int Hint {
        get {
            return GetCurrencyValue(CurrencyModel.CURRENCY_HINT);
        }
        protected set {
            SetCurrencyValue(CurrencyModel.CURRENCY_HINT, value);
        }
    }

    virtual public int Ticket {
        get {
            return GetCurrencyValue(CurrencyModel.CURRENCY_TICKET);
        }
        protected set {
            SetCurrencyValue(CurrencyModel.CURRENCY_TICKET, value);
        }
    }

	virtual public int Level {
        get {
            return GetCurrencyValue(CurrencyModel.CURRENCY_LEVEL);
        }
        protected set {
            SetCurrencyValue(CurrencyModel.CURRENCY_LEVEL, value);
        }
	}

    private int GetCurrencyValue(string resType) {
        return GetCurrencyValue(CurrencyModel.GetCurrencyModel(resType));
    }

    private int GetCurrencyValue(ICapitalCurrency res) {
        if (!currencies.ContainsKey (res))
            currencies.Add (res, 0);
        return currencies[res];
    }
 
    private void SetCurrencyValue(string resType, int quantity) {
        CurrencyModel currency = CurrencyModel.GetCurrencyModel(resType);
        if (!currencies.ContainsKey (currency))
            currencies.Add (currency, 0);
        currencies[currency] = quantity;
    }  

	public void AddCurrency(ICapitalCurrency currency, int quantity){
		if (quantity == 0)
			return;
		if (!currencies.ContainsKey(currency))
			currencies[currency] = 0;
		currencies[currency] += quantity;
        Debug.Log("Adding quantity " + quantity + " to currency " + currency.getId()+",total="+currencies[currency]);
		view.SendMessage ("UpdateGenericCurrency", currency, SendMessageOptions.DontRequireReceiver);
        OnCurrencyUpdated(currency);
	}

    public void AddCurrency(ICapitalCurrency currency, int quantity, bool save){
        AddCurrency(currency,quantity);
        if(save)
            SaveDiffCurrency(currency,quantity);
    }

    private void SaveDiffCurrency(ICapitalCurrency currency, int quantity) {
        Dictionary<ICapitalCurrency, int> c = new Dictionary<ICapitalCurrency, int>();
        c.Add(currency,quantity);
        SgUnity.ServerAction.AddCurrencyAction(c,true);
    }

    public void SaveCurrency() {
        SgUnity.ServerAction.UpdateCurrencyAction(currencies, true);
    }

	public void AddCurrency(string resType, int quantity){
		AddCurrency(CurrencyModel.GetCurrencyModel(resType), quantity);
	}
	
	public void SubtractCurrency(string resType, int quantity) {
		AddCurrency(resType, -quantity);
	}

	public void SubtractCurrency(ICapitalCurrency currency, int quantity) {
		AddCurrency(currency, -quantity);
	}


	virtual public void AddCurrency(Dictionary<ICapitalCurrency, int> rewards) {
		if (rewards == null)
			return;
		foreach(ICapitalCurrency currency in rewards.Keys) {
			AddCurrency(currency, rewards[currency]);
		}
	}

	virtual public void AddRewards(List<ICurrencyUpdate>  rewards) {
		if (rewards == null)
			return;
		foreach(ICurrencyUpdate reward in rewards) {
			AddCurrency(reward.GetCurrency(), reward.GetQuantity());
		}
	}

	virtual public bool CanDeductCurrencys(Dictionary<ICapitalCurrency, int> costs) {
		if (costs == null)
			return true;
		
		foreach(ICapitalCurrency currency in costs.Keys) {
			if (GetCurrencyValue(currency.getId()) < costs[currency]) {
				return false;
			}
		}

		return true;
	}

	//Return false if not enough currencies to deduct cost
	virtual public void DeductCurrencys(Dictionary<ICapitalCurrency, int> costs) {
		if (costs == null)
			return;
		
		foreach(ICapitalCurrency currency in costs.Keys) {
			SubtractCurrency(currency, costs[currency]);
		}
	}

	virtual public void UpdateCurrency(string currencyId, int quantity) {
		AddCurrency(currencyId, quantity);
	}

	virtual public int GetQuantity(string currencyId) {
		return GetCurrencyValue(currencyId);
	}

	public string GetCurrencyString(Dictionary<ICapitalCurrency, int> diffCurrencys) {
		string currencyString = "";

		if (diffCurrencys == null)
			return currencyString;

		foreach( ICapitalCurrency gameCurrency in diffCurrencys.Keys) {
			currencyString += "&" + gameCurrency.getId() + "=" + diffCurrencys[gameCurrency];
		}
		return currencyString;
	}

	public Vector3 GetCurrencyIconScreenPos(CurrencyModel res) {
		Transform currencyIconTransform= null;
		if(res.getId() == "gold") {
				currencyIconTransform = Util.SearchHierarchyForBone(view.transform,"Gold");
				if(currencyIconTransform!=null)
					return  currencyIconTransform.position;//view.transform.FindChild("Gold").position; //GoldIcon.position;
		}else if(res.getId() == "axe") {
			currencyIconTransform = Util.SearchHierarchyForBone(view.transform,"Axe");
			if(currencyIconTransform!=null)
				return  currencyIconTransform.position;
        }else if(res.getId() == "silver") {
			currencyIconTransform = Util.SearchHierarchyForBone(view.transform,"Silver");
			if(currencyIconTransform!=null)
				return  currencyIconTransform.position;
		}else if(res.getId() == "cheer") {
			currencyIconTransform = Util.SearchHierarchyForBone(view.transform,"Cheer");
			if(currencyIconTransform!=null)
				return  currencyIconTransform.position;
		}
			return new Vector3(-Screen.width,-Screen.height,0);
	}

	public Dictionary<ICapitalCurrency, int> GetDiffCurrencies(List<ICurrencyUpdate> resUpdates) {
		Dictionary<ICapitalCurrency, int> diffCurrencys = new Dictionary<ICapitalCurrency, int> ();
		foreach (ICurrencyUpdate currencyUpdate in resUpdates) {
			diffCurrencys[currencyUpdate.GetCurrency()] = currencyUpdate.GetQuantity();
		}
		return diffCurrencys;
	}

    public void DoLevelUp() {
        Level = Level + 1;
        OnLevelUpAction();
    }

//    public void LevelUp(CurrencyModel currency, Level newLevel, Level nextLevel) {
//        if(currency.Equals(CurrencyModel.GetCurrencyModel(CurrencyModel.CURRENCY_LEVEL)) ){
//           view.SendMessage("UpdateLevel", true, SendMessageOptions.DontRequireReceiver);
//
//            //XP Level up popup
//            ((UILevelUpPanel)PopupManager.GetInstance ().getPanel (PanelType.LEVEL_UP)).InitialiseWithData (newLevel);
//            PopupManager.GetInstance().SchedulePopup(PanelType.LEVEL_UP);
//            Debug.Log("Show Level Up Popup -- " + currency.getId() + ": Level " + newLevel.level);
//
//            Dictionary<ICapitalCurrency, int> currentDiffCurrencys = GetInstance().GetDiffCurrencys(newLevel.getRewards().ConvertAll(x => (ICurrencyUpdate)x));
//            // Give currencies if the quest is not expired
//            GetInstance().AddCurrencys(currentDiffCurrencys);
//            SgUnity.ServerAction.takeAction(ActionEnum.LEVEL_UPDATE, newLevel.level, currentDiffCurrencys, true);
//        }
//    }
       
}
