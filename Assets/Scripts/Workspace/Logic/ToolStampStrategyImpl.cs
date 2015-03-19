using UnityEngine;
using System.Collections;

public class ToolStampStrategyImpl:ToolLogicStrategy
{
	Mesh mesh;
	Material material;
	Quaternion rotation= Quaternion.identity;
	bool drawCursor=false;

	Camera cnavasCam;
	Texture2D iconTexture;

	static float CURSOR_Z_POSITION = 25;
	CanvasController canvas;

	public ToolStampStrategyImpl(){
		canvas = PropertiesSingleton.instance.canvasWorkspaceController.canvas;
	}



#if !UNITY_IPHONE
	#region ToolLogicStrategy implementation
	public void onMouseDown (IntVector2 position)
	{	
	}

	public void onMouseOverWithButton (IntVector2 position,Vector3 globalCursorPosition)
	{ 
	}

	public void onMouseOverWithButtonDone (IntVector2 position)
	{
		applyStamp(position);
	}


	public void onMouseEnter ()
	{
		initCustomCursor();
		Screen.showCursor = false;
		drawCursor = true;	
		WorkspaceEventManager.instance.onMouseEnterScrollbar += onScrollBarEnterListener;
		WorkspaceEventManager.instance.onMouseExitScrollbar  += onScrollBarExitListener;
	}
	
	public void onMouseExit ()
	{
		hideCustomCursor();
		WorkspaceEventManager.instance.onMouseEnterScrollbar -= onScrollBarEnterListener;
		WorkspaceEventManager.instance.onMouseExitScrollbar  -= onScrollBarExitListener;
	}




	void hideCustomCursor(){
		Screen.showCursor = true;
		drawCursor = false;
	}


	public void onMouseOver (IntVector2 pixelCursorPosition, Vector3 globalCursorPosition)
	{
		if (drawCursor)	
			drawMesh(globalCursorPosition);
	}


	
	#endregion
	
	void onScrollBarEnterListener () {
		Screen.showCursor = true;
		drawCursor = false;
	}
	
	void onScrollBarExitListener () {
		Screen.showCursor = false;
		drawCursor = true;
	}


#else
	IntVector2 lastPosition;
	public void onTouchStart (IntVector2 pixelPosition, Vector3 globalPosition)
	{
		initCustomCursor();
		lastPosition = pixelPosition;
	}

	public void onTouchOver (IntVector2 pixelPosition, Vector3 globalPosition)
	{
		drawMesh(globalPosition);
		lastPosition = pixelPosition;
	}

	public void onTouchEnd ()
	{
		applyStamp(lastPosition);
	}
#endif

	void applyStamp(IntVector2 position){
		Color32[] colors= canvas.fetchColors();
		Color32 currentColor = PropertiesSingleton.instance.colorProperties.activeColor;
		Color32 secondColor = PropertiesSingleton.instance.colorProperties.secondColor;
		TextureColorArray src = new TextureColorArray (iconTexture.width, iconTexture.height, iconTexture.GetPixels32 ());
		TextureColorArray dst = new TextureColorArray ((int)canvas.size.x, (int)canvas.size.y, colors);	
		TextureUtil.applyTextureToAnotherTexture (ref src, ref dst, position);
		for (int i = 0; i < colors.Length; i++) {
			if (colors[i].a > 0){
				if (colors[i].r == 0){
					colors[i].r = secondColor.r;
					colors[i].g = secondColor.g;
					colors[i].b = secondColor.b;
				} else {
					colors[i].r = currentColor.r;
					colors[i].g = currentColor.g;
					colors[i].b = currentColor.b;
				}
			}
		}
		canvas.applyColors(colors);
	}

	void drawMesh(Vector3 globalCursorPosition){
		globalCursorPosition.z = CURSOR_Z_POSITION;
		Graphics.DrawMesh(mesh,globalCursorPosition,rotation,material,8, cnavasCam);
	}


	Color32 mainColor,secondColor;

	void initCustomCursor(){
		if (material == null) {
			material = PropertiesSingleton.instance.stampCursorMaterial;					
		}
		iconTexture = PropertiesSingleton.instance.activeStampTexture;
		material.mainTexture = iconTexture;
		mesh = MeshUtil.createPlaneMesh(iconTexture.width, iconTexture.height);
		cnavasCam = PropertiesSingleton.instance.canvasWorkspaceController.camera;

	}
}


