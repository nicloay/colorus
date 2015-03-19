using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineStrategyImpl : ToolLogicStrategy {
	IntVector2                    firstPoint                                  ;
	LineMeshTypeInt 	      lineRendererMeshType;
	InterpoalateStrategyInterface interpolateStrategy                         ;
	Mesh                          mesh                                        ;
	Mesh                          maskMesh                                    ;
	Material                      material                                    ;
	Quaternion                    rotation              = Quaternion.identity ;
	bool                          drawCursor            = false               ;

	LineRendererController 	      lineRendererController;

	CanvasConfig 		      canvasConfig;
	CanvasController              canvas;
	CanvasTool                    tool;

	Camera                        canvasCam;

	public LineStrategyImpl (CanvasTool tool, InterpoalateStrategyInterface interpolateStrategy) {
		this.tool = tool;
		this.interpolateStrategy  = interpolateStrategy  ;
		canvasConfig = PropertiesSingleton.instance.canvasWorkspaceController.canvas.config;
		canvas = PropertiesSingleton.instance.canvasWorkspaceController.canvas;
		canvasCam = canvas.canvasCamera.camera;
		switch (tool.meshType){
		case LineMeshType.SQUARE_SIDES:
			lineRendererMeshType = new SquareSideLineMeshTypeImpl();
			break;
		case LineMeshType.TRIANGLE_SIDES:
			lineRendererMeshType = new TriangleSideLineMeshTypeImpl();
			break;
		}
	}
	
#region ToolLogicStrategy implementation

#if !UNITY_IPHONE
	public void onMouseDown (IntVector2 pixelPosition) {
		startWithPoint(pixelPosition);
	}



	public void onMouseOverWithButton (IntVector2 position, Vector3 globalPosition) {
		addNewPoint(position);
	}
	
	public void onMouseOverWithButtonDone (IntVector2 position) {
		addNewPoint(position);
		finishDrawing();
	}

	public void onMouseEnter () {
		Screen.showCursor = false;
		if (material == null) {
			material = PropertiesSingleton.getLineRendererMaterial ();					
		}
		material.mainTexture = tool.pointerTexture;
		if (mesh == null) {
			mesh = MeshUtil.createPlaneMesh (material.mainTexture.width, material.mainTexture.height);
		}
		if (maskMesh == null) {

			maskMesh = MeshUtil.createPlaneMesh(tool.pointerMaskTexture.width, tool.pointerMaskTexture.height);
		}
		PropertiesSingleton.instance.cursors.cursorMaskMaterial.mainTexture = tool.pointerMaskTexture;
		drawCursor = true;
		material.color = PropertiesSingleton.instance.colorProperties.activeColor;
		
		WorkspaceEventManager.instance.onMouseEnterScrollbar += onScrollBarEnterListener;
		WorkspaceEventManager.instance.onMouseExitScrollbar  += onScrollBarExitListener;
	}
	
	public void onMouseExit () {
		Screen.showCursor = true;
		drawCursor = false;	

		WorkspaceEventManager.instance.onMouseEnterScrollbar -= onScrollBarEnterListener;
		WorkspaceEventManager.instance.onMouseExitScrollbar  -= onScrollBarExitListener;
	}

	public void onMouseOver (IntVector2 pixelCursorPosition, Vector3 globalCursorPosition) {
		if (drawCursor) {
			Graphics.DrawMesh (mesh, globalCursorPosition, rotation, material, 8, canvasCam);
			Graphics.DrawMesh (maskMesh, globalCursorPosition, rotation,
			                   PropertiesSingleton.instance.cursors.cursorMaskMaterial, 8, canvasCam);
		}
	}

	void onScrollBarEnterListener () {
		Screen.showCursor = true;
		drawCursor = false;
	}

	void onScrollBarExitListener () {
		Screen.showCursor = false;
		drawCursor = true;
	}

#else
	IntVector2 previousPoint;
	public void onTouchStart (IntVector2 pixelPosition, Vector3 globalPosition)
	{
		startWithPoint(pixelPosition);
		addNewPoint(pixelPosition);
		previousPoint = pixelPosition;
	}

	public void onTouchOver (IntVector2 pixelPosition, Vector3 globalPosition)
	{
		if (pixelPosition.equalsTo(previousPoint))
		    return;
		addNewPoint(pixelPosition);
		previousPoint = pixelPosition;
	}

	public void onTouchEnd ()
	{
		finishDrawing();
	}
#endif

	void startWithPoint(IntVector2 pixelPosition){
		firstPoint = pixelPosition;
		if (lineRendererController==null){
			lineRendererController = LineRendererController.getLineRenderer ("lineRenderer", lineRendererMeshType, tool.lineTexture);
		}
	}

	void addNewPoint(IntVector2 point){
		lineRendererController.addPoint (point);
	}

	List<IntVector2> interpolatedPath;
	void finishDrawing(){
		Color32[] colors= canvas.fetchColors();
		bool useMask = PropertiesSingleton.instance.drawWithinRegion;
		if (lineRendererController == null)
			return;

		List<IntVector2> points = lineRendererController.getPointArray();
		if (interpolatedPath == null)
			interpolatedPath = new List<IntVector2>();
		else 
			interpolatedPath.Clear();
		
		InterpolateContext ic = new InterpolateContext (interpolateStrategy);
		ic.interpolate (points,interpolatedPath);
		
		TextureUtil.generateTexturePath(tool, PropertiesSingleton.instance.colorProperties.activeColor, interpolatedPath, colors, canvasConfig.canvasSize.x, canvasConfig.canvasSize.y);
		
		if (useMask){
			bool[,] maskColors = TextureUtil.floodFillLineGetRegion (firstPoint, canvas.actualColors, canvas.persistentLayer, canvasConfig.canvasSize.x, canvasConfig.canvasSize.y);
			int tCounter = 0;
			for (int y = 0; y < canvasConfig.canvasSize.y; y++) {
				for (int x = 0; x < canvasConfig.canvasSize.x; x++) {
					if (! maskColors[x,y])
						colors[tCounter].a = 0;
					tCounter++;
				}
			}
		} 
		
		if (lineRendererController!=null)
			lineRendererController.selfDestroy();
		canvas.applyColors(colors);
	}
#endregion
}

