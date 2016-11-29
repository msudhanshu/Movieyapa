using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using KiwiCommonDatabase;

public interface IEffectData  {
//	public EffectEnum effectType;
//	public List<int> ints;
//	public List<float> floats;
//	public Texture mainTexture;
//	public List<Texture> textures;
//
//	public IEffectData(EffectEnum type, Texture mainTexture) {
//		this.effectType = type;
//		this.mainTexture = mainTexture;
//	}

    EffectEnum getEffectType();
    string getName();
    string getShader();
    bool isMusic();


}
