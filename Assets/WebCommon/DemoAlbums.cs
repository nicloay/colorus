using UnityEngine;
using System.Collections;
using System.Collections.Generic;

struct AlbumMetadata{
	public string title;
	public string coverUrl;
	public Texture2D texture;
	public AlbumMetadata (string title, string coverUrl)
	{
		this.title = title;
		this.coverUrl = coverUrl;
		this.texture = new Texture2D(120,120);
	}

}

public class DemoAlbums : MonoBehaviour {
	Rect windowRect = new Rect(20, 140, 240, 380);	
	List<AlbumMetadata> albumsMetadata;
	string text="";
	Vector2 scrollPosition;
	
    void OnGUI() {
        windowRect = GUILayout.Window(20, windowRect, DoMyWindow, "Albums");
    }
	
   	void DoMyWindow(int windowID) {	
		text = GUILayout.TextArea(text);
		if (GUILayout.Button("create new"))
			createNewAblum();
		
		if (GUILayout.Button("get albums"))
			getAlbums();
		if (albumsMetadata!=null && albumsMetadata.Count>0){
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			foreach(AlbumMetadata albumMeta in albumsMetadata){
				GUILayout.BeginVertical();
					GUILayout.Label(albumMeta.title);					
					GUILayout.Box(albumMeta.texture, GUILayout.Height(120),GUILayout.Width(120));										
				GUILayout.EndVertical();				
			}
			GUILayout.EndScrollView();
		}		
		GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
    }
	
	void createNewAblum(){
		
		Dictionary<string,object> paramsObject=new Dictionary<string,object>();
		paramsObject.Add("name",text);
		
		MRUController.instance.callMailruByObjectMailruListenerAndCallback(
				"mailru.common.photos.createAlbum",
				paramsObject,
				"mailru.common.events.createAlbum",
				delegate(object obj, Callback callback){
						Dictionary<string,object> result = obj as Dictionary<string,object>;
						string status = (string)result["status"];
						if (status.Equals("opened")){
							Debug2.LogDebug("album window has opened");
						} else if (status.Equals("closed")){
							Debug2.LogDebug("album window has closed without creation. Need to unsubscribe from event");
							CallbackPool.instance.releasePermanentCallback(callback);
						} else if (status.Equals("createSuccess")){
							Debug2.LogDebug("album created id = "+(string)result["aid"]+" Need to unsubscribe from event");
							CallbackPool.instance.releasePermanentCallback(callback);
							if(albumsMetadata!=null)
								getAlbums();
						} else {
							Debug2.LogWarning("unknown status ["+status+"]");
						}
			
						
				});
	}
	
	void getAlbums(){
		MRUController.instance.callMailruByCallback("mailru.common.photos.getAlbums", delegate(object result, Callback callback) {			
			Debug2.LogDebug("getAlbums resul:\n" + Json.Serialize(result));
				
			List<object> albums = result as List<object>;
			albumsMetadata = new List<AlbumMetadata>();
			foreach (object item in albums) {
				var album=item as Dictionary<string,object>;
				albumsMetadata.Add(new AlbumMetadata((string)album["title"]    ,
													(string)album["cover_url"]));								
			}
			downloadPictures();
		});
	}

	void downloadPictures (){
		if ( albumsMetadata.Count>0){
			foreach (AlbumMetadata albumMeta in albumsMetadata){
				StartCoroutine(downloadTextureToMetadata(albumMeta));		
			}
		}
	}
	
	IEnumerator downloadTextureToMetadata(AlbumMetadata albumMeta){
		WWW www = new WWW(albumMeta.coverUrl);		
        yield return www;		
		www.LoadImageIntoTexture(albumMeta.texture);        
	}
	
}