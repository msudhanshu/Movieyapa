using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIJamPopup: UIGamePopUp {
   
    /**
     * Set up the building with the given building.
     */
    public void Initialise(ICapitalCurrency cur, int requiredQuantity) {

    }

    public void PopulateData(){

          //  Initilized = true;
          //  PopupManager.GetInstance().ShowPanel(panelType);
    }

    public override void BackButtonClicked() {
        PopUpManager.GetInstance().ShowPanel (PopUpType.DEFAULT);
    }

}
