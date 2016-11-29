using UnityEngine;
using System.Collections;

public class QFrameRoller : MonoBehaviour {
    // public Queue<QFrameController> qFrameController =  new Queue<QFrameController>();

    public RollableReel [] frameReels;
    private int indexOfNextQuestion;
	// Use this for initialization
	void Start () {
        //StartRolling();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StartRolling() {
        StartCoroutine("roll");
    }


//    public RollableReel getNextReel() {
//        
//    }

    IEnumerator roll() {
        float curDist=0;
        float totDist = 5;
        float delay = 1;
        while(curDist < totDist ) {
            for(int i=0 ; i<3; i++) {
                frameReels[i].transform.Translate(curDist,0,0);
            }
            curDist++;
            yield return new WaitForSeconds(delay);    

        }
       // QFrameController
    }

}


public enum FrameReelState {
    PREVIOUS,
    CURRENT,
    NEXT
}
