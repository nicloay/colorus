using UnityEngine;
using System.Collections;

public class WindowPictureController : ControllerInterface {
	GameState previouseNewPictureState = GameState.IN_GAME;
	
	#region ControllerInterface implementation
	public void init ()
	{
		WorkspaceEventManager.instance.onMenuNewPictureClick      += onMenuNewPictureClickListener;
		WorkspaceEventManager.instance.onExitFromPicChooserWindow += onExitFromPicChooserWindowListener;
		WorkspaceEventManager.instance.onSelectAlbum              += onSelectAlbumListener;
		WorkspaceEventManager.instance.onSelectPicture            += onSelectPictureListener;
	}
	#endregion
	
	void onMenuNewPictureClickListener(){
		previouseNewPictureState = PropertiesSingleton.instance.gameState;
		PropertiesSingleton.instance.gameState = GameState.PICTURE_SELECT;
		loadPictureIcons();
	}
	
	
	void onExitFromPicChooserWindowListener(){
		unloadPictureIcons();
		PropertiesSingleton.instance.gameState = previouseNewPictureState;
	}
	
	void onSelectAlbumListener(int albumNumber){
		PropertiesSingleton.instance.selectedAlbum = albumNumber;
		loadPictureIcons();
	}
	
	void onSelectPictureListener(int pictureId){
		SheetObject  so ;
		if (pictureId==-1){
			so= PropertiesSingleton.instance.albums.album[0].sheetObject;
		} else {
			int albumNumber = PropertiesSingleton.instance.selectedAlbum;			
			so = PropertiesSingleton.instance.albums.album[albumNumber].sheetList.sheetList[pictureId];
		}
		sendEventNeedToOpenPicture(so);
		onExitFromPicChooserWindowListener();
	}
	
	void unloadPictureIcons(){
		Texture2D[,] texs = PropertiesSingleton.instance.albumsIcons;
		for (int i = 0; i < texs.GetLength(0); i++) {
			if (texs[i,0] != null){
				int j = 0;
				while (texs[i,j]!=null && j < texs.GetLength(1)){
					Resources.UnloadAsset(texs[i,j]);
					texs[i,j]=null;
					j++;
				}
			}
		}
	}
	
	void loadPictureIcons(){
		if (PropertiesSingleton.instance.albumsIcons==null)
			PropertiesSingleton.instance.albumsIcons = new Texture2D[10,100]; //TODO get actual values here
		SheetObject[] sheet= PropertiesSingleton.instance.albums.album[PropertiesSingleton.instance.selectedAlbum].sheetList.sheetList;
		if (PropertiesSingleton.instance.albumsIcons[PropertiesSingleton.instance.selectedAlbum,0]!=null)
			return; //pictures already loaded here
		for (int i = 0; i < sheet.Length; i++) {
			string iconPath =  sheet[i].persistentBorderLayerPath.Replace("border","icon");
			PropertiesSingleton.instance.albumsIcons[PropertiesSingleton.instance.selectedAlbum,i] = Resources.Load(iconPath,typeof(Texture2D)) as Texture2D;
		}
	}
	
	void sendEventNeedToOpenPicture(SheetObject so){
		if (WorkspaceEventManager.instance.onPicIconeClick!=null)
			WorkspaceEventManager.instance.onPicIconeClick(so);
	}
}
