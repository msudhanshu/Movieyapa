using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIHomeMainPopup: UIGamePopUp {
   
	public Text miniGameMinLevelLockText;
	public Text packageMinLevelLockText;
//	override protected void Init() {
//		base.Init();
//		activePanel = this;
//	}
//

    /**
     * Set up the building with the given building.
     */
    public void Initialise() {
		miniGameMinLevelLockText.text = SgGameManager.Instance.config.GetStringValue (SgConfigValue.PACKAGE_MINIMUM_LEVEL_LOCK_TAG);
		packageMinLevelLockText.text = SgGameManager.Instance.config.GetStringValue (SgConfigValue.PACKAGE_MINIMUM_LEVEL_LOCK_TAG);
    }

    public void PopulateData(){

          //  Initilized = true;
          //  PopupManager.GetInstance().ShowPanel(panelType);
    }


	void Start() {
			}


    public override void BackButtonClicked() {
        PopUpManager.GetInstance().ShowPanel (PopUpType.DEFAULT);
    }

}
