using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIPackageIntroPopup: UIGamePopUp {
   
    private PackageModel package;
    private UserPackage userPackage;

    /**
     * Set up the building with the given building.
     */
    public void Initialise(PackageModel package, UserPackage userPackage) {
        this.package = package;
        this.userPackage = userPackage;
    }

    public void PopulateData(){

          //  Initilized = true;
          //  PopupManager.GetInstance().ShowPanel(panelType);
    }

    public void OnPlayButtonClicked() {
        PackageManager.GetInstance().PlayPackage(this.package,this.userPackage);
    }

    public override void BackButtonClicked() {
        PopUpManager.GetInstance().ShowPanel (PopUpType.PACKAGE_LIST);
    }

}
