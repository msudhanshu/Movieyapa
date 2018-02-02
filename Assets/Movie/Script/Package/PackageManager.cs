using UnityEngine;
using System.Collections;
using SgUnity;
using KiwiCommonDatabase;
using System.Collections.Generic;
using System;
using JSON;

public class PackageManager : Manager<PackageManager>, IQuestionAnsweredCallback
{

    public GameObject questionFrameView;
    private PackageModel activePackage;
    private UserPackage activeUserPackage;
    private string activeQuestionId;
    private int currentQuestionCount = 0;
    override public void PopulateDependencies() { }

    override public void StartInit()
    {
        CapitalManager.OnLevelUpAction += OnLevelUp;
    }

    public void showPackages()
    {
        List<PackageModel> packages =
            //KiwiCommonDatabase.DatabaseManager.GetInstance
            SgGameManager.Instance.wrapper.packages;
        if (packages != null && packages.Count > 0)
        {
            PopUpManager.GetInstance().ShowPanel(PopUpType.PACKAGE_LIST);
            if (UIGamePopUp.activePanel is UIPackageListPopup)
                ((UIPackageListPopup)UIGamePopUp.activePanel).Initialise(packages);
        }
    }

    public void PlayPackage(PackageModel package, UserPackage userPackage)
    {
        this.activePackage = package;
        this.activeUserPackage = userPackage;
        Dictionary<ICapitalCurrency, int> costs = PackageManager.GetPackagePlayCost(this.activePackage, this.activeUserPackage);
        if (!CapitalManager.GetInstance().CanDeductCurrencys(costs))
        {
            //show jam popup
            PopUpManager.GetInstance().ShowPanel(PopUpType.JAM);
            Debug.LogError("Don't have enough currency to play this package:" + package.name + ", required:" + costs.ToString());
            return;
        }
        CapitalManager.GetInstance().DeductCurrencys(costs);
        SgUnity.ServerAction.AddCurrencyAction(costs, true);
        //CapitalManager.GetInstance().SaveCurrency();
        PopUpManager.GetInstance().ShowPanel(PopUpType.DEFAULT);

        GameStart();
    }

    public void GameStart()
    {
        questionFrameView.SetActive(true);
        QCameraController.GetInstance().CloseMainMenuCamera();
        ResetPackageQuestion();
        ShowNextQuestionInPackage();
    }

    private void ResetPackageQuestion()
    {
        currentQuestionCount = 0;
    }

    private void SetNextActiveQuestion()
    {
        //GenerateNextPackageQuestion(int level, string package_id, int currentQuestionIndex);
        this.activeQuestionId = this.activePackage.questionIds[(int)UnityEngine.Random.Range(0, this.activePackage.questionIds.Count)];
    }

    //will be called from congrates popup
    public void ShowNextQuestionInPackage()
    {
        SetNextActiveQuestion();
        currentQuestionCount++;
        QuestionAnswerManager.GetInstance().Reset(this);
        QuestionAnswerManager.GetInstance().ShowNextPackageQuestion(activeQuestionId);
    }

    public void OnCorrectAnswer(QuestionModel q)
    {
        OnAnswering();
    }

    public void OnWrongAnswer(QuestionModel q)
    {
        OnAnswering();
    }

    private void OnAnswering()
    {
        if (CheckIfPackageFinished())
            OnPackageFinish();
        else
        {
            PopUpManager.GetInstance().ShowPanel(PopUpType.PACKAGE_Q_ANSWERED);
        }
    }

    private bool CheckIfPackageFinished()
    {
        int totquestion = Math.Min(this.activePackage.questionIds.Count, this.activePackage.size);
        return (currentQuestionCount > totquestion);
    }

    private void OnPackageFinish()
    {
        CapitalManager.GetInstance().AddCurrency(GetPackagePlayReward(this.activePackage, this.activeUserPackage));
        CapitalManager.GetInstance().SaveCurrency();
        this.activeUserPackage.lockStatus = LockStatus.PLAYED;
        this.activeUserPackage.score = 23;
        this.activeUserPackage.solved = 2;
        SgUnity.ServerAction.UpdateUserPackageAction(this.activeUserPackage, true);
        // OneSignal.SendTag("package_played", "true");
        PopUpManager.GetInstance().ShowPanel(PopUpType.PACKAGE_COMPLETE);
    }

    public void GameEnd()
    {
        QCameraController.GetInstance().GoToMainMenuCamera();
        questionFrameView.SetActive(false);
    }

    public static LockStatus GetLockStatus(PackageModel packageModel, UserPackage userPackage)
    {
        //TODO TEMP
        LockStatus lockStatus = userPackage != null ? userPackage.lockStatus : LockStatus.FORCED_LEVEL_LOCK;
        //put some logic that future lock will not be shown more than 3 level ahead of current level
        return lockStatus;
    }

    public static UserPackage GetUserPackage(PackageModel package)
    {
        try
        {
            return SgGameManager.Instance.wrapper.userPackages.Find(x => x.package_id == package._id);
        }
        catch (Exception e)
        {
            Debug.LogError("UserPackages is null for this package id:" + package._id);
            return null;
        }
    }

    //TODO MANJEET
    public static Dictionary<ICapitalCurrency, int> GetPackagePlayCost(PackageModel package, UserPackage userPackage = null)
    {
        return package.costsMap;
    }

    public static Dictionary<ICapitalCurrency, int> GetPackagePlayReward(PackageModel package, UserPackage userPackage = null)
    {
        return package.rewardsMap;
    }

    public static int GetTimeLockedPackageCost(PackageModel package, UserPackage userPackage = null)
    {
        return (int)(package.timeLockGoldPrice / (float)(package.unlockTime * Util.DAYS_TO_SECONDS) *
            (userPackage.TimeUnlockEndTime - Utility.GetServerTime()));
    }

    public void OnLevelUp()
    {
        List<PackageModel> pkgs = SgGameManager.Instance.wrapper.packages;
        List<UserPackage> userPkgs = SgGameManager.Instance.wrapper.userPackages;
        List<PackageModel> pkgsToActivate = pkgs.FindAll(x =>
            (x.minLevel <= CapitalManager.GetInstance().Level &&
                !(userPkgs.Exists(y => y.package_id == x._id))
            )
        );
        //List<PackageModel> pkgsToActivate = pkgs.FindAll(x => x.minLevel>CapitalManager.GetInstance().Level);
        foreach (PackageModel p in pkgsToActivate)
        {
            Debug.Log("A new package:" + p._id + " is time to unlocked now");
            UserPackage userPackage = new UserPackage(p._id, LockStatus.CURRENCY_TIME_LOCK);
            userPackage.TimeUnlockEndTime = Utility.GetServerTime() + (long)(p.unlockTime * Util.DAYS_TO_SECONDS);
            SgGameManager.Instance.wrapper.userPackages.Add(userPackage);
            SgUnity.ServerAction.UpdateUserPackageAction(userPackage, true);
        }
    }
}
