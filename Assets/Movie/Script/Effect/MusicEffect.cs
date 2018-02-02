using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MusicEffect : Effect {
    public AudioClip audioClip;
   
    private void ReloadEffect() {
        //base.ReloadEffect();
        //TODO
	}

    override public void ReloadEffect<T>(T asset) {
		base.ReloadEffect(asset);
        audioClip = asset as AudioClip;
        ReloadEffect();
    }
	
}
