using UnityEngine;
using System.Collections;

public class UILevelController : UIGamePopUp {
	private bool Initilized = false;
	public GameObject levelPrefab;

	void Start() {
		DataHandler.OnDiffFinished+=OnDiffFinishedCallback;
	}

	public void OnDiffFinishedCallback() {
		StartCoroutine(PopulateLevel());
	}

	IEnumerator PopulateLevel() {
		while(DataHandler.wrapper == null || DataHandler.wrapper.levelSceneData==null) yield return 0;
		foreach ( LevelSceneData lsd in DataHandler.wrapper.levelSceneData ) {
			Transform go = Pool.Instantiate(levelPrefab);
			UILevelButtonSelected levelB = go.GetComponent<UILevelButtonSelected>();
			go.parent = content.transform;
			go.name = "Level_"+lsd.id;
			levelB.Init (lsd);
		}
		Initilized = true;
		PopUpManager.GetInstance().ShowPanel(panelType);
		yield break;
	}

}
