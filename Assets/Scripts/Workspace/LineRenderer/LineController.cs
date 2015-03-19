using UnityEngine;
using System.Collections;

public class LineController : MonoBehaviour {

	private bool            drawMesh = false ;
	private Material        mat              ;
	private Mesh            mesh             ;
	private LineMeshTypeInt lineMeshType     ;
	
	private LineController () {
	}
	
	public void initialize (LineMeshTypeInt lineMeshType, Material mat) {
		this.lineMeshType = lineMeshType;
		this.mat = mat;
	}
	
	// Update is called once per frame
	void Update () {
		if (drawMesh) {
			renderMesh ();	
		}	
	}

	private void renderMesh () {
		Graphics.DrawMesh (mesh, Vector3.zero, Quaternion.identity, mat, 8);
	}
	
	public void setPoints (IntVector2 pointA, IntVector2 pointB) {
		drawMesh = true;				
		mesh = lineMeshType.GetLineMesh (pointA, pointB, mat.mainTexture.width, mat.mainTexture.height);
	}
	
	public void setPoint (IntVector2 point) {
		drawMesh = true;
		mesh = lineMeshType.GetPointMesh(point,mat.mainTexture.width, mat.mainTexture.height);
		mat = PropertiesSingleton.getLineRendererMaterial ();		
	}
	
	public static LineController createInstance (string name, LineMeshTypeInt lineMeshType, Transform parentTransform) {
		GameObject go = new GameObject (name);
		go.transform.parent = parentTransform;
		LineController lc = go.AddComponent<LineController> ();
		lc.initialize (lineMeshType,PropertiesSingleton.getLineRendererMaterial ());
		return lc;
	}
	
}
