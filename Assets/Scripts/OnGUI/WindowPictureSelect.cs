using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using localisation;

[Serializable]
public class PictureSelectWindowConfig{
	public int centerX;
	public int centerY;
	
	public int      iconButtonWidth = 78;
	public int      iconButtonHeight = 59;
	public int      iconButtonMargin = 10;
	public string   iconButtonStyleName = "pictureButton";
	public GUIStyle iconButtonStyle;
			
	
	
	public int      picButtonWidth = 200;
	public int      picButtonHeight = 150;
	public int      picButtonMargin = 10;
	public string   picButtonStyleName = "pictureButton";
	public GUIStyle picButtonStyle;
	public int      picColumnNumber = 3;
	public int      picRowNumber    = 3;

	public int      scrollAreaPadding = 3;
	
	public Albums albums;
	public string windowCaption = "Select Picture";
}

[Serializable]
public class WindowPictureSelect : GUIPart {
	public PictureSelectWindowConfig config;
	public GuiModalWindow            window;
	GUISkin currentSkin;
	Rect windowRect = new Rect(0,0,600,400);
	Rect[] iconsRect;
	Texture2D[] iconTextures;
	Rect[] picRect;	

	Rect scrollAreaRect;
	Rect[] viewRect;
	Vector2 scrollPosition;

	ScrollviewAutocloser autocloser;

	public void setCenterPosition(int x, int y, GUISkin skin){
		currentSkin = skin;
		config.centerX = x;
		config.centerY = y;
		setStyle(skin,config.picButtonStyleName,out config.picButtonStyle);
		setStyle(skin,config.iconButtonStyleName,out config.iconButtonStyle);
		if (PropertiesSingleton.instance.guiStampList == null)
			Debug.LogError("guiStampList in prop singleton is empty please fix it");
		config.albums = PropertiesSingleton.instance.albums;
		recalculatePositions();
		window.setProperties( windowRect, new GUIContent(config.windowCaption.Localized()), skin, doMyWindow, 
			WorkspaceEventManager.instance.onExitFromPicChooserWindow,
			WorkspaceEventManager.instance.onExitFromPicChooserWindow);
		autocloser = new ScrollviewAutocloser(config.scrollAreaPadding, config.picButtonHeight + config.picButtonMargin);
	}	



	Event e;
	public override void OnGUI ()
	{
		e = Event.current;	
		autocloser.preprocess(e, ref scrollPosition);
		window.OnGUI(new GUIContent(config.windowCaption.Localized()));	
		autocloser.postProcess(e, ref scrollPosition);
	}
	

	void doMyWindow(){
		if (GUI.Button(iconsRect[0],iconTextures[0], config.iconButtonStyle) && WorkspaceEventManager.instance.onSelectPicture != null)
			WorkspaceEventManager.instance.onSelectPicture(-1);//its empty image here
		for (int i = 1; i < iconsRect.Length; i++) {
			if (GUI.Button(iconsRect[i],iconTextures[i], config.iconButtonStyle) && WorkspaceEventManager.instance.onSelectAlbum!=null)
				WorkspaceEventManager.instance.onSelectAlbum(i);
		}

		scrollPosition = GUI.BeginScrollView(scrollAreaRect,scrollPosition,viewRect[PropertiesSingleton.instance.selectedAlbum],false,true);			
			Texture2D[,] pictureIcons =  PropertiesSingleton.instance.albumsIcons;//todo fix links here
			int count = PropertiesSingleton.instance.albums.album[PropertiesSingleton.instance.selectedAlbum].sheetList.sheetList.Length;
			Texture2D tex;
			for (int i = 0; i < count; i++){
				tex = pictureIcons[PropertiesSingleton.instance.selectedAlbum,i];
				if (tex!=null){
					if (GUI.Button(picRect[i], tex, config.picButtonStyle) && WorkspaceEventManager.instance.onSelectPicture != null)
						WorkspaceEventManager.instance.onSelectPicture(i);
				
				} else {
					if (GUI.Button(picRect[i], "...", config.picButtonStyle) && WorkspaceEventManager.instance.onSelectPicture != null)
						WorkspaceEventManager.instance.onSelectPicture(i);				 
				}
			}			
		GUI.EndScrollView();
	}

	void recalculatePositions(){
		
		GUIStyle verticalScrollBar = currentSkin.FindStyle("verticalScrollbar");
		int scrollWidth =(int)( verticalScrollBar.fixedWidth + verticalScrollBar.margin.left + verticalScrollBar.margin.right);
		
		
		int areaHeight = config.picButtonHeight * config.picRowNumber
				   + config.picButtonMargin * (config.picRowNumber - 1)
				   + config.scrollAreaPadding *2;
		int areaWidth = config.picButtonWidth * config.picColumnNumber 
				   + config.picButtonMargin * (config.picColumnNumber -1)
				   + scrollWidth
				   + config.scrollAreaPadding *2;
		
		int areaTop = config.iconButtonHeight + config.iconButtonMargin;
		
		windowRect.width = areaWidth;
		windowRect.height = areaHeight + areaTop;
		windowRect.x = config.centerX - windowRect.width/2;
		windowRect.y = config.centerY - windowRect.height/2;
		
		scrollAreaRect = new Rect(0,areaTop,areaWidth,areaHeight);

		recalculateIconPositions();
		recalculatePictureButtonPositions();
	}
	
	void recalculatePictureButtonPositions(){
		int maxSize = 0;
		List<Album> albums = config.albums.album;
		viewRect = new Rect[albums.Count];
		for (int i = 1; i < albums.Count; i++){
			int size = albums[i].sheetList.sheetList.Length;
			if (size > maxSize)
				maxSize = size;
			viewRect[i]=new Rect(0, 0, 
					(config.picButtonWidth + config.picButtonMargin) * config.picColumnNumber 
			                     - config.picButtonMargin + config.scrollAreaPadding * 2 ,
					((config.picButtonHeight + config.picButtonMargin) * (Mathf.Ceil ((float)size / (float)config.picColumnNumber)) 
			 		     - config.picButtonMargin + config.scrollAreaPadding * 2));
		}
		
		int x = 0;
		int yPosition =0;// config.iconButtonHeight+config.iconButtonMargin+config.picButtonMargin;
		int stepY = config.picButtonHeight + config.picButtonMargin;
		int stepX = config.picButtonWidth  + config.picButtonMargin;

		maxSize = 100;
		picRect = new Rect[maxSize];
		for (int i = 0; i < maxSize; i++) {			
			picRect[i]=new Rect(config.scrollAreaPadding + x * stepX,
			                    config.scrollAreaPadding + yPosition, config.picButtonWidth, config.picButtonHeight);			
			x++;
			
			if (x==config.picColumnNumber){
				x = 0;
				yPosition+=stepY;
			}				
		}
	}
	
	
	void recalculateIconPositions(){
		iconsRect = new Rect[config.albums.album.Count];
		iconTextures = new Texture2D[config.albums.album.Count];
		int left = 0;
		int top = 0;
		int width = config.iconButtonWidth;
		int height = config.iconButtonHeight;	
		int xStep = width + config.iconButtonMargin;
		for (int i = 0; i < config.albums.album.Count; i++) {
			iconsRect[i] = new Rect(left,top,width,height);
			left+=xStep;
			iconTextures[i]= config.albums.album[i].icon;			
		}
	}
}
