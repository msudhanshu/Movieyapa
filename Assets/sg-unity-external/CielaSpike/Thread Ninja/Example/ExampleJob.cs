using UnityEngine;

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;



//		ExampleJob myJob = new ExampleJob();
//		myJob.url = url;
//		myJob.Start();
//		while( !myJob.Update() )
//		{
//			yield return 0;
//		}
//		error = myJob.dummyServer.error;
//		text = myJob.dummyServer.text;
//		isDone = myJob.dummyServer.isDone;


public class ExampleJob : ThreadedJob
{
	public string url	;
	public	DummyServer dummyServer;
	protected override void ThreadFunction()
	{
		dummyServer = new DummyServer(url);
	}
	protected override void OnFinished()
	{
	}
}

