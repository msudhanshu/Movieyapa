using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UICarrierLevelUpPopup: UIGamePopUp {
   
    /**
     * Set up the building with the given building.
     */
    public void Initialise() {

    }

    public void PopulateData(){

          //  Initilized = true;
          //  PopupManager.GetInstance().ShowPanel(panelType);
    }

    public override void BackButtonClicked() {
        PopUpManager.GetInstance().ShowPanel (PopUpType.DEFAULT);
        CarrierManager.GetInstance().GameEnd();
    }

}
