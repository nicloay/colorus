using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TakeScreenshot : MonoBehaviour {
	Rect windowRect = new Rect(280, 20, 240, 60);	
	
	void OnGUI() {
		windowRect = GUILayout.Window(40, windowRect, DoMyWindow, "ScreenShot");
	}
	
   	void DoMyWindow(int windowID) {	
		if (GUILayout.Button("screen shot"))
			StartCoroutine( takeScreenShot());
		
		GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
    }
	
	IEnumerator takeScreenShot ()
	{
		yield return new WaitForEndOfFrame();
		Texture2D texture = getScreenTexture ();	
#if !UNITY_WEBPLAYER
		System.IO.File.WriteAllBytes("./123321test.png",texture.EncodeToPNG());		
#endif				
		MRUController.instance.uploadTexture(texture, delegate(object incomeObj, Callback callback){
			Dictionary<string,object> paramObj = new Dictionary<string, object>(){
				{"url",(string)incomeObj},
				{"aid",2},
				{"set_as_cover",false},
				{"name","testupload picture"},
				{"tags","unity3d, js, png"},
				{"theme","6"}				
			};
			
			MRUController.instance.callMailruByObjectMailruListenerAndCallback(
				"mailru.common.photos.upload",paramObj,
				"mailru.common.events.upload", delegate(object result, Callback mruCallback){
					Dictionary<string,object> resultObj=result as Dictionary<string,object>;					
					string status = (string)resultObj["status"];
					if (status.Equals("uploadSuccess")){
						Debug2.LogDebug("status ok");
						string imgFullPath=(string)(resultObj["originalProps"] as Dictionary<string,object>)["url"];
						MRUController.instance.removeTextureFromServer(imgFullPath);
						CallbackPool.instance.releasePermanentCallback(mruCallback);	
					} else if (status.Equals("closed")){
						Debug2.LogDebug("user closed window");
						string imgFullPath=(string)(resultObj["originalProps"] as Dictionary<string,object>)["url"];
						MRUController.instance.removeTextureFromServer(imgFullPath);
						CallbackPool.instance.releasePermanentCallback(mruCallback);
					} else if (!status.Equals("opened")){
						Debug2.LogError("unkonwn status +"+status);
					}
					Debug2.LogDebug("current status ="+status);
					
				}
			);
			
		});
		
	}

	Texture2D getScreenTexture ()
	{
		Texture2D texture = new Texture2D (Screen.width, Screen.height, TextureFormat.RGB24, false);
		texture.ReadPixels (new Rect (0, 0, Screen.width, Screen.height), 0, 0);
		texture.Apply ();
		return texture;
	}
}
