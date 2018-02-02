using UnityEngine;
using System.Collections;

public class IE_Default : TextureEffect {

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

	virtual public Shader GetShader() {
		return Shader.Find("Diffuse");
	}
}
