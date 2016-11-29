using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UICarrierQAnsweredPopup: UIGamePopUp {
   
    /**
     * Set up the building with the given building.
     */
    public void Initialise() {

    }

    public void PopulateData(){

          //  Initilized = true;
          //  PopupManager.GetInstance().ShowPanel(panelType);
    }

    public void OnNextButtonClicked() {
        PopUpManager.GetInstance().ShowPanel(PopUpType.DEFAULT);
        CarrierManager.GetInstance().ShowNextQuestionInCarrier();
    }

//    public override void BackButtonClicked() {
//        PopUpManager.GetInstance().ShowPanel (PopUpType.DEFAULT);
//    }

}
