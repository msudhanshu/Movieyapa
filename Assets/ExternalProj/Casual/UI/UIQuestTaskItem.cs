using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIQuestTaskItem : MonoBehaviour
{
	public Text description;
	public Text status;
	public Button skipButton;
	public Text skipText;

	[HideInInspector]
	public UserQuestTask userQuestTask;

	virtual public void InitialiseWithData(UserQuestTask questTask) {
		this.userQuestTask = questTask;
		description.text = questTask.questTask.frontDescription;
		status.text = questTask.currentCount+" / "+questTask.questTask.requiredQuantity;
		if(questTask.questTask.skipCost > 0){
			skipButton.gameObject.SetActive(true);
			skipText.text = "Skip: "+ questTask.questTask.skipCost + " GOLD";
		}
		else
			skipButton.gameObject.SetActive(false);
	}

	public void skipButtonClick(){
		if(ResourceManager.GetInstance().CanSkip(this.userQuestTask.questTask)){
			Dictionary<IGameResource, int> currentDiffResources = new Dictionary<IGameResource, int>();
			currentDiffResources.Add(DatabaseManager.GetDbResource(DbResource.RESOURCE_GOLD), -this.userQuestTask.questTask.skipCost);
			ResourceManager.GetInstance().AddResources(currentDiffResources);
			ServerAction.takeAction(EAction.QUEST_TASK_SKIP, userQuestTask,ServerSyncManager.GetInstance().serverNotifier, currentDiffResources);
			this.userQuestTask.forceFinishQuestTask(false, false);
			this.updateValue(this.userQuestTask.questTask.requiredQuantity);

		}
	}

	public void updateValue(int currentCount){
		if(this.isCountSufficient()) {
			//Mark Completed
			skipText.text = "Completed";
			status.text = userQuestTask.currentCount+" / "+userQuestTask.questTask.requiredQuantity;
			//placeCompleteStamp();
		} else{
			//Update UI text
			//this.valueLabel.setText(this.questTask.currentQuantity + " / " + this.questTask.requiredQuantity);
		}
	}

	public bool isCountSufficient(){
		if(this.userQuestTask.currentCount >= this.userQuestTask.questTask.requiredQuantity)
			return true;

		return false;
	}
}
