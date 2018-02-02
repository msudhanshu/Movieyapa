using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UICurrencyView : MonoBehaviour {

	public Text levelText;

	public Text goldText;

	public Text hintText;

	public Text ticketText;
    public Text silverText;

	private int displayedLevel;
	private int displayedGold;
	private int displayedHint;
	private int displayedTicket;
	private int displayedSilver;
    private int displayedXp;

	void Start() {
		#region old_implementation
        /*		List<CustomCurrencyType> currencys = CapitalManager.GetInstance().GetCustomCurrencyTypes();
		if (currencys.Count == 0) {
			customCurrency1Label.gameObject.SetActive(false);
			customCurrency1Sprite.gameObject.SetActive(false);
		} else {
			customCurrency1Label.gameObject.SetActive(true);
			customCurrency1Sprite.gameObject.SetActive(true);
			customCurrency1Sprite.spriteName = currencys[0].spriteName;
			displayedCustomCurrencys1 = CapitalManager.GetInstance().GetCustomCurrency(currencys[0].id);
			customCurrency1Label.text = displayedCustomCurrencys1.ToString();
			customCurrencyType1 = currencys[0].id;
		}
		if (currencys.Count < 2) {
			customCurrency2Label.gameObject.SetActive(false);
			customCurrency2Sprite.gameObject.SetActive(false);
		} else {
			customCurrency2Label.gameObject.SetActive(true);
			customCurrency2Sprite.gameObject.SetActive(true);
			customCurrency2Sprite.spriteName = currencys[1].spriteName;
			displayedCustomCurrencys2 = CapitalManager.GetInstance().GetCustomCurrency(currencys[1].id);
			customCurrency2Label.text = displayedCustomCurrencys2.ToString();
			customCurrencyType2 = currencys[1].id;
		}*/
		#endregion
        CapitalManager.OnCurrencyUpdated += UpdateGenericCurrency;
        CapitalManager.OnLevelUpAction += OnLevelUp;
	}

    public void OnLevelUp() {
        UpdateGenericCurrency(CurrencyModel.GetCurrencyModel(CurrencyModel.CURRENCY_LEVEL));
    }

	public void UpdateGenericCurrency(ICapitalCurrency currency) {
        DisplayCurrency(new CurrencyUpdate(currency, false));
		//StopCoroutine ("DisplayGenericCurrency");
		//StartCoroutine ("DisplayGenericCurrency", new CurrencyUpdate(currency, false));
	}

	public void UpdateGenericCurrencyInstant(ICapitalCurrency currency) {
        DisplayCurrency(new CurrencyUpdate(currency, true));
        //StopCoroutine ("DisplayGenericCurrency");
		//StartCoroutine ("DisplayGenericCurrency", new CurrencyUpdate(currency, true));
	}

    private void DisplayCurrency(CurrencyUpdate resUpdate) {
        ICapitalCurrency currency = resUpdate.currency;

        switch (currency.getId()) {
        case "gold" :
            StopCoroutine("DisplayGenericCurrencyGold");
            StartCoroutine("DisplayGenericCurrencyGold",resUpdate);
            break;
        case "hint" :
            StopCoroutine("DisplayGenericCurrencyHint");
            StartCoroutine("DisplayGenericCurrencyHint",resUpdate);
            //DisplayGenericCurrencyHint(resUpdate);
            break;
        case "level" :
            StopCoroutine("DisplayGenericCurrencyLevel");
            StartCoroutine("DisplayGenericCurrencyLevel",resUpdate);
            //DisplayGenericCurrencyLevel(resUpdate);
            break;
        case "ticket" :
            StopCoroutine("DisplayGenericCurrencyTicket");
            StartCoroutine("DisplayGenericCurrencyTicket",resUpdate);
            //DisplayGenericCurrencyTicket(resUpdate);
            break;
		case "silver" :
			StopCoroutine("DisplayGenericCurrencySilver");
			StartCoroutine("DisplayGenericCurrencySilver",resUpdate);
			break;
        }
    }

    private IEnumerator DisplayGenericCurrencyGold(CurrencyUpdate resUpdate) {
        //StopCoroutine ("DisplayGenericCurrency");
        yield return StartCoroutine ("DisplayGenericCurrency", resUpdate);
    }
    private IEnumerator DisplayGenericCurrencyHint(CurrencyUpdate resUpdate) {
        //StopCoroutine ("DisplayGenericCurrency");
        yield return StartCoroutine ("DisplayGenericCurrency", resUpdate);
    }
    private IEnumerator DisplayGenericCurrencyTicket(CurrencyUpdate resUpdate) {
        //StopCoroutine ("DisplayGenericCurrency");
        yield return StartCoroutine ("DisplayGenericCurrency", resUpdate);
    }
    private IEnumerator DisplayGenericCurrencyLevel(CurrencyUpdate resUpdate) {
        //StopCoroutine ("DisplayGenericCurrency");
        yield return StartCoroutine ("DisplayGenericCurrency", resUpdate);
    }
	private IEnumerator DisplayGenericCurrencySilver(CurrencyUpdate resUpdate) {
		//StopCoroutine ("DisplayGenericCurrency");
		yield return StartCoroutine ("DisplayGenericCurrency", resUpdate);
	}      

	private IEnumerator DisplayGenericCurrency(CurrencyUpdate resUpdate) {
		ICapitalCurrency currency = resUpdate.currency;
		bool instant = resUpdate.instant;

		switch (currency.getId()) {
		case "gold" :
            while( !UpdateGenericCurrency(ref displayedGold, CapitalManager.GetInstance().Gold, ref goldText, instant))
				yield return true;
			break;
		case "hint" :
            while( !UpdateGenericCurrency(ref displayedHint, CapitalManager.GetInstance().Hint, ref hintText, instant))
				yield return true;
			break;
		case "level" :
            while( !UpdateGenericCurrency(ref displayedLevel, CapitalManager.GetInstance().Level, ref levelText, instant))
				yield return true;
            break;
		case "ticket" :
            while( !UpdateGenericCurrency(ref displayedTicket, CapitalManager.GetInstance().Ticket, ref ticketText, instant))
				yield return true;
            break;
        case "silver" :
			while( !UpdateGenericCurrency(ref displayedSilver, CapitalManager.GetInstance().Silver, ref silverText, instant))
                yield return true;
            break;
		}
	}
	private bool UpdateGenericCurrency(ref int shownValue, int resManagerValue, ref Text label, bool instant = false) {
		if (instant) {
			label.text = resManagerValue.ToString();
			return true;
		}
		//Debug.LogError("Currency View shown " + shownValue + " Res Man Value " + resManagerValue + " instant " + instant);
		if (shownValue != resManagerValue) {
			int difference = shownValue - resManagerValue;
			if (difference > 2000) shownValue -= 1000;
			else if (difference > 200) shownValue -= 100;
			else if (difference > 20) shownValue -= 10;
			else if (difference > 0) shownValue -= 1;
			else if (difference < -2000) shownValue += 1000;
			else if (difference < -200) shownValue += 100;
            else if (difference < -20) shownValue += 10;
            else if (difference < 0) shownValue += 1;
            label.text = shownValue.ToString();
            return false;
        }
        return true;
    }

	public void UpdateLevel(bool showLevelUp) {
        levelText.text = CapitalManager.GetInstance().Level.ToString();
	}

	private class CurrencyUpdate {
		public ICapitalCurrency currency;
		public bool instant;

		public CurrencyUpdate(ICapitalCurrency currency, bool instant) {
			this.currency = currency;
			this.instant = instant;
		}
	}
}
