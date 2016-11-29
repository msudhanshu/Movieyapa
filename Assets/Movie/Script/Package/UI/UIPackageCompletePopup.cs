using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIPackageCompletePopup: UIGamePopUp {
   
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
        PackageManager.GetInstance().GameEnd();
        PopUpManager.GetInstance().ShowPanel (PopUpType.PACKAGE_LIST);
//        PackageManager.GetInstance().PlayPackage(this.package);
    }

    public override void BackButtonClicked() {
        PackageManager.GetInstance().GameEnd();
        PopUpManager.GetInstance().ShowPanel (PopUpType.PACKAGE_LIST);
    }

}
