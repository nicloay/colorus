using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(CallbackPool))]
public class MRUController : MonoSingleton<MRUController> {
	CallbackPool callbackPool;
	public string privateKey;
	public bool initOnStart=false;
	public Action<object> onApiReady;
	public Dictionary<string,object> mailruSession;
	
	public override void Init ()
	{
		base.Init ();		
		
		if (initOnStart)
			initializeMailRuApi();		
	}
	
	public void initializeMailRuApi(){
		callbackPool = CallbackPool.instance;
		callbackPool.initialize();
		initMailruApi(privateKey, delegate(object obj,Callback callback){
			if (onApiReady!=null)
				onApiReady(obj);
			mailruSession = obj as Dictionary<string,object>;
			Debug2.LogDebug("mail ru api ready");				
		});
	}
	
	public static void initMailruApi(string privateKey, Action<object, Callback> onInitDone){		
		Callback c = CallbackPool.instance.getCallback(CallbackType.DISPOSABLE);
		c.action = onInitDone;		
		string eval= @"
			mailru.loader.require('api', function() {
				mailru.app.init('PRIVATE_KEY');

				callback(CALLBACK_ID,mailru.session);
       		});
		".Replace("PRIVATE_KEY",privateKey).Replace("CALLBACK_ID", ""+c.id);
		Debug2.LogDebug("initMailruApi external evaluation: \n"+eval);
		Application.ExternalEval(eval);
	}
	
	
	public void callMailruByParams(string functionName, params string[] parameters){		
		string functionParmas=string.Join("', '",parameters);
		functionParmas = (functionParmas.Length > 0) ? "'"+functionParmas+"'" : "";
		string eval=@"
			FUNCTION_NAME(PARAMETERS);
		".Replace("FUNCTION_NAME", functionName)
			.Replace("PARAMETERS"  , functionParmas);	
		Debug2.LogDebug("callMailruByParams external evaluation: \n"+eval);
		Application.ExternalEval(eval);
	}

	
	public void callMailruByCallbackAndParams(string functionName, Action<object, Callback> action, params object[] parameters){		
		Callback callback= callbackPool.getCallback(CallbackType.DISPOSABLE);
		callback.action = action;
		string initialization="";
		string functionParams="";
		for (int i = 0; i < parameters.Length; i++) {
			initialization+=@"
				paramNUM=PARAMETER_JSON;	
			".Replace("NUM",""+i)
				.Replace("PARAMETER_JSON",Json.Serialize(parameters[i]));
			functionParams+=", paramNUM".Replace("NUM",""+i);
		}
		string eval=@"
			INITIALIZATION
			
			function _callbackCALLBACK_ID(result){ 
				callback(CALLBACK_ID, result); 
			}			

			FUNCTION_NAME(_callbackCALLBACK_ID ADDITIONAL_PARAMETERS);
		".Replace("INITIALIZATION",initialization)
			.Replace("CALLBACK_ID",""+callback.id)
				.Replace("FUNCTION_NAME", functionName)
				.Replace("ADDITIONAL_PARAMETERS", functionParams);
				
				
		Debug2.LogDebug("callMailruByCallbackAndParams external evaluation: \n"+eval);
		Application.ExternalEval(eval);
	}
	
	public void callMailruByCallback(string functionName, Action<object, Callback> action){		
		Callback callback= callbackPool.getCallback(CallbackType.DISPOSABLE);
		callback.action = action;
		
		string eval=@"
			function _callbackCALLBACK_ID(result){ 
				callback(CALLBACK_ID, result); 
			}
			FUNCTION_NAME(_callbackCALLBACK_ID);
		".Replace("FUNCTION_NAME", functionName)
			.Replace("CALLBACK_ID"  , ""+callback.id);
		
		Debug2.LogDebug("callMailruByCallback external evaluation: \n"+eval);
		Application.ExternalEval(eval);
	}
	
	public void callMailruByObjectMailruListenerAndCallback(string functionName, object paramsObject, string mailruListenEvent, Action<object, Callback> action){
		Callback callback = callbackPool.getCallback(CallbackType.PERMANENT);		
		Debug2.LogDebug("callbackType="+callback.type.ToString());
		callback.action = action;
		string eval=@"
			function _callbackCALLBACK_ID(result){ 
				result.originalProps=PARAMS_JSON_STRING;
				callback(CALLBACK_ID, result); 
			}
			var mailruEventId=mailru.events.listen(MAIL_RU_LISTEN_EVENT, _callbackCALLBACK_ID);
			updateCallbackId(CALLBACK_ID, mailruEventId);
			var params=PARAMS_JSON_STRING;
			FUNCTION_NAME(params);
		".Replace("MAIL_RU_LISTEN_EVENT", mailruListenEvent)
		  .Replace("CALLBACK_ID"         , ""+callback.id)
		  .Replace("PARAMS_JSON_STRING"  , 	Json.Serialize(paramsObject))
		  .Replace("FUNCTION_NAME"       , functionName);

		Debug2.LogDebug("callMailruByObjectMailruListenerAndCallback external evaluation: \n"+eval);
		
		Application.ExternalEval(eval);
	}
		
	public void uploadTexture(Texture2D tex, Action<object, Callback> action)
	{
		string b64string =System.Convert.ToBase64String(tex.EncodeToPNG());
	 	Callback callback = callbackPool.getCallback(CallbackType.DISPOSABLE);		
		callback.action = action;
		
		string eval=@"
			var data = new FormData();
			data.append('imgdata','BASE64_STRING')			
			$.ajax({
				url         : 'saveimage.php',
				data        : data,
				cache       : false,
				contentType : false,
				processData : false,
				type        : 'POST',
				success     : function(data){					
					var url = window.location.href;
					var fullPath=url.substring(0, url.lastIndexOf('/') + 1)+data;
					callback(CALLBACK_ID, fullPath);					
				}
		});
		".Replace("BASE64_STRING",b64string)
		 .Replace("CALLBACK_ID",""+callback.id);
		Debug2.LogDebug("uploadTexture external evaluation: \n"+eval);
		Application.ExternalEval(eval);
	}
	
	public void removeTextureFromServer(string imgFullPath){		
		string eval=@"
			var img='IMG_FULL_PATH';
			var fileName=img.substring(img.lastIndexOf('/')+1);
			$.get( 'removeimage.php', { imageName: fileName} );	
		".Replace("IMG_FULL_PATH",imgFullPath);
		Debug2.LogDebug("removeTextureFromServer external evaluation: \n"+eval);
		Application.ExternalEval(eval);			
	}
	
}