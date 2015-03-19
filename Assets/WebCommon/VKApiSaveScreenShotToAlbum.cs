using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class VKApiSaveScreenShotToAlbum : MonoBehaviour {
	public string vkKey;
	public string albumName = "test images";
	string status="not ready";
	VKController vkc;
	void Start () {
		vkc = VKController.instance;		
		vkc.onApiReady+=delegate(bool initStatus){			
			status=initStatus ? "Ready" : "Problem with initialization";
		};
		vkc.initializeVKApi(vkKey);		
	}
	
	void OnGUI(){
		GUILayout.Label("status: "+status);
		GUILayout.TextArea(albumName);
		if (GUILayout.Button("take screenshot"))
			takeScreenshot();
	}
	
	void takeScreenshot(){
		vkc.api("photos.getAlbums",new Dictionary<string,object>(),delegate(object arg1, Callback arg2) {
			Debug2.LogDebug("im at  takeScreenshot=====\n"+Json.Serialize(arg1));
			Dictionary<string,object> resultDict=arg1 as Dictionary<string,object>;
			if (!resultDict.ContainsKey("response")){
				Debug2.LogError("vk api error \n"+Json.Serialize(arg1));
				return;
			}
			Dictionary<string,object> response =  resultDict["response"] as Dictionary<string,object>;
			List<object> items = response["items"] as List<object>;
			long albumId=-1;
			for (int i = 0; i < items.Count; i++) {
				Dictionary<string,object> album = items[i] as Dictionary<string,object>;
				if (((string)album["title"]).Equals(albumName)){
					albumId =(long)album["id"];
					break;
				}				
			}
			if (albumId==-1)
				createAlbumAndPostScreenShot();
			else
				postScreenShot (albumId);
		});
	}
	
	void createAlbumAndPostScreenShot(){
		Dictionary<string,object> parameters= new Dictionary<string, object>();
		parameters["title"]=albumName;
		vkc.api("photos.createAlbum",parameters,delegate(object arg1, Callback arg2) {
			Debug2.LogDebug("im at  createAlbumAndPostScreenShot=====\n"+Json.Serialize(arg1));	
			Dictionary<string,object> resultDict=arg1 as Dictionary<string,object>;
			if (!resultDict.ContainsKey("response")){
				Debug2.LogError("vk api error \n"+Json.Serialize(arg1));
				return;
			}
			Dictionary<string,object> response =  resultDict["response"] as Dictionary<string,object>;
			long aid=(long)response["id"];
			postScreenShot(aid);
		});
	}
	
	void postScreenShot (long albumId)
	{
		StartCoroutine( getScreenShotFromScreen(delegate(Texture2D tex) {		
			Dictionary<string,object> getUploadServParams=new Dictionary<string,object>();
			getUploadServParams["album_id"]=(long)albumId;
			vkc.api("photos.getUploadServer",getUploadServParams,delegate(object arg1, Callback arg2){
				Debug2.LogDebug("im at  postScreenShot getWallUploadServer=====\n"+Json.Serialize(arg1));	
				Dictionary<string,object> resultDict=arg1 as Dictionary<string,object>;
				if (!resultDict.ContainsKey("response")){
					Debug2.LogError("vk api error \n"+Json.Serialize(arg1));
					return;
				}
				Dictionary<string,object> response =  resultDict["response"] as Dictionary<string,object>;
				string upload_url=(string)response["upload_url"];
				WWWForm form = new WWWForm();
	
				form.AddBinaryData("photo", tex.EncodeToPNG(), "PHOTO_NAME.png", "image/png");
		
				StartCoroutine(CallbackOnWWWResponse(upload_url, form, 
					delegate(string postResult){
						Debug2.LogDebug("post success \n"+postResult);
						Dictionary<string,object> postResultDict=Json.Deserialize(postResult) as Dictionary<string,object>;
					
						Dictionary<string,object> parameters=new Dictionary<string, object>();
						parameters["album_id"]=albumId;						
						parameters["server"]      = postResultDict["server"];
						parameters["photos_list"] = postResultDict["photos_list"];
						parameters["hash"]        = postResultDict["hash"];
						parameters["caption"]     = "test upload to vk server";
						
						vkc.api("photos.save",parameters,delegate(object psarg1, Callback psarg2) {
							Debug2.LogDebug("im at  postScreenShot getWallUploadServer=====\n"+Json.Serialize(psarg1));	
							Dictionary<string,object> psresultDict=psarg1 as Dictionary<string,object>;
							if (!psresultDict.ContainsKey("response")){
								Debug2.LogError("vk api error \n"+Json.Serialize(psarg1));
							} else {
								Debug2.LogDebug("photos post success \n"+Json.Serialize(psarg1));
								long photoId=(long)((psresultDict["response"] as List<object>)[0] as Dictionary<string,object>)["id"];
								
								vkc.windowConfirm("ваша картинка успешно сохранена\\n" +
									"в альбоме \""+albumName+"\"\\n" +
									"хотите добавить ее на стену?",delegate(object confirmObject, Callback confirmCallback) {
										bool result=(bool)confirmObject;
										Debug2.LogDebug("confirmation result = "+result);
										if (result){
											Dictionary<string,object> wallProps=new Dictionary<string, object>();
											wallProps["owner_id"]=vkc.inputData["viewer_id"];
											wallProps["message"]="тестовая запись на стену";
											wallProps["attachments"]="photo"+vkc.inputData["viewer_id"]+"_" + photoId;
											vkc.api("wall.post",wallProps,delegate(object wallPostObj, Callback wallPostCallback) {
												if (!(wallPostObj as Dictionary<string,object>).ContainsKey("response"))
													Debug2.LogError("problem with wall.post upload "+ Json.Serialize(wallPostObj));
											});
										}									
									});
							}
						});
					
					},
					delegate(string postError){
						Debug2.LogError("problem with post photo\n"+postError);
					}
				));
			});
		}));
	}
	
	IEnumerator getScreenShotFromScreen (Action<Texture2D> resultTexture)
	{
		yield return new WaitForEndOfFrame();
		Texture2D texture = new Texture2D (Screen.width, Screen.height, TextureFormat.RGB24, false);
		texture.ReadPixels (new Rect (0, 0, Screen.width, Screen.height), 0, 0);
		texture.Apply ();
		resultTexture(texture);
		yield return null;
	}	
	
	
	IEnumerator CallbackOnWWWResponse(string URL, WWWForm form, Action<string> callbackOnSuccess, Action<string> callbackOnError){
		WWW www = new WWW(URL, form);
		yield return www;

		if (www.error != null)
			callbackOnError(www.error);
		else			
			callbackOnSuccess(www.text);
		www = null;
	}
}
