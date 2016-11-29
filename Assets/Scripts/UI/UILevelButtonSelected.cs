using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class UILevelButtonSelected : MonoBehaviour {
	LevelSceneData levelSceneData;
	public Text title;

	public void Init(LevelSceneData levelSceneData) {
		this.levelSceneData = levelSceneData;
		title.text = "Level "+levelSceneData.id;
	}

	public void LevelSelected() {
		MazeManager.GetInstance().LoadLevel(GetLevel());
		PopUpManager.GetInstance().ShowPanel(PopUpType.DEFAULT);
	}

	private LevelSceneData GetLevel() {
		//LevelSceneData l = new LevelSceneData();
		return levelSceneData;
	}
}
