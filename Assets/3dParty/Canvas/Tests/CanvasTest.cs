using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CanvasTest : MonoBehaviour {
	public Texture2D texture;
	public RectOffset screenOffset;
	CanvasConfig canvasConfig;
	public CanvasTool canvasTool;
	public CanvasController canvas;
	public Texture2D brushPointer;



	void Awake(){
		canvas = gameObject.GetComponent<CanvasController>();
		Rect camRect = new Rect(screenOffset.left,
		                        screenOffset.bottom, Screen.width - screenOffset.horizontal, Screen.height - screenOffset.vertical);

		canvasConfig = canvas.config;



		if (texture == null)
			canvasConfig.canvasSize = new IntVector2(1024,768);
		else
			canvasConfig.canvasSize = new IntVector2(texture.width, texture.height);

		CanvasCameraConfig camConfig = new CanvasCameraConfig();
		camConfig.camMinSize = 10;
		camConfig.screenPixelRect = camRect;
		canvas.initialize(canvasConfig, camConfig, texture);

		testDrawing();
		oldWidth = Screen.width;
		oldHeight = Screen.height;
	}


	void Update(){
		testUndoRedo();
		handleChangeScreenSize();
	}

	int oldWidth = Screen.width;
	int oldHeight = Screen.height;
	void handleChangeScreenSize(){
		if (Screen.width != oldWidth || Screen.height !=oldHeight){
			Debug.Log("updating");
			canvas.canvasCamera.updateAspect( (float)Screen.width / (float)Screen.height);
		}
		oldWidth = Screen.width;
		oldHeight = Screen.height;

	}

	void OnGUI(){
		if (canvas!=null && canvas.canvasCamera != null)
			testScrollPosition();
	}

#region TEST_SCROLL_POSITION
	void testScrollPosition(){
		Vector2 camBottLeftPosition = canvas.canvasCamera.cameraGlobalBottomLeftCornerPosition;
		testHorizontalScroll(camBottLeftPosition);
		testVerticalScroll(camBottLeftPosition);
	}


	void testVerticalScroll(Vector2 camPosition){
		if (canvas.canvasCamera.cameraSize.y >= canvas.size.y)
			return;
		Rect vScrollRect = new Rect(40,80,10, Screen.height - 160);
		
		float newY = GUI.VerticalScrollbar(vScrollRect,
		                      camPosition.y,
		                      canvas.canvasCamera.cameraSize.y,
		                      canvas.extents.y,
		                      -canvas.extents.y);
		if (newY != camPosition.y){
			camPosition.y = newY;
			canvas.canvasCamera.cameraGlobalBottomLeftCornerPosition = camPosition;
		}
	}

	void testHorizontalScroll(Vector2 camPosition){
		if (canvas.canvasCamera.cameraSize.x >= canvas.size.x)
			return;
		Rect hScrollRect = new Rect(80,40,Screen.width - 160, 10);
		float newX = GUI.HorizontalScrollbar(hScrollRect,
		                                     camPosition.x,
				                        canvas.canvasCamera.cameraSize.x, 
				                        -canvas.extents.x, 
				                        canvas.extents.x);
		if (newX != camPosition.x){
			camPosition.x = newX;
			canvas.canvasCamera.cameraGlobalBottomLeftCornerPosition = camPosition;
		}
	}
#endregion

#region TEST_UNDO_REDO
	void testUndoRedo(){
		if (Input.GetKeyDown(KeyCode.Z))
			canvas.undo();
		if (Input.GetKeyDown(KeyCode.X))
			canvas.redo();
	}
#endregion


#region TEST_DRAWING
	void testDrawing(){
		//test bucket
		CanvasController.events.onMouseOverWithButtonDone += onMouseOverWithButtonDoneBucket;
		/* test drawing
		CanvasController.events.onMouseLeftButtonDown += onMouseDownListener;
		CanvasController.events.onMouseOverWithButton += onMouseOverWithButton;
		CanvasController.events.onMouseOverWithButtonDone += onMouseOverWithButtonDone;
		*/
	}

	void onMouseOverWithButtonDoneBucket (IntVector2 position) {
		Color32[] colors = canvas.fetchColors();
		Color32 randomColor = ColorUtil.getRandomColor();
		for (int i = 0; i < colors.Length; i++) {
			colors[i].r = randomColor.r;
			colors[i].g = randomColor.g;
			colors[i].b = randomColor.b;
			colors[i].a = randomColor.a;
		}

		canvas.applyColors(colors,false,true,position);
	}




	List<IntVector2> points;
	void onMouseDownListener (IntVector2 obj) {
		points = new List<IntVector2>();
		points.Add(obj);
	}

	void onMouseOverWithButton (IntVector2 obj) {
		points.Add(obj);
	}
	
	List<IntVector2> interpolatedPath;
	void onMouseOverWithButtonDone (IntVector2 obj) {
		Profiler.BeginSample("onMouseOverWithButtonDone");
		Profiler.BeginSample("init start");
		points.Add(obj);
		InterpolateContext ic = new InterpolateContext (new LinearInterpolationStrategy());
		Profiler.EndSample();

		Profiler.BeginSample("interpolation");
		if (interpolatedPath==null)
			interpolatedPath= new List<IntVector2>();
		else 
			interpolatedPath.Clear();
		ic.interpolate(points, interpolatedPath);
		Profiler.EndSample();
		Profiler.BeginSample("fetch colors");
		Color32[] workingColors= canvas.fetchColors();
		Profiler.EndSample();

		Profiler.BeginSample("apply points with cache");
		TextureUtil.generateTexturePath(canvasTool, ColorUtil.getRandomColor(),interpolatedPath,workingColors,canvasConfig.canvasSize.x, canvasConfig.canvasSize.y);
		Profiler.EndSample();
	
		Profiler.BeginSample("update back layer");
		Debug.Log("change color and use aply method");
		//canvas.applyColors(workingColors, ColorUtil.getRandomColor());
		Profiler.EndSample();
		Profiler.EndSample();
	}
#endregion

}
