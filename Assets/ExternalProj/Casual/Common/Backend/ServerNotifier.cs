using UnityEngine;
using System.Collections;

public interface ServerNotifier {

	void OnSuccess(GameResponse response);
	void OnFailure(string error);
}
