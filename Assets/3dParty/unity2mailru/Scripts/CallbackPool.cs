using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CallbackPool : MonoSingleton<CallbackPool> {
	public bool initializeAtStart = false;
	public int initPoolSize=5;
	public int currentPoolSize;
	public bool warningOnCallbackExceedLimit = true;
	public int callbackLimit = 10;
	
	Queue<Callback> callbackQueue;
	List <Callback> disposableCallbacks;
	List <Callback> permanentCallback;
	Dictionary<long,Callback> callbackDict;
	
	
	public override void Init ()
	{
		if (initializeAtStart)
			initialize();
	}
	
	
	public Callback getCallback(CallbackType callbackType){
		Callback callback;
		if (callbackQueue.Count>0){
			callback = callbackQueue.Dequeue();	
			callback.reset();
		} else {			
			callback = createNewCallback(); 
		}
		callback.type = callbackType;
		switch (callbackType){
		case CallbackType.DISPOSABLE:
			disposableCallbacks.Add(callback);
			break;
		case CallbackType.PERMANENT:
			permanentCallback.Add(callback);
			break;
		default:
			Debug2.LogError("Wrong callbackType" + callbackType.ToString());			
			break;
		}
		
		return callback; 
	}
	
	public void callbackHandler(string resultString){		
		Debug2.LogDebug("callbackHandler fired with resutlString: \n"+resultString);
		Dictionary<string,object> resultObj=Json.Deserialize(resultString) as Dictionary<string,object>;
		long    callbackId = (long) resultObj["id"];
		Debug2.LogDebug("callbackId="+callbackId);
		object result     = resultObj.ContainsKey("object") ? resultObj["object"] : null;
		Callback callback = callbackDict[callbackId];
		if (callback.action!=null){
			callback.action(result, callback);
		} else {
			Debug2.LogError("callback "+callbackId+" is empty");
		}
		
		if (callback.type == CallbackType.DISPOSABLE){			
			disposableCallbacks.Remove(callback);
			enqueCallback(callback);
		}		
	}
	
	void enqueCallback(Callback callback){
		callback.reset();
		callbackQueue.Enqueue(callback);
	}
	
	public void releasePermanentCallback(Callback callback){	
		Debug2.LogDebug("releasing callback id="+callback.id+"   mruListenerId="+callback.mailruEventId);
		if (permanentCallback.Contains(callback)){
			if (callback.mailruEventId != Callback.UNSET_HTML_EVENT_ID){				
				string eval="mailru.events.remove(CALLBACK_ID);"
									.Replace("CALLBACK_ID",""+callback.mailruEventId);
				Debug2.LogDebug("releasing callback js:\n"+eval);
				Application.ExternalEval(eval);	
			} else {
				Debug2.LogError("something wrong callback id="+callback.id+" permanent but doesnt have htmlEventId to unsubscribe it from mail ru events");
			}			
			permanentCallback.Remove(callback);
			enqueCallback(callback);			
		} else {
			Debug2.LogError("permanent callback doesn't contain callback with id= "+callback.id);
		}
	}
	
	public void releaseDisposableCallback(Callback callback){
		Debug2.LogDebug("releasing disposable callback id="+callback.id);
		disposableCallbacks.Remove(callback);
		enqueCallback(callback);
	}
	
	
	public Callback createNewCallback ()
	{
		currentPoolSize++;
		if (warningOnCallbackExceedLimit && currentPoolSize>=callbackLimit){
			Debug2.LogWarning("total amount of callback is "+currentPoolSize+"\n" +
				" probably you forget to destroy some permanent callback \n" +
				" if you want to increase amount of callback limit change [callbackLimit] in  CallbackPool \n" +
				" If you want to disable this check, please make  [warningOnCallbackExceedLimit] = false");
		}
		Callback c = new Callback();
		callbackDict.Add(c.id,c);
		return c;		
	}
	
	public void setMailruEventId(string parameters){
		Debug2.LogDebug("setMailruEventId params="+parameters);
		 Dictionary<string,object> result=Json.Deserialize(parameters) as Dictionary<string,object>;
		long callbackId=(long)result["callbackId"];
		long mailruEventId=(long)result["mailruEventId"];
		callbackDict[callbackId].mailruEventId=mailruEventId;		
	}
	bool initialized=false;
	public void initialize(){
		Debug2.LogDebug("initializing CallbackPool");
		if (initialized)
			return;
		initialized=true;
		initHtmlJS ();
		initCallbackQueue();
	}
#region initialization		
		
	
	void initCallbackQueue(){
		if (initPoolSize>0){
			callbackDict        = new Dictionary<long, Callback>();
			callbackQueue       = new Queue          <Callback>();
			disposableCallbacks = new List           <Callback>();
			permanentCallback   = new List           <Callback>();
			currentPoolSize = 0;
			int i = initPoolSize;			
			while (i-- > 0){
				Callback c = createNewCallback ();
				callbackQueue.Enqueue(c);
			}
		} else {
			Debug2.LogError("pool size must be greater than 0");
		}
	}
	
	void initHtmlJS ()
	{		
		string commandStringify = @"
			JSON.stringify = JSON.stringify || function (obj) {
			    var t = typeof (obj);
			    if (t != ""object"" || obj === null) {
			        // simple data type
			        if (t == ""string"") obj = '""'+obj+'""';
			        return String(obj);
			    } else {
			        // recurse array or object
			        var n, v, json = [], arr = (obj && obj.constructor == Array);
			        for (n in obj) {
			            v = obj[n]; t = typeof(v);
			            if (t == ""string"") v = '""'+v+'""';
			            else if (t == ""object"" && v !== null) v = JSON.stringify(v);
			            json.push((arr ? """" : '""' + n + '"":') + String(v));
			        }
			        return (arr ? ""["" : ""{"") + String(json) + (arr ? ""]"" : ""}"");
			    }
			};			
		";
		
		string commandCallback = @"
			function callback(id, obj){
				var result=new Object();
				result.id=id;
				result.object=obj;
				var resultString=JSON.stringify(result);				
				u.getUnity().SendMessage('OBJECT_NAME','callbackHandler',resultString);				
			};			
		".Replace("OBJECT_NAME",gameObject.name);	
		
		string commandUpdateCallbackId = @"
			function updateCallbackId(callbackId, mailruEventId){
				var result=new Object();
				result.callbackId=callbackId;
				result.mailruEventId=mailruEventId;
				var resultString=JSON.stringify(result);
				console.log('sending ids to callback'+callbackId+'  mruId'+mailruEventId);
				u.getUnity().SendMessage('OBJECT_NAME','setMailruEventId',resultString);
			};
			
			".Replace("OBJECT_NAME",gameObject.name);
				
		Debug2.LogDebug("1");
		Application.ExternalEval(commandStringify);
		Debug2.LogDebug("2");
		Application.ExternalEval(commandCallback);
		Debug2.LogDebug("3");
		Application.ExternalEval(commandUpdateCallbackId);
		Debug2.LogDebug("4");
	}
#endregion	
	
	
}
