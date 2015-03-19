using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

public class CallbackPoolInfo : MonoBehaviour {
	Rect windowRect = new Rect(Screen.width-320, Screen.height-220, 300, 200);	
	BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
	
	Dictionary<long,Callback>  callbackDict;
	Queue          <Callback>  callbackQueue;
	List           <Callback>  disposableCallbacks;
	List           <Callback>  permanentCallback;
	
	CallbackPool cp;
	
	void Start(){
		getPrivateFields();
	}
	
	void OnGUI(){
		windowRect = GUILayout.Window(0, windowRect, DoMyWindow, "CallbackPool info");
	}
	
  	void DoMyWindow(int windowID) {		
		GUILayout.Label("current pool size="  + cp.currentPoolSize + "  dict size="+ (callbackDict!=null ? ""+callbackDict.Count : "null"));
		
		GUILayout.Label("callbackQueue.size ="       + (callbackQueue!=null ? ""+callbackQueue.Count : "null"));
		GUILayout.Label("disposableCallbacks.size =" + (disposableCallbacks!=null ? ""+disposableCallbacks.Count : "null"));
		GUILayout.Label("permanentCallback.size ="   + (permanentCallback!=null ? ""+ permanentCallback.Count : "null"));
		GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
    }
	
	
	void getPrivateFields(){
		cp = CallbackPool.instance;
		callbackQueue       = typeof(CallbackPool).GetField("callbackQueue"      , flags).GetValue(cp) as Queue          <Callback>;
		disposableCallbacks = typeof(CallbackPool).GetField("disposableCallbacks", flags).GetValue(cp) as List           <Callback>;
		permanentCallback   = typeof(CallbackPool).GetField("permanentCallback"  , flags).GetValue(cp) as List           <Callback>;
		callbackDict        = typeof(CallbackPool).GetField("callbackDict"       , flags).GetValue(cp) as Dictionary<long,Callback>;
	}
}
