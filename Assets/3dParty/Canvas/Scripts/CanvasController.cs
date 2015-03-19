using UnityEngine;
using System.Collections;
using System;


public delegate void OnCanvasCameraChangePosition(float zoomHRatio, float zoomVRatio, Vector2 position, float newSize);
public delegate void OnScrollDown(Vector3 globalPosition, IntVector2 pixelPosition, Vector2 screenPosition);
public delegate void OnMouseOverWithScrolll(Vector3 globalPosition, IntVector2 pixelPosition, Vector2 screenPosition);
public delegate void OnMouseOverWithScrollDone();

public class CanvasEvents{
	public Action<IntVector2> 		onMouseOverWithButtonDone;
	public Action<IntVector2, Vector3>      onMouseOverWithButton;
	public Action<IntVector2> 		onMouseLeftButtonDown;

	public Action             		onMouseOverWithScrollDone;
	public OnMouseOverWithScrolll 		onMouseOverWithScroll;
	public OnScrollDown	 		onMouseScrollDown;

	public Action 		  		onMouseEnter;
	public Action             		onMouseExit;
	public Action<IntVector2, Vector3> 	onWorkspaceMouseOver;
	public Action<float, IntVector2, Vector3> onVerticalMouseScroll;
	public OnCanvasCameraChangePosition	onCameraChangePosition ;

	public Action<IntVector2, Vector3>      onTouchStart;
	public Action<IntVector2, Vector3>      onTouchOver;
	public Action				onTouchEnd;

	public Action<Touch, Touch>		onDblTouchBegin;
	public Action<Touch, Touch>		onDblTouch;

	public Action                           onActiveReceiveNewColors;
}

public class CanvasController : MonoBehaviour {

	public static CanvasEvents events = new CanvasEvents();
	public CanvasConfig config;

	public CanvasLayer backLayer;
	public CanvasLayer frontLayer;
	public CanvasRadialLayer radialLayer;
	public CanvasCollider canvasCollider;
	public CanvasBuffer canvasBuffer;
	public Vector2 size;
	public Vector2 extents;
	public CanvasCamera canvasCamera;

	public Color32[] actualColors{
		get{
			return _actualColors;
		}
	}

	public bool[] persistentLayer{
		get{
			return _persistentLayer;
		}
	}

	Color32[] _actualColors;
	bool[] _persistentLayer;
	bool initialized = false;
	public void initialize(CanvasConfig config, CanvasCameraConfig canvasCameraConfig, Texture2D texture=null){
		canvasBuffer = new CanvasBuffer(config.canvasSize, config.bufferSize);
		this.config = config;
		canvasCamera = new CanvasCamera(canvasCameraConfig, config.canvasSize, gameObject);
		backLayer = new CanvasLayer(config.canvasSize, 100, config.layersShader, canvasCamera);
		frontLayer = new CanvasLayer(config.canvasSize, 20, config.layersShader, canvasCamera);
		radialLayer = new CanvasRadialLayer(config.canvasSize, 25, config.radialFillShader, config.radialFillTexture, canvasCamera);
		canvasCollider = CanvasCollider.createCanvasCollider (config.canvasSize, 10, gameObject, canvasCamera);

		setNewPicture(texture);
		initialized = true;

		handleInnerEvents();
	}

	public void updateCameraConfig(CanvasCameraConfig canvasCameraConfig){
		canvasCamera.updateCameraConfig(canvasCameraConfig);
	}

	public void setNewPictureBlank(){
		setNewPicture(null);
	}


	public Texture2D getResultTexture(){
		Color32[] resultColors;
		if (frontLayerNull)
			resultColors = _actualColors;
		else 
			resultColors = TextureUtil.mergetTextureAbovePixelArray(frontLayer.getTexture(),_actualColors);
		Texture2D result = new Texture2D((int)config.canvasSize.x,
		                                 (int)config.canvasSize.y,
		                                 TextureFormat.ARGB32,
		                                 false);
		result.SetPixels32(resultColors);
		result.Apply();
		resultColors = null;
		return result;
	}

	bool frontLayerNull = false;

	public void setNewPicture(Texture2D texture, Texture2D persistentBorder=null){
		int totalPixelSize = config.canvasSize.x * config.canvasSize.y;
		if (_actualColors == null || _actualColors.Length != (totalPixelSize))
			_actualColors = new Color32[totalPixelSize];
		size = new Vector2(config.canvasSize.x, config.canvasSize.y);
		extents.x = size.x / 2;
		extents.y = size.y / 2;
		ColorUtil.setWhitePixels(_actualColors);

		Texture2D backTexture = backLayer.setBlank(_actualColors);
		if (texture !=null)
			frontLayer.setTexture(texture);
		frontLayerNull =( texture==null);

		if (persistentBorder != null){
			Color32[] colors  = persistentBorder.GetPixels32();
			if (_persistentLayer == null 
			    || _persistentLayer.Length != totalPixelSize )
				_persistentLayer = new bool[totalPixelSize];

			for (int i = 0; i < _persistentLayer.Length; i++) {
				_persistentLayer[i] = colors[i].a > 253;
			}
		} else {
			if (_persistentLayer == null 
			    || _persistentLayer.Length != totalPixelSize )
				_persistentLayer = new bool[totalPixelSize];

			for (int i = 0; i < totalPixelSize; i++) {
				_persistentLayer[i] = false;
			}
		}
		canvasBuffer.resetUndoRedo();
	}

	void Update(){
		if (!initialized)
			return;	
		backLayer.render();
		if (!frontLayerNull)
			frontLayer.render();
		radialLayer.render();
	}

	public void zoom(float amount){
		canvasCamera.zoom(amount);
	}

	public void zoom(float amount, IntVector2 pixelPosition, Vector3 globalPosition){
		canvasCamera.zoom(amount,pixelPosition, globalPosition);
	}

	public Color32[] fetchColors(){
		return canvasBuffer.getArray();
	}

	public void undo(){
		Color32[] undo = canvasBuffer.getForUndo();
		if (undo !=null)
			applyColors(undo, false);
	}

	public void redo(){
		Color32[] redo = canvasBuffer.getForRedo();
		if (redo!=null)
			applyColors(redo, false);
	}

	//TODO - curently working with no alpha, if you need it, need to make delegates
	byte _r,_g,_b;
	public void applyColors(Color32[] colors, bool sendEvents = true, bool radialAnimate = false, IntVector2 center = null){
		for (int i = 0; i < colors.Length; i++) {
			if (colors[i].a !=0){
				_r = _actualColors[i].r;
				_actualColors[i].r = colors[i].r;
				colors[i].r = _r;
				
				_g = _actualColors[i].g;
				_actualColors[i].g = colors[i].g;
				colors[i].g = _g;
				
				_b = _actualColors[i].b;
				_actualColors[i].b = colors[i].b;
				colors[i].b = _b;
			}
		}

		if (radialAnimate){
			IntVector2 point;
			if (center == null){
				 point = new IntVector2( config.canvasSize.x/2, config.canvasSize.y/2);
			} else {
				point = center;
			}
			StartCoroutine(radialLayer.updateColorsAndAnimate(colors,point));
		}

		StartCoroutine( backLayer.updateColors(_actualColors, radialAnimate));
		if (sendEvents && events.onActiveReceiveNewColors != null)
			events.onActiveReceiveNewColors();

	}


	public void clearActiveColors(bool radialAnimation = false){
		Color32[] colors = fetchColors();
		for (int i = 0; i < colors.Length; i++) {
			colors[i].r = 255;
			colors[i].g = 255;
			colors[i].b = 255;
			colors[i].a = 255;
		}
		if (radialAnimation)
			applyColors(colors,false,true);
		else
			applyColors(colors, false);
	}


	void handleInnerEvents () {
#if UNITY_IOS
		handleTouchEvents();
#else
		handleMouseEvents();
#endif
	}


#if UNITY_IOS
	void handleTouchEvents(){
		events.onDblTouchBegin += onDblTouchBeginListener;
		events.onDblTouch      += onDblTouchListener;
	}

	Vector3 startWorldPoint1,startWorldPoint2;
	Vector2 startScreenPosition1, startScreenPosition2;
	float startCamSize;
	IntVector2 tmpIV2 = new IntVector2();
	void onDblTouchBeginListener (Touch t1, Touch t2) {
		canvasCamera.getCoordinates(t1.position, ref startWorldPoint1, ref tmpIV2);
		canvasCamera.getCoordinates(t2.position, ref startWorldPoint2, ref tmpIV2);
		startScreenPosition1 = t1.position;
		startScreenPosition2 = t2.position;
		startCamSize = canvasCamera.camera.orthographicSize;
	}

	void onDblTouchListener (Touch t1, Touch t2) {
		canvasCamera.syncTwoPointsToWorld(t1.position, startScreenPosition1, startWorldPoint1, 
		                                  t2.position, startScreenPosition2, startWorldPoint2, startCamSize);
	}
#else

	void handleMouseEvents(){
		events.onVerticalMouseScroll += onVerticalMouseScrollListener;
		
		events.onMouseScrollDown         += onMouseScrollDownListener;
		events.onMouseOverWithScroll     += onMouseOverWithScrollListener;
		events.onMouseOverWithScrollDone += onMouseOverWithScrollDone;
	}


	void onVerticalMouseScrollListener (float arg1, IntVector2 arg2, Vector3 arg3) {
		float amount = -1 * arg1 * config.scrollRatio;
		zoom(amount, arg2, arg3);
	}


	Vector3 startPoint;
	void onMouseScrollDownListener (Vector3 globalPosition, IntVector2 pixelPosition, Vector2 screenPosition) {
		startPoint = canvasCamera.screenPointToWorld(screenPosition);
	}

	void onMouseOverWithScrollListener (Vector3 globalPosition, IntVector2 pixelPosition, Vector2 screenPosition) {
		canvasCamera.syncScreenPointWithWorld(screenPosition, startPoint);
	}

	void onMouseOverWithScrollDone () {
	}
	
#endif
	
}

