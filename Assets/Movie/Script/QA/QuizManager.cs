using UnityEngine;
using System.Collections;
using SgUnity;

//DEPRECATED
public class QuizManager : Manager<QuizManager> {
	
    public GameObject questionFrameView;

	override public void PopulateDependencies() {}
	
	override public void StartInit() {
		
	}

//	public void GameStart() {
//        questionFrameView.SetActive(true);
//        QCameraController.GetInstance().CloseMainMenuCamera();
//		QuestionAnswerManager.GetInstance().Reset();
//		QuestionAnswerManager.GetInstance().ShowNextQuestion();
//	}
//
//	public void GameEnd() {
//        QCameraController.GetInstance().GoToMainMenuCamera();
//        questionFrameView.SetActive(false);
//	}
}
