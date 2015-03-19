using UnityEngine;
using System.Collections;

[System.Serializable]
public class Cursors{
	public Texture2D             inkPointerTexture                                           ;
	public Texture2D             inkMaskTexture                                           ;
	public Texture2D             brushPointerTexture                                         ;
	public Texture2D             brushMaskTexture                                         ;
	public Texture2D             largeBrushPointerTexture                                    ;
	public Texture2D             largeBrushMaskTexture                                    ;
	public Texture2D             rollerPointerTexture                                        ;
	public Texture2D             rollerMaskTexture                                        ;
	public Material              cursorStampMaterial                                         ;
	public Material              cursorMaskMaterial                                          ;
}

[System.Serializable]
public class ScreenProps{
	public int minWidth     = 800;
	public int minHeight    = 600;
	public int normalWidth  = 1024;
	public int normalHeight = 768;
	public int offsetLeft   = 22;
	public int offsetTop    = 30;
	public int offsetDown   = 94;
	public int offsetRight  = 142;

	public static float _normalAspect=-1;

	public float normalAspect {
		get {
			if (_normalAspect == -1)
				_normalAspect = normalWidth / normalHeight;
			return _normalAspect;
		}
	}
}

[System.Serializable]
public class ColorsProps{
	public Color32 activeColor;
	public Color32 secondColor;
	public bool randomEnabled=false;
	public int totalHistoryNumber = 7;
	public Color32[] colorHistory;
	public Color32[] predefinedColors;
	public int predefinedColorsNumber = 12;
}

[System.Serializable]
public class SoundProps{
	public int musicCycleChanceInPercent = 75;
	public bool mute = false;
	public AudioClip[] music;
	public AudioClip currentMusic;


	public float musicLevel = 0.7f;
	public float sfxLevel   = 0.7f;

	public bool musicMute;
	public bool sfxMute;

	public float shiftMusicTimeInSecons = 8;
	public bool doWeeNeedToPlayMusic{
		get{
			return  !mute && !musicMute && musicLevel>0;
		}
	}

	public bool doWeeNeedToPlaySFX{
		get{
			return  !mute && !sfxMute && sfxLevel>0;
		}
	}
}

[System.Serializable]
public class ZoomScrollProps{
	public float left;
	public float right;
	public float value;
	public float size;
}

[System.Serializable]
public class WorkspaceCameraProperties{
	public int worldLeft    = -512;
	public int worldRight   = 512;
	public int worldTop     = 384;
	public int worldBottom  = -384;
}

[System.Serializable]
public class WorkspaceToolsConfig{
	public CanvasTool brush;
	public CanvasTool bigBrush;
	public CanvasTool Roller;
	public CanvasTool Ink;
}

[System.Serializable]
public class PropertiesSingleton : PersistentMonoSingleton<PropertiesSingleton> {
	private static string TOOL_CAMERA_NAME      ="ToolCamera"      ;
	
	public GameState gameState;
	public WorkspaceToolsConfig              tools;


	public Color32                           screenShotInProgressColor = Color.white;
	public float                             screenShotTimeout = 0.75f;

	public SystemLanguage                    language;
	public Cursors 				 cursors     ;
	public GUIStampList                      guiStampList;
	public int                               activeStampId;
	public Texture2D                         activeStampTexture;

	public Albums                            albums      ;
	public WorkspaceCameraProperties         workspaceCamProps;
	public CanvasWorkSpaceController	canvasWorkspaceController;

	public int                               selectedAlbum  = 1;
	public Texture2D[,]                     albumsIcons ;
	
	public ScreenProps screen                             				     ;
	public ColorsProps colorProperties;
	public SoundProps  soundProperties;
	public int                   width                                                       ;
	public int                   height                                                      ;
	public int                   bufferCount                                                 ;
	public int                   bufferZStep                                                 ;
	public int                   bufferZOffset                                               ;	
	public SheetList             defultSheetList                                             ;	
	private SheetObject           _activeSheet                                               ;
	
	public EventType             lastGuiEventType;
	
	public bool undoEnabled = false;
	public bool redoEnabled = false;	
	public WinterGenerator       winterGenerator;
	public ScreenSizeController screenSizeController;
	public Camera guiCamera;
	public SheetObject activeSheet {
		get {
			return this._activeSheet;
		}
		
	}	
	public ToolType              activeTool                                                  ;
	public int                   maxFreeLayersInBoofer                                       ;
	public byte                  sensitivity              = 1                                ;
	public float                 floodFillingTime         = 1.0f                             ;
	public bool                  isWorkspaceFullScreen                                       ;
	public Rect                  fullCameraViewPortRect   = new Rect(0,0,1,1)                ;
	public Rect                  smalllCameraViewPortRect = new Rect(0.05f,0.2f,0.75f,0.75f) ;
	public float                 tmpMeshLineWidth         = 15.0f                            ;
	public float                 lineMeshZPosition        = 100                              ;
	public IntVector2            workpsaceCursorPixelPosition                                ;
	public Vector3               workspaceCursorGlobalPosition                               ;	
	/*
	public ConfirmReflashWindowController confirmReflash                                     ;
	*/
	public bool                  drawWithinRegion                                            ;


	public bool                  debugModeOn              = false                            ;
	public float                 timeoutUntilClick        = 1.5f                             ;
	public bool                  wrongActionShowFrame;
	public float                 wrongActionShowFrameTimeout = 0.3f;
	
	private Camera   _workSpaceCamera      ;
	public Material  lineRendererMaterial ;
	public Material  stampCursorMaterial;
	
	private Color32  _activeColor          ;
	private Camera   _toolCamera           ;
	
	public float		    cameraMaxSize = 384;
	public float 		    cameraMinSize  = 20;
	public float                camearZoomStep = 20;
	public float                moveHandSpeedRatio = 0.1f;
	
	public float                zoomButtonDeltaPerClick = 0.75f;
	public void setNewTool(ToolType tool){
		this.activeTool = tool;
		if (WorkspaceEventManager.instance.onToolChange!=null)
			WorkspaceEventManager.instance.onToolChange(tool);
	}	
		

	//TODO candidate to remove
	public Camera toolCamera {
		get {
			if (this._toolCamera==null){
				Camera c=getCameraByName (TOOL_CAMERA_NAME);
				this._toolCamera=c;				
			}			
			return this._toolCamera;
		}
		set {
			_toolCamera = value;
		}
	}

	private Camera getCameraByName (string cameraName)
	{
		foreach (Camera c in Camera.allCameras){
			if (c.name.Equals( cameraName)){
				return c;
			}
		}
		Debug.LogWarning("can't find "+ cameraName +" on scene");							
		return Camera.main;
	}	
	
	public static Rect getWorkspaceCameraViewPortRect(){
		return instance.isWorkspaceFullScreen ? instance.fullCameraViewPortRect : instance.smalllCameraViewPortRect;	
	}
	
	public static Vector3 getFirstLayerZPosition(){
		return new Vector3(0,0, (float)( instance.bufferZOffset));		
	}
	
	public static Vector3 getLayerZPosition(int index){
		return new Vector3(0,0, (float)( instance.bufferZOffset + instance.bufferZStep*index));		
	}
	
	public static Vector3 getMaskZPosition(){
		return new Vector3(0,0, (float)( instance.bufferZOffset - instance.bufferZStep));		
	}
	
	public static Vector3 getBackLayerZPosition(){
		return new Vector3(0,0, (float)( instance.bufferZOffset + instance.bufferZStep*instance.bufferCount+instance.bufferZOffset));		
	}
	
	public static Vector3 getFrontColliderZPosition(){
		return new Vector3(0,0, (float)( instance.bufferZOffset - (instance.bufferZStep * 2)));		
	}
	
	
	public static Material getLineRendererMaterial(){
		return instance.lineRendererMaterial;
	}
	
	public override void Init ()
	{
		initializeColors();
		InstantiateMaterials();
	}

	void InstantiateMaterials(){
		lineRendererMaterial = new Material(lineRendererMaterial);
		stampCursorMaterial = new Material(stampCursorMaterial);
		cursors.cursorMaskMaterial = new Material(cursors.cursorMaskMaterial);
		cursors.cursorStampMaterial = new Material(cursors.cursorStampMaterial);
	}
	
	void initializeColors(){
		colorProperties.activeColor = ColorUtil.getRandomColor();
		if (colorProperties.colorHistory == null || colorProperties.colorHistory.Length != colorProperties.totalHistoryNumber)
			colorProperties.colorHistory = new Color32[colorProperties.totalHistoryNumber];
		for (int i = 0; i < colorProperties.totalHistoryNumber; i++) {
			colorProperties.colorHistory[i]=ColorUtil.getRandomColor();
		}
		
		if (colorProperties.predefinedColors == null || colorProperties.predefinedColorsNumber != colorProperties.predefinedColors.Length){
			colorProperties.predefinedColors = new Color32[colorProperties.predefinedColorsNumber];
			for (int i = 0; i < colorProperties.predefinedColorsNumber; i++) {
				colorProperties.predefinedColors[i]=ColorUtil.getRandomColor();
			}
		}
		
	}


	public void setSheet(SheetObject sheetObject){
		if (_activeSheet!=null)
			_activeSheet.releaseTextures();
		_activeSheet = sheetObject;
		if (WorkspaceEventManager.instance.onSheetChange!=null)
			WorkspaceEventManager.instance.onSheetChange(sheetObject);		
	}
}
