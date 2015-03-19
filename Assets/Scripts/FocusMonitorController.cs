using UnityEngine;
using System.Collections;

public class FocusMonitorController : MonoBehaviour {

	void OnApplicationFocus(bool status){
#if UNITY_WEBPLAYER
		Application.ExternalCall("onApplictaionFocus");
#endif		
	}

	void OnApplicationPause(bool status){
#if UNITY_WEBPLAYER
		Application.ExternalCall("onApplictaionFocus");
#endif		
	}
}
