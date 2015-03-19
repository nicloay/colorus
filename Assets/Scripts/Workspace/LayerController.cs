using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayerController : MonoBehaviour, EditorConstructionInterface {
	static string LINE_RENDERER_GO_NAME = "lineRenderer";
	bool                   fillAnimate = false     ;
	float                  animationStartTime;
	float                  duration;
	LineRendererController lrc;

	int _width=-1;
	int _height=-1;

	int width{
		get{
			if (_width == -1)
				_width = getMaterialTexture().width;
			return _width;
		}
	}

	int height{
		get{
			if (_height == -1)
				_height = getMaterialTexture().height;
			return _height;
		}
	}


	static Dictionary<LineMeshType, LineMeshTypeInt> supportedMeshLineTypes = new Dictionary<LineMeshType, LineMeshTypeInt>(){
		{LineMeshType.SQUARE_SIDES, new SquareSideLineMeshTypeImpl()},
		{LineMeshType.TRIANGLE_SIDES, new TriangleSideLineMeshTypeImpl()}
	};	
	
	
	public LineRendererController getLineRendererControlelr (LineMeshType lineMeshType, Texture2D pointerTexture) {
		if (lrc == null) {
			lrc = LineRendererController.getLineRenderer (LINE_RENDERER_GO_NAME, supportedMeshLineTypes[lineMeshType], pointerTexture);
		}		
		return lrc;	
	}
	
	public void destroyLineRenderer () {
		if (lrc != null) {
			lrc.selfDestroy ();	
			lrc = null;
		}
	}
	
	public void initialize () {
		deactivate ();
	}

	public void reset () {
		//stub, nothing need to do right now
	}	
	
	void FixedUpdate () {		
		if (fillAnimate) {
			updateAnimationVars ();	
		}
	}

	void updateAnimationVars () {
		float value = InterpolationUtil.moveInLinear(Time.timeSinceLevelLoad - animationStartTime, 0,duration, 1);
		if (value >=1){
			value = 100;
			fillAnimate = false;
		}
		getMaterial().SetFloat("_FillValue",value);
	}
	
	public bool isActive () {
		return gameObject.activeSelf;	
	}
	
	public void activate () {
		gameObject.renderer.enabled = true;
	}
	
	public void activateWithRadialAnimation (IntVector2 center) {
		float maxX = Mathf.Max(center.x, width - center.x);
		float maxY = Mathf.Max(center.y, height - center.y);

		Vector2 v1 = getUvPoint(maxX, maxY);
		float ratioX = (width  - maxX) / maxX;
		float ratioY = (height - maxY) / maxY;

		Vector2 v2 = new Vector2(v1.x * ratioX, v1.y * ratioY);

		float x0, x1, y0, y1;
		if (center.x == (int)maxX){
			x0 = 0.5f - v1.x;
			x1 = 0.5f + v2.x;
		} else {
			x0 = 0.5f - v2.x;
			x1 = 0.5f + v1.x;
		}

		if (center.y == (int)maxY){
			y0 = 0.5f - v1.y;
			y1 = 0.5f + v2.y;
		} else {
			y0 = 0.5f - v2.y;
			y1 = 0.5f + v1.y;
		}
		Vector2[] uv1 = new Vector2[6];
		uv1[0] =          new Vector2(x0, y0);
		uv1[1] = uv1[5] = new Vector2(x0, y1);
		uv1[2] = uv1[3] = new Vector2(x1, y0);
		uv1[4] =          new Vector2(x1, y1);

		gameObject.renderer.enabled = true;

		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.uv1 = uv1;

		fillAnimate = true;
		animationStartTime = Time.timeSinceLevelLoad;
		duration = PropertiesSingleton.instance.floodFillingTime;
		updateAnimationVars();
	}

	Vector2 getUvPoint(float x, float y){
		float diag=Mathf.Sqrt(x * x + y * y);
		return new Vector2(0.5f * x / diag, 0.5f * y / diag);
	}

	public void deactivate () {
		fillAnimate = false;
		getMaterial().SetFloat("_FillValue",100);
		gameObject.renderer.enabled = false;
	}

	public void fastClearTexture(Color32[] clearPixels){
		Texture2D mainTex = getMaterialTexture();
		mainTex.SetPixels32(clearPixels);
		mainTex.Apply();
	}

	public void clearTexture (Color32[] colors) {
		
		Texture2D mainTex = getMaterialTexture();
		colors = mainTex.GetPixels32 ();
		ColorUtil.clearPixels(colors);
		mainTex.SetPixels32 (colors);
		mainTex.Apply ();
			
	}
	
	public void setColor (Color layerColor) {	
		getMaterial ().color = layerColor;	
	}


	public void applyTextureToPoint (Texture2D texture, IntVector2 point, Color32 newColor) {
		Color32[] newColors = TextureUtil.generateTextureColorsByPatternAndPoint (width, height, texture, point);		
		applyNewColors(newColors, newColor);
	}
	
	public void applyTextureToPoints (Texture2D texture, List<IntVector2> points, Color32 newColor) {
		Color32[] newColors = TextureUtil.generateTextureColorsByPatternAndPoints (width, height, texture, points);		
		applyNewColors(newColors, newColor);
	}
	
	public void applyNewColors (Color32[] colors, Color32 layerColor) {
		Texture2D texture = getMaterialTexture();				
		texture.SetPixels32 (colors);
		texture.Apply (false);
		getMaterial ().color = layerColor;		
	}

	public void copyLayerTextureToTexture (Texture2D destinationTexture) {
				
		Color32[] destinationColors = destinationTexture.GetPixels32 ();		
		copyLayerTextureToPixelArray (ref destinationColors);		
		
		destinationTexture.SetPixels32 (destinationColors);
		destinationTexture.Apply ();		
	}
	
	public void copyLayerTextureToPixelArray (ref Color32[] destinationColors) {
		Color32[] layerColors = getTexturePixelArray ();
		Color32 layerColor = getMaterial ().color;
		if (layerColors.Length != destinationColors.Length)
			Debug.LogError ("destination texture and layer texture must have the same size");

		for (int i = 0; i < layerColors.Length; i++) {
			if (layerColors [i].a != 0) {					
				destinationColors [i] = Color32.Lerp (destinationColors [i], layerColor, (float)layerColors [i].a / 255);			
			}
		}
	}
	
	Texture2D mainTexture;	
	private Color32[] getTexturePixelArray () {
		return getMaterialTexture().GetPixels32 ();
	}
	
	private Texture2D getMaterialTexture(){
		if (mainTexture == null)
			mainTexture = (Texture2D)getMaterial ().mainTexture;
		return mainTexture;
	}
	
	Material mat;
	private Material getMaterial () {
		if (mat==null)
			mat = transform.renderer.material;
		return mat;
	}
}
