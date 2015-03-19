using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class MainMenuConfig{
	public int mainOffset=11;
	public int rightColumnWidth=86;
	public int drawAreaMarginX = 12;
	public int drawAreaMarginY = 12;
	
	public int bottomButtonsHeight=56;
	public int minWidth = 1024;
	public int minHeight = 768;
	public int additionalPanelTop = 316;
	public int toolPanelTop = 157; 
	public int soundButtonPosition = 6;
	public int actualWidth;
	public int actualHeight;
	public Color32 disabledGUITintColor = Color.gray;
	public GUISkin skin;
}

public class MainOnGUI : MonoBehaviour {
	public MainMenuConfig config;	
	public OnWrongActionGUI           onWrongActionGUI;
	public BottomMenu                 bottomMenu;
	public ColorHistory               colorHistory;
	public ActiveColor                activeColor;
	public PredefinedColorsAndPalette predefinedColors;
	public AdditionalRightPannel      additionalRightPannel;
	public ToolPanel                  toolPanel;
	public ActiveTool                 activeTool;
	public WindowColorPicker          colorPickerWindow;
	public WindowStampSelect          stampSelectWindow;
	public WindowPictureSelect        pictureSelectWindow;
	public WindowInfo                 infoWindow;
	public WindowSound                soundWindow;
	public ScrollBars                 scrollBars;
	//public HelpView                   helpView;


	public Matrix4x4 guiMatrix;	
	public bool useMatrix=false;

	PropertiesSingleton props;
	Color32 oldColor;

	GameState previousState;

	void OnGUI () {
		if (props.gameState == GameState.SAVE_PIC)
			return;

		useGUILayout = false;
		if (config.skin!=null)
			GUI.skin = config.skin;
		
		if(useMatrix)
			GUI.matrix = guiMatrix;
		if (PropertiesSingleton.instance.gameState != GameState.IN_GAME){
			oldColor = GUI.color;
			GUI.color = config.disabledGUITintColor;
			GUI.enabled = false;
		}
		bottomMenu               .OnGUI();
		colorHistory             .OnGUI();
		activeColor              .OnGUI();
		predefinedColors         .OnGUI();		
		additionalRightPannel    .OnGUI();
		toolPanel                .OnGUI();
		activeTool               .OnGUI();
		scrollBars               .OnGUI();
		if (PropertiesSingleton.instance.gameState != GameState.IN_GAME){
			GUI.color = oldColor;
			GUI.enabled = true;
		}
		switch (PropertiesSingleton.instance.gameState){
		case GameState.COLOR_PICKER_ACTIVE:
			if (previousState != GameState.COLOR_PICKER_ACTIVE)
				colorPickerWindow.setActiveColor();

			colorPickerWindow.OnGUI();
			break;
		case GameState.STAMP_SELECT:
			stampSelectWindow.OnGUI();
			break;
		case GameState.PICTURE_SELECT:

			pictureSelectWindow.OnGUI();
			break;
			/*
		case GameState.SHOW_INFO:
			helpView.OnGUI();
			*/
			//infoWindow.OnGUI();
			break;
		case GameState.SOUND_SETTINGS:
			soundWindow.OnGUI();
			break;
		}

		onWrongActionGUI.OnGUI();
		if (Event.current.type != EventType.layout)
			PropertiesSingleton.instance.lastGuiEventType = Event.current.type;
		//debug part (then adjust gui size)
		//if (Time.frameCount %50 ==0)
		//	recalculatePositions();

		previousState = props.gameState;
	}
	
	void Awake() {		
		recalculatePositions();
		props = PropertiesSingleton.instance;
	}
	
	void recalculatePositions(){	
		if (config.skin==null){
			Debug.LogError("skin is null");
		}	
		float scale = 1; 
		if (Screen.width < config.minWidth ||
	       	    Screen.height < config.minHeight){	

			float screenAspect = (float)Screen.width / (float)Screen.height;
			float normalAspect = (float)config.minWidth / (float)config.minHeight;

			if (screenAspect > normalAspect){
				scale = (float)Screen.height / config.minHeight;			 
			} else {
				scale = (float)Screen.width / config.minWidth;
			}

			guiMatrix = Matrix4x4.TRS(Vector3.zero,
						   Quaternion.identity,
						   new Vector3( scale,
							       scale,
								1.0f));
			useMatrix = true;	
		} else {
			useMatrix = false;
		}
		
		float screenHeght = Screen.height / scale;
		float screenWidth = Screen.width / scale;
		config.actualWidth =(int) screenWidth;
		config.actualHeight =(int) screenHeght;

		int downMenuYposition = (int)(screenHeght - config.mainOffset - config.bottomButtonsHeight);
		
		bottomMenu           .setTopLeftOffset((int)config.mainOffset, downMenuYposition, config.skin);
		
		int colorHistoryRightPoint =(int)( screenWidth - config.rightColumnWidth-config.mainOffset - config.drawAreaMarginX);		
		colorHistory         .setTopRightPosition(colorHistoryRightPoint,downMenuYposition, config.skin);
		
		int rightColumnLeft = (int)(screenWidth - config.rightColumnWidth - config.mainOffset);
		activeColor          .setTopLeftPosition(rightColumnLeft, downMenuYposition, config.skin);
		
		int drawAreaDownBound = (int)(downMenuYposition - config.drawAreaMarginY);
		predefinedColors     .setDownLeftPosition(rightColumnLeft, drawAreaDownBound, config.skin);		
		colorPickerWindow    .setRightDownPosition(rightColumnLeft - config.drawAreaMarginY, downMenuYposition - config.drawAreaMarginY, config.skin);		
		
		additionalRightPannel.setTopLeftPosition(rightColumnLeft, config.additionalPanelTop, config.skin);
		toolPanel            .setTopLeftPosition(rightColumnLeft, config.toolPanelTop, config.skin);
		activeTool           .setTopLeftPosition(rightColumnLeft, config.drawAreaMarginY,config.skin);
		
		int screenCenterX = (int)screenWidth / 2;
		int screenCenterY = (int)screenHeght / 2;
		stampSelectWindow.setCenterPosition(screenCenterX, screenCenterY, config.skin);
		pictureSelectWindow.setCenterPosition(screenCenterX, screenCenterY, config.skin);
		infoWindow.setCenterPosition(screenCenterX, screenCenterY, config.skin);
		
		int contentWidth = (int)( screenWidth - config.rightColumnWidth - config.mainOffset*2 - config.drawAreaMarginX);
		int contentHeight = (int)(screenHeght - config.bottomButtonsHeight - config.mainOffset*2 - config.drawAreaMarginY);
		
		scrollBars.setProperties(config.mainOffset, config.mainOffset, contentWidth,contentHeight,config.mainOffset,config.skin);		

		int soundButtonLeft = (config.soundButtonPosition - 1) * (config.bottomButtonsHeight + config.drawAreaMarginX) + config.mainOffset;
		int soundButtonTop = downMenuYposition;
		soundWindow.setSoundButtonTopLeftPosition(soundButtonLeft,soundButtonTop, config.skin);
		onWrongActionGUI.setScreenSize(config);

		//helpView.recalculatePosition(config.skin);
	}
	
	
	
	int lastWidth;
	int lastHeight;
	bool lastFullScreen;
	void Update(){		
		if (Screen.width != lastWidth || Screen.height != lastHeight || Screen.fullScreen != lastFullScreen) {						
			lastWidth = Screen.width;
			lastHeight = Screen.height;
			lastFullScreen = Screen.fullScreen;
			recalculatePositions();
		}
	}
		
}
