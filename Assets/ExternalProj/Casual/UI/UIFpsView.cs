using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIFpsView : QualityMonitor {
	public string FPS_TITLE = "FPS:";
	private UILabel fpsLabel;
	private int displayedFps;

	public override void Start() {
		base.Start ();
		fpsLabel = GetComponent<UILabel>();
	
	}

	public override void Update() {
		base.Update ();
		UpdateFps();
	}

	public void UpdateFps() {
			displayedFps = (int)fps;
			fpsLabel.text = FPS_TITLE + displayedFps.ToString();
	}

}
