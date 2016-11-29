using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

public class ServerSyncManager : Manager<ServerSyncManager> {

	private int numUpdates = 0;
	private int DRAW_CALLS_THRESHOLD = 300;
	bool isProgressing = false;

	private int BATCH_MAX_REQUESTS_COUNT = 5;
	private string batchRequestUrl = ServerConfig.BATCH_REQUEST_URL;
	private static string REQ_KEY_PREFIX = "u_";

	private List<ServerRequestParam> pendingRequests = new List<ServerRequestParam>();
	private List<ServerRequestParam> currentRequests = new List<ServerRequestParam>();

	public GenericServerNotifier serverNotifier = new GenericServerNotifier();

	public override void StartInit ()
	{
		ServerAction.Init();
	}

	override public void PopulateDependencies(){}

	public void GetResponseAsync(EAction action, ServerNotifier notifier, string url) {
		Debug.LogWarning("URL : " + url);
		lock(pendingRequests){
			pendingRequests.Add(new ServerRequestParam(action, url, notifier));
		}
	}

	public void GetResponseSync(EAction action, ServerNotifier notifier, string url) {

	}

	public void GetRawResponse(EAction action, ServerNotifier notifier, string url) {
		StartCoroutine(GetRawResponseCoroutine(action,notifier,url));
	}

	IEnumerator GetRawResponseCoroutine(EAction action, ServerNotifier notifier, string url) {
		Debug.Log("URL hit: "+url);
		WWW serverResponse = new WWW(url);
		yield return serverResponse;
		Debug.Log("Ulr="+url+", Response="+serverResponse.text);
		if (serverResponse.error == null || "".Equals(serverResponse.error)) {
			if (notifier != null){
				GameResponse response = new GameResponse(action,serverResponse.text,false);// = JsonConvert.DeserializeObject<GameResponse>(serverResponse.text);
				notifier.OnSuccess(response);
			}
		} else {
			if (notifier != null) notifier.OnFailure(serverResponse.text);
		}
	}

	public void Notify(string url) {
		
	}

	void LateUpdate() {
		//Better way to call the StartSync
		numUpdates++;
		if ((numUpdates > DRAW_CALLS_THRESHOLD || (ServerConfig.BATCHING_ENABLED && pendingRequests.Count >= BATCH_MAX_REQUESTS_COUNT)) && !isProgressing) {
			if (pendingRequests.Count > 0) {
				StartSync();
				isProgressing = true;
			}
			numUpdates = 0;
		}

	}

	void StartSync() {
		lock (pendingRequests) {
			currentRequests.Clear();
			currentRequests.AddRange(pendingRequests);
		}

		//GetResponseFromServerParallel();
		GetResponseFromServerSequential();

		pendingRequests.Clear();
	}

	/*
	 * All Requests are fired in one loop 
	 */
	void GetResponseFromServerParallel() {
		for(int i = 0; i < currentRequests.Count; i++) {
			StartCoroutine(GetResponseFromServer(i));
		}
	}

	/*
	 * A new request is fired only after the completion of previous request 
	 */
	void GetResponseFromServerSequential() {
		if(!ServerConfig.BATCHING_ENABLED)
			StartCoroutine(GetAllResponsesFromServer());
		else
			StartCoroutine(GetBatchedResponsesFromServer());
	}

	IEnumerator GetResponseFromServer(int index) {
		WWW serverResponse = new WWW(currentRequests[index].url);
		yield return serverResponse;

		if (serverResponse.error == null || "".Equals(serverResponse.error)) {
			if (currentRequests[index].notifier != null){
				GameResponse response = JsonConvert.DeserializeObject<GameResponse>(serverResponse.text);
				currentRequests[index].notifier.OnSuccess(response);
			}
		} else {
			if (currentRequests[index].notifier != null) currentRequests[index].notifier.OnFailure(serverResponse.text);
		}

		currentRequests.RemoveAt(index);
		if (currentRequests.Count == 0) 
			isProgressing = false;
	}
	
	IEnumerator GetAllResponsesFromServer() {
		for (int index = 0; index < currentRequests.Count; index++) { 
			WWW serverResponse = new WWW(currentRequests[index].url);
			yield return serverResponse;
			
			if (serverResponse.error == null || "".Equals(serverResponse.error)) {
				if (currentRequests[index].notifier != null) {
					GameResponse response = JsonConvert.DeserializeObject<GameResponse>(serverResponse.text);
					currentRequests[index].notifier.OnSuccess(response);
				}
			
			} else {
				if (currentRequests[index].notifier != null) currentRequests[index].notifier.OnFailure(serverResponse.text);
			}
		}

		currentRequests.Clear();
		isProgressing = false;
	}

	private string getBatchUrlData(){

		StringBuilder batchData = new StringBuilder();
			if(currentRequests.Count > 0){
				StringBuilder appenededUrls = new StringBuilder();
				string appendedUrlsString = null ;
				
				for(int i=0; i <  currentRequests.Count; i++){
					string key = REQ_KEY_PREFIX + i;
					ServerRequestParam param = currentRequests[i]; 
					string requestUrl = WWW.EscapeURL(param.url,Encoding.UTF8);
					appenededUrls.Append("##" + requestUrl);
					batchData.Append("&"  + key + "=" + requestUrl);
					batchData.Append("&verifier_request_url_"+ key +"="+ Utility.Md5Sum(requestUrl) );
					
				}
				batchData.Append("&num_requests=" + currentRequests.Count);
				batchData.Append("&batch_timestamp=" + System.DateTime.Now.Ticks); //FIXME: Verify TimeStamp
				
				appendedUrlsString = appenededUrls.ToString() ;
			//	if(!appendedUrlsString.contains(NEIGHBOR_DIFF_REQ_KEY)) {
					batchData.Append("&user_id=" + Config.USER_ID);
			//		batchData.append("&user_shard=" + Config.USER_SHARD);
			//		batchData.append("&user_social_shard=" + Config.USER_SOCIAL_SHARD) ;
			/*	} else {
					// user_id contains the userId of visting friend
					batchData.append("&user_id=" + Config.FRIEND_USER_ID);
				}*/
				//add md5sum of the batch-url-data
				batchData.Append("&verifier="+  Utility.Md5Sum(appendedUrlsString) );
			}
		return batchData.ToString();
	}	

	IEnumerator GetBatchedResponsesFromServer() {
		string batchedData = getBatchUrlData();

		WWW serverResponse = new WWW(batchRequestUrl,System.Text.Encoding.UTF8.GetBytes(batchedData));
		yield return serverResponse;

		if (serverResponse.error != null && serverResponse.error != ""){
			//TODO:Show Network Error Popup
			yield return -1;
		}
		GameBatchResponse batchResponse = JsonConvert.DeserializeObject<GameBatchResponse>(serverResponse.text);

		if(batchResponse == null || batchResponse.gameResponses == null) yield return -1;
		int batchRequestCount = currentRequests.Count;
		for(int i=0; i < batchRequestCount; i++){
			ServerNotifier notifier = currentRequests[i].notifier;
			if(batchResponse.gameResponses != null && batchResponse.gameResponses[i] != null ){
				if(batchResponse.gameResponses[i].failed){
				//	handleFailure(i, , keyId, ithIdentifier);
				//TODO: Handle failure by recalling the failed url and rest as done in SW
					yield return -1;
				}else{
					// set action also to the game response
					ServerRequestParam reqParam = currentRequests[i];
					if(reqParam != null)
						batchResponse.gameResponses[i].action = reqParam.action;
					currentRequests[i].notifier.OnSuccess(batchResponse.gameResponses[i]);
				}
			}
		}
		currentRequests.Clear();
		isProgressing = false;
	}

}
