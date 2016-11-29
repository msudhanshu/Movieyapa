using System;

public class ServerRequestParam {
    public EAction action;
	public string url;
	public ServerNotifier notifier;

    public ServerRequestParam(EAction action, string url, ServerNotifier notifier = null){
		this.action = action;
		this.url = url;
		this.notifier = notifier;
	}

	public ServerRequestParam(){
	}
}

