using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIPackageItem : MonoBehaviour {

    public RawImage image;
    public Text name;
    private PackageModel package;
    private UserPackage userPackage;
    public LockStatus lockStatus;
    public Image justUnlockImage;
    public Image levelLockImage;
    public Image timeLockImage;
    public Text leveLockLevel;
    public Text timeLockCost;
    public Text timeLockCountDown;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if(userPackage!=null && userPackage.lockStatus==LockStatus.CURRENCY_TIME_LOCK) {
            if(userPackage.TimeUnlockEndTime > Utility.GetServerTime())  {
                //Timer coutdown
                timeLockCountDown.text = timeLockCountDownString();
            } else {
                Debug.Log("A package "+package._id+" is just moved from time lock to just unlocked");
                userPackage.lockStatus = LockStatus.JUST_UNLOCKED;
                lockStatus = userPackage.lockStatus;
                SgUnity.ServerAction.UpdateUserPackageAction(userPackage,true);
                UpdateLockUI();
            }
        }
	}
       
    public void Init(PackageModel packageModel) {
        UserPackage u = PackageManager.GetUserPackage(packageModel);
        Init(packageModel,u);
    }

    public void Init(PackageModel packageModel, UserPackage userPackage) {
        this.userPackage = userPackage;
        this.package = packageModel;
        name.text = packageModel.name;
        //packageModel.logo = "Hangar";
        string logoNameForRes = packageModel.logo.Split('.')[0];
        Debug.Log("Loading sprite from resource:"+logoNameForRes);
        image.texture = (Texture)Resources.Load<Texture>(logoNameForRes);
        lockStatus = PackageManager.GetLockStatus(packageModel,userPackage);
        UpdateLockUI();
    }

    private void UpdateLockUI() {
        justUnlockImage.gameObject.SetActive(false);
        levelLockImage.gameObject.SetActive(false);
        timeLockImage.gameObject.SetActive(false);
        switch(lockStatus) {
            case LockStatus.INVISIBLE:
                this.gameObject.SetActive(false);
                break;
            case LockStatus.FORCED_LEVEL_LOCK:
                levelLockImage.gameObject.SetActive(true);
                leveLockLevel.text = package.minLevel+"";
                break;
            case LockStatus.CURRENCY_TIME_LOCK:
                timeLockImage.gameObject.SetActive(true);
                timeLockCost.text = PackageManager.GetTimeLockedPackageCost(package,userPackage)+"";
                timeLockCountDown.text = timeLockCountDownString();
                break;
            case LockStatus.JUST_UNLOCKED:
                justUnlockImage.gameObject.SetActive(true);
                break;
            default:
            break;
        }
    }

    private string timeLockCountDownString() {      
        return Utility.ToDateTimeString(userPackage.TimeUnlockEndTime - Utility.GetServerTime());
    }

    public void onItemButtonClicked() {
        switch(lockStatus) {
            case LockStatus.FORCED_LEVEL_LOCK:
                PopUpManager.GetInstance().ShowPanel (PopUpType.PACKAGE_FORCELEVEL_LOCK);
                if (UIGamePopUp.activePanel is UIPackageLevelLockPopup)
                ((UIPackageLevelLockPopup)UIGamePopUp.activePanel).Initialise(package,userPackage);
            
                break;
            case LockStatus.CURRENCY_TIME_LOCK:
                PopUpManager.GetInstance().ShowPanel (PopUpType.PACKAGE_TIME_LOCK);
                if (UIGamePopUp.activePanel is UIPackageTimeLockPopup)
                ((UIPackageTimeLockPopup)UIGamePopUp.activePanel).Initialise(package,userPackage);
            
                break;
            case LockStatus.JUST_UNLOCKED:
                 userPackage.lockStatus = LockStatus.UNPLAYED;
                ShowIntroPopup();
                break;
            case LockStatus.UNPLAYED:
            case LockStatus.PLAYED:
            case LockStatus.WELLPLAYED:
            default:
                ShowIntroPopup();
                break;

        }
    }

    private void ShowIntroPopup() {
        PopUpManager.GetInstance().ShowPanel (PopUpType.PACKAGE_INTRO);         
        if (UIGamePopUp.activePanel is UIPackageIntroPopup)
            ((UIPackageIntroPopup)UIGamePopUp.activePanel).Initialise(package,userPackage);
    }
}
