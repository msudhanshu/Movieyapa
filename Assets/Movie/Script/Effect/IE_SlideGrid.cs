using UnityEngine;
using System.Collections;

public class IE_SlideGrid : TextureEffect {

	override public void Init() {
		base.Init();
    }

	// Use this for initialization
	void Awake () {
		Init();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	override public string ShaderName() {
		return "EffectSlidingGrid";
	}
}
