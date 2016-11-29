using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UICarrierIntroPopup: UIGamePopUp {
   
    /**
     * Set up the building with the given building.
     */
    public void Initialise() {

    }

    public void PopulateData(){

          //  Initilized = true;
          //  PopupManager.GetInstance().ShowPanel(panelType);
    }

    public void OnPlayButtonClicked() {
        CarrierManager.GetInstance().PlayCarrier();
//        Dictionary<ICapitalCurrency, int> costs = CarrierManager.GetCarrierPlayCost(Config.USER_ID);
//        if (!CapitalManager.GetInstance().CanDeductCurrencys(costs)) {
//            //show jam popup
//            Debug.LogError("Don't have enough currency to play this package:"+Config.USER_ID+", required:"+costs.ToString());
//            return;
//        }
//
//        CapitalManager.GetInstance().DeductCurrencys(costs);
//        CapitalManager.GetInstance().SaveCurrency();
//        PopUpManager.GetInstance().ShowPanel(PopUpType.DEFAULT);
//        CarrierManager.GetInstance().GameStart();
    }

    public override void BackButtonClicked() {
        PopUpManager.GetInstance().ShowPanel (PopUpType.DEFAULT);
    }

}
