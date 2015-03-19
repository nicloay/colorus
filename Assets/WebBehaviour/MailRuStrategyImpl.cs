using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using localisation;

public class MailRuStrategyImpl : WebStrategyInt {
	bool ready = false;
	string albumName="Мои раскраски в Colorus";
	
	#region WebStrategyInt implementation
	public void onStart (string data)
	{
		MRUController.instance.privateKey = data;
		MRUController.instance.initializeMailRuApi();
		MRUController.instance.onApiReady+=delegate(object obj){
			ready = true;			
		};
	}

	public void onNewPictureOpen(SheetObject sheetObject){
		string text= "Colorus | " + sheetObject.nameKey.Localized();		
		MRUController.instance.callMailruByParams("mailru.app.utils.setTitle",text);				
	}

	public void onPictureSave (Texture2D texture, string pictureName)
	{
		MRUController.instance.callMailruByCallback("mailru.common.photos.getAlbums",delegate(object arg1, Callback arg2) {
			List<object> albumList=arg1 as List<object>;
			string albumId="";
			for (int i = 0; i < albumList.Count; i++) {
				Dictionary<string,object> album = albumList[i] as Dictionary<string,object>;
				string title=(string)album["title"];
				if (title.Equals(albumName)){
					albumId = (string)album["aid"];
					break;
				}
			}
			if (!String.IsNullOrEmpty( albumId)){
				savePicture(albumId, pictureName, texture);
			} else {
				createAlbumAndSavePictureOnSucess(pictureName, texture);
			}
				
			
		});	
	}
	

	public bool isReady ()
	{
		return ready;
	}
	#endregion

	void createAlbumAndSavePictureOnSucess (string pictureName, Texture2D texture)
	{
		Dictionary<string,object> paramsObject=new Dictionary<string,object>();
		paramsObject.Add("name",albumName);
		
		MRUController.instance.callMailruByObjectMailruListenerAndCallback(
				"mailru.common.photos.createAlbum",
				paramsObject,
				"mailru.common.events.createAlbum",
				delegate(object obj, Callback callback){
						Dictionary<string,object> result = obj as Dictionary<string,object>;
						string status = (string)result["status"];
						if (status.Equals("closed")){
							Debug2.LogDebug("album window has closed without creation");
							CallbackPool.instance.releasePermanentCallback(callback);
						} else if (status.Equals("createSuccess")){
							string aid=(string)result["aid"];
							Debug2.LogDebug("album created id = "+aid);
							CallbackPool.instance.releasePermanentCallback(callback);
							savePicture(aid, pictureName, texture);
						}
			
						
				});
	}
	
	void savePicture(string aid, string pictureName, Texture2D texture){
		MRUController.instance.uploadTexture(texture, delegate(object incomeObj, Callback callback){
			Dictionary<string,object> paramObj = new Dictionary<string, object>(){
				{"url",(string)incomeObj},
				{"aid",aid},
				{"set_as_cover",false},
				{"name",pictureName},
				{"tags","colorus, раскраска"},
				{"theme","14"}				
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
	
}
