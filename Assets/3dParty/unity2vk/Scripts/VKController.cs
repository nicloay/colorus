using UnityEngine;
using System.Collections;
using System;
using System.Collections.Specialized;

public class VKController : MonoSingleton<VKController> {
	public string privateKey;
	public bool initOnStart=false;
	public Action<bool> onApiReady;
	public NameValueCollection inputData;  
	CallbackPool callbackPool;
	bool initialized=false;
	
	public override void Init ()
	{
		Debug2.Log("init MRUController");
		base.Init ();		
		
		if (initOnStart)
			initializeVKApi();		
	}
	
	
	Callback callBackOnInitDone;
	Callback callbackOnInitError;	
	public void initializeVKApi(string key=""){
		if (!String.IsNullOrEmpty(key))
			privateKey = key;
		if (initialized){
			Debug2.LogWarning("vk api already initialized, init method will be ignored");
		} else {
			initialized=true;
			callbackPool = CallbackPool.instance;
			callbackPool.initialize();
			initVKApi(privateKey, 
				delegate(object obj,Callback callback){
					inputData = HTTPUtility.ParseQueryString ((string)obj);
					Debug2.LogDebug("viewer id ="+ inputData["viewer_id"]);
					callbackPool.releaseDisposableCallback(callbackOnInitError);
					if (onApiReady!=null)
						onApiReady(true);
					Debug2.LogDebug("VK api is ready");				
				},
				delegate(object obj,Callback callback){
					onApiReady(false);
					callbackPool.releaseDisposableCallback(callBackOnInitDone);
					Debug2.LogError("problem with vk initialization\n"+Json.Serialize(obj));
				});
			
		}
		
	}
	
	public void callMethod(string methodName,params object[] parameters){
		string jsParams="";
		for (int i = 0; i < parameters.Length; i++) {
			if (parameters[i] is string)
				jsParams +=", \""+(string)parameters[i]+"\"";
			else if (parameters[i] is bool)
				jsParams += ", " + ((bool)parameters[i]).ToString().ToLower();
			else if (parameters[i] is int)
				jsParams += ", " + parameters[i];
		}
		string eval=@"
			VK.callMethod('METHOD_NAME'PARAMETERS);
		".Replace("METHOD_NAME",methodName)
			.Replace("PARAMETERS",jsParams);
		Debug2.LogDebug("callMethod external evaluation \n"+eval);
		Application.ExternalEval(eval);		
	}
	
	public void api(string methodName, object paramObject, Action<object,Callback> callback){
		Callback c=callbackPool.getCallback(CallbackType.DISPOSABLE);
		c.action = callback;		
		string eval=@"
			function _callbackCALLBACK_ID(result){ 
				callback(CALLBACK_ID, result); 
			}
			var params=PARAMS_JSON_STRING;
			VK.api(""METHOD_NAME"",params,_callbackCALLBACK_ID);
		".Replace("CALLBACK_ID",""+c.id)
			.Replace("METHOD_NAME",methodName)
				.Replace("PARAMS_JSON_STRING",Json.Serialize(paramObject));
		Debug2.LogDebug("vk.api external evaluation: \n"+eval);
		Application.ExternalEval(eval);
	}
	
	
	void initVKApi(string key, Action<object, Callback> onInitDone, Action<object, Callback> onInitError){
		callBackOnInitDone = callbackPool.getCallback(CallbackType.DISPOSABLE);
		callBackOnInitDone.action = onInitDone;
		callbackOnInitError = callbackPool.getCallback(CallbackType.DISPOSABLE);
		callbackOnInitError.action = onInitError;
		string eval=@"
			function _callbackCALLBACK_ID(result){ 
				result=location.search;
				callback(CALLBACK_ID, result); 
			}
			VK.init(function() { 
			     _callbackCALLBACK_ID();
		  	}, function() { 
			     callback(CALLBACK_ON_ERROR_ID);
			}, '5.1'); 
		".Replace("CALLBACK_ID",""+callBackOnInitDone.id)
			.Replace("CALLBACK_ON_ERROR_ID",""+callbackOnInitError.id);
		Debug2.LogDebug("initVKApi external evaluation: \n"+eval);
		Application.ExternalEval(eval);
	}
	
	
	public void windowConfirm(string text, Action<object, Callback> onDone){
		Callback callback = callbackPool.getCallback(CallbackType.DISPOSABLE);
		callback.action = onDone;
		string eval=@"
			var result=confirm('TEXT');
			callback(CALLBACK_ID, result);
		".Replace("TEXT",text.Replace("'","\"")).Replace("CALLBACK_ID",""+callback.id);
		Debug2.LogDebug("windowConfirm external evaluation:\n "+eval);	
		Application.ExternalEval(eval);
	}
	
	
	public void uploadTexture(Texture2D texture, string url, string name, Action<string> callbackOnSuccess, Action<string> callbackOnError){
		WWWForm form = new WWWForm();
		form.AddBinaryData("photo", texture.EncodeToPNG(), name+".png", "image/png");
		StartCoroutine(callbackOnWWWResponse(url,form,callbackOnSuccess,callbackOnError));
	}
	
	
	IEnumerator callbackOnWWWResponse(string URL, WWWForm form, Action<string> callbackOnSuccess, Action<string> callbackOnError){
		WWW www = new WWW(URL, form);
		yield return www;

		if (www.error != null)
			callbackOnError(www.error);
		else			
			callbackOnSuccess(www.text);
		www = null;
	}
}
