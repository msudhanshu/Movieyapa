using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIMiniGameListPopup: UIGamePopUp {
    public GameObject packageItemPrefab;

    public GameObject packageListPanel;
    private List<PackageModel> packages;
    /**
     * Set up the building with the given building.
     */
    public void Initialise(List<PackageModel> packages) {
        this.packages = packages;
    }


    private List<UIPackageItem> uiPackageItems;
    public void PopulateData(){
        if(uiPackageItems==null)
            uiPackageItems = new List<UIPackageItem>();
        ClearAllItems();

        foreach(PackageModel model in packages) {
                Transform go = Pool.Instantiate(packageItemPrefab);
                UIPackageItem levelB = go.GetComponent<UIPackageItem>();
                 go.parent = packageListPanel.transform;
                go.name = "Package_"+model.name;
                 uiPackageItems.Add (levelB);
                levelB.Init (model);
            }
          //  Initilized = true;
          //  PopupManager.GetInstance().ShowPanel(panelType);
    }

    private void ClearAllItems() {
        if(uiPackageItems!=null) {
            foreach(UIPackageItem item in uiPackageItems) {
                Pool.Destroy(item.gameObject);
            }
            uiPackageItems.Clear();
        }
    }

    /**
     * Show the panel.
     */ 
    override protected IEnumerator DoShow() {
        yield return new WaitForSeconds(UI_DELAY / 3.0f);
        content.SetActive (true);
        PopulateData();
        if(animator!=null)
            animator.SetTrigger("Opening");
    }


    /**
     * Reshow the panel (i.e. same panel but for a different object/building).
     */ 
    override protected IEnumerator DoReShow() {
        //  iTween.MoveTo(content, hidePosition, UI_DELAY);
        yield return new WaitForSeconds(UI_DELAY / 3.0f);
        PopulateData();
        if(animator!=null)
            animator.SetTrigger("Opening");
    }

    public override void BackButtonClicked() {
        PopUpManager.GetInstance().ShowPanel (PopUpType.DEFAULT);
    }

}
