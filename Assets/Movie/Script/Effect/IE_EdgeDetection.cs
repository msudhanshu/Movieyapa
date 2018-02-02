using UnityEngine;
using System.Collections;

public class IE_EdgeDetection : TextureEffect {

	override public void Init() {
		base.Init();
    }

	override public string ShaderName() {
		return "EffectEdgeDetect";
	}

	// Use this for initialization
	void Awake () {
		Init();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
