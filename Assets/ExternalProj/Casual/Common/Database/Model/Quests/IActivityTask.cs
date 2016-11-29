using UnityEngine;
using System.Collections;
using Object = System.Object;

public interface IActivityTask{
	
	Object GetTarget();
	
	Object GetAction();

	string GetTargetId();
	
	int GetInitialQuantity(ActivityTaskType type);
	
	TaskMap GetNewTaskMap();
	
	bool Activate(ActivityTaskType type,int quantity);

	bool Activate(ActivityTaskType type,int quantity, QuestTask questTask);

	bool ActivateOnRestore(ActivityTaskType type,int quantity, QuestTask questTask);

	void OnComplete(ActivityTaskType type);
	
	void OnFinish(int quantity);
	
	void OnVisitingNeighbor();
	
	void OnReturningHome();
	
}
