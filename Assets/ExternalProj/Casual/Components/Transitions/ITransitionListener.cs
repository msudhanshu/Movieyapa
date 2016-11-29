using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ITransitionListener {

	AssetState GetAssetState();
	void SetAssetState(AssetState state);

	void TransitionStarted(StateTransition transition);
	void TransitionProgressed(StateTransition transition);
	void TransitionCompleted(StateTransition transition);

	void SyncState(Dictionary<IGameResource, int> diffResources);
	void SyncActivity(Dictionary<IGameResource, int> diffResources);

/*	void SetAssetState(AssetState state);
	AssetState GetAssetState();*/
}
