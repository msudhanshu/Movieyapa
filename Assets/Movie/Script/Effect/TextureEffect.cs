using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TextureEffect : Effect {
    [HideInInspector]
    public Texture mainTexture;
	protected Shader shader;
	private Material _baseMaterial;
	protected Material baseMaterial {
		get {
			if(_baseMaterial == null)
				_baseMaterial = GetComponent<Renderer>().material;
			return _baseMaterial;
		}
	}

	private void ReloadEffect() {
        //base.ReloadEffect();
        shader = (Shader)Resources.Load<Shader>(effectData.getShader());
		baseMaterial.shader = shader;
		baseMaterial.mainTexture = mainTexture;

	}

    override public void ReloadEffect<T>(T asset) {
        mainTexture = asset as Texture;
        ReloadEffect();
        base.ReloadEffect(asset);
    }

}
