using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TextureEffect : Effect {
    [HideInInspector]
    public Texture mainTexture;
//	private Material _baseMaterial;
//	protected Material baseMaterial {
//		get {
//			if(_baseMaterial==null)
//				_baseMaterial = GetComponentInChildren<Renderer> ().material;
//			return _baseMaterial;
//		} set {
//
//		}
//	}

	virtual public string ShaderName() {
		return "Diffuse";
	}

	virtual public Shader GetShader() {
		Shader shader = (Shader)Resources.Load<Shader>(ShaderName());
		if (shader == null) {
			shader = Shader.Find("Diffuse");
		}
		return shader;
	}

	private void ReloadImageEffect() {
		Material baseMaterial = GetComponentInChildren<Renderer>().material;
		baseMaterial.shader = GetShader();
		baseMaterial.mainTexture = mainTexture;
	}

    override public void ReloadEffect<T>(T asset) {
		base.ReloadEffect(asset);
        mainTexture = asset as Texture;
        ReloadImageEffect();
    }

}
