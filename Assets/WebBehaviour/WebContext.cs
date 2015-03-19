using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using localisation;

public enum WebType{
	ONLINE,
	MAIL_RU,
	VK,
	FB
}

public class WebContext : MonoSingleton<WebContext> {
	public string mailRuKey;
	public string vkKey;
	WebType webType;
	public Action<object,Callback> onWebContextReady;
	bool ready = false;
	public Dictionary<WebType,WebStrategyInt> supportedWeb;
	string initData;
	WebStrategyInt activeStrategy;
	
#if UNITY_WEBPLAYER	
	public override void Init ()
	{		
		CallbackPool.instance.initialize();	
		onWebContextReady+=onWebContextReadyListener;				
		WorkspaceEventManager.instance.onSheetChange+=onSheetChangeListener;		
		WorkspaceEventManager.instance.onSavePictureClick+=onSavePictureListener;
		supportedWeb=new Dictionary<WebType, WebStrategyInt>(){
			{WebType.MAIL_RU, new MailRuStrategyImpl()},
			{WebType.VK     , new VKStrategyImpl()},
			{WebType.ONLINE , new StubStrategyImpl()},
			{WebType.FB     , new StubStrategyImpl()},
		};
		setWebType();
	}
#else 
	public override void Init ()
	{		
		DestroyImmediate(this.gameObject);
	}
#endif	
	void onSavePictureListener(){
		Texture2D texture = PropertiesSingleton.instance.canvasWorkspaceController.canvas.getResultTexture();
		string text= PropertiesSingleton.instance.activeSheet.nameKey.Localized();
		if (activeStrategy!=null)
			activeStrategy.onPictureSave(texture, text);
		else
			Debug.LogWarning("empty save strategy");

		DestroyImmediate(texture);
		Resources.UnloadUnusedAssets();
		System.GC.Collect();
	}

	void onSheetChangeListener (SheetObject obj)
	{
		StartCoroutine( changeSheetObjectThenReady(obj));
	}

	IEnumerator changeSheetObjectThenReady (SheetObject obj)
	{
		while (!ready || activeStrategy ==null || !activeStrategy.isReady())
			yield return new WaitForSeconds(0.1f);
		activeStrategy.onNewPictureOpen(obj);
	}
	
		
	void onWebContextReadyListener(object obj,Callback cb){
		Debug2.Log("im here from callback");
		string pageName=(string)obj;
		initData="";
		if (pageName.StartsWith("mru")){
			webType = WebType.MAIL_RU;
			initData = mailRuKey;
		}
		else if (pageName.StartsWith("vk")){
			webType = WebType.VK;
			initData = vkKey;
		}
		else if (pageName.StartsWith("fb"))
			webType = WebType.FB;
		else if (pageName.StartsWith("online"))
			webType = WebType.ONLINE;
		else  
			Debug2.LogError("unknown page");
		ready = true;
		activeStrategy= supportedWeb[webType];
		activeStrategy.onStart(initData);
	}


	public void hideApplication(){
		Application.ExternalEval("document.getElementById('unityPlayer').style.visibility = 'hidden';");
	}

	public void showApplication(){
		Application.ExternalEval("document.getElementById('unityPlayer').style.visibility = 'visible';");
	}

	public void setWebType(){
		Callback c = CallbackPool.instance.getCallback(CallbackType.DISPOSABLE);
		c.action = onWebContextReady;
#if UNITY_WEBPLAYER
		string eval= @"
			var pathname = window.location.pathname;
			var page=pathname.substring(pathname.lastIndexOf('/')+1);
			callback(CALLBACK_ID,page);
		".Replace("CALLBACK_ID",""+c.id);		

		Application.ExternalEval(eval);		
#endif
	}
	
}
