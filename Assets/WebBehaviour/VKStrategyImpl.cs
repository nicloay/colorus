#if !UNITY_IPHONE
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using localisation;

public class VKStrategyImpl : WebStrategyInt {
	bool ready = false;
	VKController vkc;
	string albumName="Мои раскраски в Colorus";
	string albumDescription="Мои картинки из Colorus\n раскраски http://vk.com/appcolorus ";
	string wallMessage="Моя новая картинка \"PICTURE_NAME\" \n раскраска Colorus http://vk.com/appcolorus ";
	public void onStart (string key)
	{
		vkc= VKController.instance;		
		vkc.initializeVKApi(key);
		vkc.onApiReady+=delegate(bool status){
			if (status){
				ready = true;
			} else {
				Debug2.LogError("can't initialize api");
			}
		};
	}
	
	public bool isReady ()
	{
		return ready;
	}


	public void onNewPictureOpen (SheetObject sheetObject)
	{
		string text= sheetObject.nameKey.Localized() + " | Colorus";
		vkc.callMethod("setTitle",text);
	}

	public void onPictureSave (Texture2D texture, string pictureName)
	{
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
			string wallText=wallMessage.Replace("PICTURE_NAME",pictureName);
			if (albumId==-1)
				createAlbumAndPostScreenShot(texture,wallText);
			else
				postScreenShot (texture, wallText, albumId);
		});
	}
	
	
	void createAlbumAndPostScreenShot(Texture2D tex, string wallText){
		Dictionary<string,object> parameters= new Dictionary<string, object>();
		parameters["title"]=albumName;
		parameters["description"]=albumDescription;
		vkc.api("photos.createAlbum",parameters,delegate(object arg1, Callback arg2) {
			Debug2.LogDebug("im at  createAlbumAndPostScreenShot=====\n"+Json.Serialize(arg1));	
			Dictionary<string,object> resultDict=arg1 as Dictionary<string,object>;
			if (!resultDict.ContainsKey("response")){
				Debug2.LogError("vk api error \n"+Json.Serialize(arg1));
				return;
			}
			Dictionary<string,object> response =  resultDict["response"] as Dictionary<string,object>;
			long aid=(long)response["id"];
			postScreenShot(tex, wallText, aid);
		});
	}
	
	void postScreenShot (Texture2D tex, string wallText, long albumId){
			
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
			
			vkc.uploadTexture(tex,upload_url,"colorus",			
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

										WebContext.instance.hideApplication();
										Dictionary<string,object> wallProps=new Dictionary<string, object>();
										wallProps["owner_id"]=vkc.inputData["viewer_id"];
										wallProps["message"]=wallText;
										wallProps["attachments"]="photo"+vkc.inputData["viewer_id"]+"_" + photoId;
										vkc.api("wall.post",wallProps,delegate(object wallPostObj, Callback wallPostCallback) {
											if (!(wallPostObj as Dictionary<string,object>).ContainsKey("response"))
												Debug2.LogError("problem with wall.post upload "+ Json.Serialize(wallPostObj));
											WebContext.instance.showApplication();
										});
									}									
								});
						}
					});
				
				},
				delegate(string postError){
					Debug2.LogError("problem with post photo\n"+postError);
				}
			);
		});		
	}
}
#endif
