using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UILoadingPopup: UIGamePopUp {
	public Image loadingBarImage;
	public GameObject companyLogo;
	public GameObject gameLogo;
	public Text progressText;
	public float Value
	{
		get 
		{
			if(loadingBarImage != null)
				return (loadingBarImage.fillAmount*100);	
			else
				return 0;	
		}
		set 
		{
			if (loadingBarImage != null) {
				loadingBarImage.fillAmount = value / 100f;
			}
		} 
	}
		
	public float countdown = 0 ;
	public float companyLogoDuration = 2 ;
	public float gameLogoMinDuration = 1 ;
	public float totalProgressDuration = 4.0f;
	public bool canFinishLoadingBar = false;
	
	void Start() {
		SgGameManager.OnGameInited += OnGameInitialised;
		Reset ();
		UpdateProgress (50);
	}

	void Reset() {
		countdown = 0;
		Value = 0;
		canFinishLoadingBar = false;
		companyLogo.SetActive (true);
		UpdateProgressBarText ();
	}

	void Update() {
		if (!canFinishLoadingBar) {
			countdown += Time.deltaTime;
			if (countdown >= companyLogoDuration + gameLogoMinDuration) {
				canFinishLoadingBar = true;
				OnFullProgress ();
			} else if (countdown >= companyLogoDuration) {
				companyLogo.SetActive (false);
			}
		}
	}

	public void OnGameInitialised() {
		UpdateProgress (100);
	}

	private float progressDiffToTime(float diff) {
		return totalProgressDuration / 100.0f * diff;
	}

	public void UpdateProgress(float progress) {
		string progressItween = "progressitween";
		iTween.Stop (gameObject);

		float diff = progress - Value;

		Hashtable param = new Hashtable();
		param.Add("from",Value);
		param.Add("type",progressItween);
		param.Add("to", progress);
		param.Add("time", progressDiffToTime(diff));
		param.Add("onupdate", "TweenedSomeValue");         
		param.Add("onComplete", "OnFullProgress");
		param.Add("onCompleteTarget", gameObject);
		iTween.ValueTo (this.gameObject, param);
	}

	public void TweenedSomeValue(int val){
		Value = val;
		UpdateProgressBarText ();
	}

	void UpdateProgressBarText() {
		progressText.text = Value + "%";
	}

	public void OnFullProgress()
	{
		if (Value >= 100) {
			// show completed string
			if (canFinishLoadingBar) {
				Hide ();
			}
		}
//		int offset = 5;
//		if (Value <= offset) {
//			UpdateProgress (100);
//		} else if (Value >= 100 - offset) {
//			UpdateProgress (0);
//		}
	}

    public override void BackButtonClicked() {
        //close the app ... show exit popup
    }

}
