using System;

[System.Serializable]
public class GameResponse {
	public EAction action;
	public string response;
	public bool failed;

	public GameResponse(EAction a, string response, bool fail) {
		this.action = action;
		this.response = response;
		this.failed = fail;
	}
}