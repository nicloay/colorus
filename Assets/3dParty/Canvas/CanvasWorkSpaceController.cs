using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CanvasWorkSpaceController : MonoBehaviour {
	public Shader layerShader;
	public int bufferSize;
	public Color canvasCamBGColor;
	public float canvasCamMinSize;
	public int canvasCamDepth = 20;
	public float scrollRatio = 10;
	public CanvasController canvas;
	CanvasConfig canvasConfig;
	CanvasCameraConfig canvasCameraConfig;
	PropertiesSingleton props;
	WorkspaceEventManager em;
	bool canvasInitialized = false;

	void Awake() {
		props = PropertiesSingleton.instance;
		em = WorkspaceEventManager.instance;
		canvas = GetComponent<CanvasController>();
		createCanvas();
		prebuildCanvasCamConfig();
		ScreenSizeController.onResolutionChange += onResolutionChangeListener;
		em.onSheetChange += onSheetChangeListener;
		em.onPicIconeClick += onPictureIconClickListener;

	}

	void createCanvas(){
		if (canvas.config == null)
			canvasConfig = new CanvasConfig();
		else 
			canvasConfig = canvas.config;
		canvasConfig.layersShader = layerShader;
		canvasConfig.canvasSize = new IntVector2(props.width, props.height);
		canvasConfig.bufferSize = bufferSize;
	}

	void prebuildCanvasCamConfig(){
		canvasCameraConfig = new CanvasCameraConfig();
		canvasCameraConfig.bgColor = canvasCamBGColor;
		canvasCameraConfig.camMinSize = canvasCamMinSize;
		canvasCameraConfig.cameraDepth = canvasCamDepth;
	}
			
	void onResolutionChangeListener (IntVector2 resolution, Vector2 scale) {
		canvasCameraConfig.screenPixelRect = new Rect (props.screen.offsetLeft * scale.x,
		                                               props.screen.offsetDown * scale.y,
		                                               (resolution.x - props.screen.offsetLeft - props.screen.offsetRight) * scale.x,
		                                               (resolution.y  - props.screen.offsetTop - props.screen.offsetDown) * scale.y);
		if ( canvasInitialized){
			canvas.updateCameraConfig(canvasCameraConfig);
		} else {
			canvas.initialize(canvasConfig, canvasCameraConfig);		
			canvasInitialized = true;
			doCanvasLogic();
		}
	}


	void onPictureIconClickListener (SheetObject sheetObject) {
		PropertiesSingleton.instance.setSheet( sheetObject);	
	}

	void onSheetChangeListener (SheetObject sheet) {
		if (canvasInitialized){
			canvas.setNewPicture(sheet.persistentFrontOutline, sheet.persistentBorderLayer);
			canvas.canvasCamera.zoom(10000);
		} else 
			StartCoroutine(waitInitializationAndSetPicture(sheet));
	}

	IEnumerator waitInitializationAndSetPicture(SheetObject sheet){
		while (!canvasInitialized)
			yield return null;
		canvas.setNewPicture(sheet.persistentFrontOutline, sheet.persistentBorderLayer);
	}

#region Canvas Events logic

	static Dictionary<ToolType,ToolLogicContext>      supportedLogic ;

	void doCanvasLogic(){
		initializeSupportedLogicDictionary();
		subscribeToEvents();
	}


	void initializeSupportedLogicDictionary(){
		supportedLogic = new Dictionary<ToolType, ToolLogicContext> (){
			{ToolType.BUCKET     , new ToolLogicContext (new ToolBucketStrategyImpl ())},
			{ToolType.INK     , new ToolLogicContext (new LineStrategyImpl (
					PropertiesSingleton.instance.tools.Ink,
					new CatmullRomStrategy()))},
			{ToolType.BRUSH      , new ToolLogicContext (new LineStrategyImpl (
					PropertiesSingleton.instance.tools.brush,					
					new CatmullRomStrategy()))},		
			{ToolType.BRUSH_LARGE, new ToolLogicContext (new LineStrategyImpl (
					PropertiesSingleton.instance.tools.bigBrush,					
					new CatmullRomStrategy()))},			
			{ToolType.ROLLER, new ToolLogicContext (new LineStrategyImpl (
					PropertiesSingleton.instance.tools.Roller,					
					new LinearInterpolationStrategy()))},
			{ToolType.STAMP      , new ToolLogicContext (new ToolStampStrategyImpl  ())},
			{ToolType.HAND       , new ToolLogicContext (new ToolHandStrategyImpl   ())},
			{ToolType.PIPETTE    , new ToolLogicContext (new ToolPipetteStrategyImpl())}
		};	
	}

	void subscribeToEvents ()
	{
#if !UNITY_IPHONE
		CanvasController.events.onMouseLeftButtonDown += onMouseLeftButtonDownListener;

		CanvasController.events.onMouseEnter                 	+= onMouseEnter;
		CanvasController.events.onWorkspaceMouseOver		+= onMouseOver;
		CanvasController.events.onMouseExit			+= onMouseExit;

		CanvasController.events.onMouseOverWithButton     += onMouseOverWithButton;
		CanvasController.events.onMouseOverWithButtonDone += onMouseOverWithButtonDone;
#else
		CanvasController.events.onTouchStart += onTouchStartListener;
		CanvasController.events.onTouchOver += onTouchOverListener;
		CanvasController.events.onTouchEnd += onTouchEndListener;
#endif
	}

	
#if !UNITY_IPHONE
	void onMouseLeftButtonDownListener (IntVector2 pixelPosition) {
		moseDownLogic (pixelPosition,PropertiesSingleton.instance.activeTool);
	}

	public void moseDownLogic (IntVector2 pixelPosition, ToolType toolType, bool starMouseMonitor=true)
	{
		supportedLogic [toolType].onMouseDown (pixelPosition);
	}

	void onMouseOverWithButtonDone (IntVector2 pixelPosition) {	

		supportedLogic [PropertiesSingleton.instance.activeTool].onMouseOverWithButtonDone (pixelPosition);

	}

	void onMouseOverWithButton (IntVector2 pixelPosition, Vector3 globalPosition) {
		
		supportedLogic [PropertiesSingleton.instance.activeTool].onMouseOverWithButton (pixelPosition, globalPosition);
	}
	
	void onMouseEnter () {
		supportedLogic [PropertiesSingleton.instance.activeTool].onMouseEnter ();
	}
	
	void onMouseExit () {
		supportedLogic [PropertiesSingleton.instance.activeTool].onMouseExit ();
	}
	
	void onMouseOver (IntVector2 pixelCursorPosition, Vector3 globalCursorPosition) {
		supportedLogic [PropertiesSingleton.instance.activeTool].onMouseOver (pixelCursorPosition, globalCursorPosition);
	}
#else


	void onTouchStartListener (IntVector2 pixelPosition, Vector3 globalPosition) {
		supportedLogic [PropertiesSingleton.instance.activeTool].onTouchStart(pixelPosition, globalPosition);
	}

	void onTouchOverListener (IntVector2 pixelPosition, Vector3 globalPosition) {
		supportedLogic [PropertiesSingleton.instance.activeTool].ontTouchOver(pixelPosition, globalPosition);
	}

	void onTouchEndListener () {
		supportedLogic [PropertiesSingleton.instance.activeTool].onTouchEnd();
	}
#endif
#endregion

}
