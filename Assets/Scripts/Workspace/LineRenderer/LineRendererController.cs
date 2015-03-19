using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LineRendererController : MonoBehaviour {
	private int               lineNumber     = 0                        ;
	private IntVector2        previousPoint                             ;
	private Queue<IntVector2> pointQueue     = new Queue<IntVector2> () ;
	private LineMeshTypeInt   lineMeshType                              ;
	
	public List<LineItem> line;
	
	private LineRendererController()
	{
	}

	Camera renderCam;
	Material lineMaterial;

	public void initialize(LineMeshTypeInt lineMeshType){
		this.lineMeshType = lineMeshType;	
		lineMaterial = PropertiesSingleton.instance.lineRendererMaterial;
		renderCam = PropertiesSingleton.instance.canvasWorkspaceController.canvas.canvasCamera.camera;
	}
	
	public void addPoint (IntVector2 point) {
		LineItem lc = new LineItem (lineMeshType, lineMaterial);
		if (lineNumber == 0) {
			lc.setPoint (point);			
		} else {
			if (point.equalsTo (previousPoint))
				return;
			lc.setPoints (previousPoint, point);
		}
		line.Add(lc);
		previousPoint = point;			
		pointQueue.Enqueue (point);
		lineNumber++;
	}
	
	public List<IntVector2> getPointArray () {
		return pointQueue.ToList();
	}
	
	public void selfDestroy () {
		for (int i = 0; i < line.Count; i++) {
			Destroy(line[i].mesh);
		}
		line = null;
		
		GameObject.Destroy (gameObject);
		System.GC.Collect();
	}
	
	public static LineRendererController getLineRenderer (string name, LineMeshTypeInt lineMeshType, Texture2D pointerTexture) {		

		PropertiesSingleton.instance.lineRendererMaterial.mainTexture = pointerTexture;		
		GameObject go = new GameObject (name);
		go.transform.parent = PropertiesSingleton.instance.canvasWorkspaceController.transform;
		go.transform.localPosition = Vector3.forward;
		LineRendererController lrc = go.AddComponent<LineRendererController> ();
		lrc.initialize(lineMeshType);
		lrc.line = new List<LineItem>();
		return lrc;
	}
	
	void Update(){
		for (int i = 0; i < line.Count; i++) {
			line[i].renderMesh(renderCam);
		}
	}
	
}
