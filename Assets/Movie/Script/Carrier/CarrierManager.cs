using UnityEngine;
using System.Collections;
using SgUnity;
using KiwiCommonDatabase;
using System.Collections.Generic;
using System;

public class CarrierManager : Manager<CarrierManager>, IQuestionAnsweredCallback
{

    public GameObject questionFrameView;
    public int carrierQuestionCount = 0;
    public bool testQuestion = false;
    public int testQuestionId = 1;

    override public void PopulateDependencies() { }
    override public void StartInit() { }

    public void StartCarrier()
    {
        ResetCarrierQuestion();
        //		PopUpManager.GetInstance().ShowPanel(PopUpType.DEFAULT);
        PopUpManager.GetInstance().ShowPanel(PopUpType.CARRIER_INTRO);
        if (UIGamePopUp.activePanel is UICarrierIntroPopup)
            ((UICarrierIntroPopup)UIGamePopUp.activePanel).Initialise();
    }

    private void ResetCarrierQuestion()
    {
        carrierQuestionCount = 0;
    }

    public void PlayCarrier()
    {
        Dictionary<ICapitalCurrency, int> costs = GetCarrierPlayCost(CapitalManager.GetInstance().Level);
        if (!CapitalManager.GetInstance().CanDeductCurrencys(costs))
        {
            PopUpManager.GetInstance().ShowPanel(PopUpType.JAM);
            Debug.LogError("Don't have enough currency to play this , required:" + costs.ToString());
            return;
        }
        CapitalManager.GetInstance().DeductCurrencys(costs);
        //SgUnity.ServerAction.AddCurrencyAction(costs,true);
        CapitalManager.GetInstance().SaveCurrency();
        PopUpManager.GetInstance().ShowPanel(PopUpType.DEFAULT);
        GameStart();
    }

    public void GameStart()
    {
        questionFrameView.SetActive(true);
        QCameraController.GetInstance().CloseMainMenuCamera();
        ResetCarrierQuestion();
        ShowNextQuestionInCarrier();
    }

    private void MarkAsSolved(QuestionModel q)
    {
        QSolvedItemModel qsolved = new QSolvedItemModel(q._id, q.level);
        SgGameManager.Instance.wrapper.userSolvedQuestions.Add(qsolved);
        SgGameManager.Instance.wrapper.unSolvedQuestionsWithLevel.RemoveAll(qlevelItemModel => qlevelItemModel.id == q._id);
        SgRemoteAction.CarrierQSolved(qsolved);
    }

    //will be called from congrates popup
    public void ShowNextQuestionInCarrier()
    {
        carrierQuestionCount++;
        QuestionAnswerManager.GetInstance().Reset(this);
        QuestionAnswerManager.GetInstance().ShowNextCarrierQuestion();
    }

    public void OnCorrectAnswer(QuestionModel q)
    {
        CapitalManager.GetInstance().AddCurrency(CurrencyModel.CURRENCY_GOLD, 20);
        CapitalManager.GetInstance().SaveCurrency();
        MarkAsSolved(q);
        if (CheckIfCarrierLevelFinished())
            OnCarrierLevelFinish();
        else
        {
            PopUpManager.GetInstance().ShowPanel(PopUpType.CARRIER_Q_ANSWERED);
        }
    }

    public void OnWrongAnswer(QuestionModel q)
    {
        PopUpManager.GetInstance().ShowPanel(PopUpType.CARRIER_FAIL);
    }

    private bool CheckIfCarrierLevelFinished()
    {
        return (carrierQuestionCount > GetCarrierQuestionCount(CapitalManager.GetInstance().Level));
    }

    //called from ui
    private void OnCarrierLevelFinish()
    {
        CapitalManager.GetInstance().AddCurrency(GetCarrierPlayReward(CapitalManager.GetInstance().Level));
        CapitalManager.GetInstance().DoLevelUp();
        //SgUnity.ServerAction.UpdateLevelAction(CapitalManager.GetInstance().Level);
        CapitalManager.GetInstance().SaveCurrency();
        PopUpManager.GetInstance().ShowPanel(PopUpType.CARRIER_LEVEL_UP);
    }

    public void GameEnd()
    {
        QCameraController.GetInstance().GoToMainMenuCamera();
        questionFrameView.SetActive(false);

        //temp
        MovieGameManager.GetInstance().InitHomeMainScreen();
    }

    public static int GetCarrierQuestionCount(int Level)
    {
        //return GameParamModel.carrier_question_default_count;
        return (int)SgGameManager.Instance.config.GetLongValue(SgConfigValue.CARRIER_QCOUNT_IN_ALEVEL_TAG);
    }

    public static Dictionary<ICapitalCurrency, int> GetCarrierPlayCost(int Level)
    {
        return GameParamModel.carrier_play_default_cost;
    }

    public static Dictionary<ICapitalCurrency, int> GetCarrierPlayReward(int Level)
    {
        return GameParamModel.carrier_play_default_reward;
    }


    public enum CarrierReplayType
    {
        NEW,
        REPLAY,
        RETRY
    }

}
