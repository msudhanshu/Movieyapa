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
        audioClip = asset as AudioClip;
        ReloadEffect();
        base.ReloadEffect(asset);
    }
	
}
