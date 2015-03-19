using UnityEditor;
using UnityEngine;
using System.Collections;

public class CameraSizeInPixelWizard : ScriptableWizard {
	
	public Camera camera;
	public Vector2 normalSize=new Vector2(1024,768);
	public Vector2 leftBottomCorner;
	public Vector2 size;
	
	[MenuItem ("Razukrashka/Adjust camera sizePosition")]
	static void CreateWizard(){
		ScriptableWizard.DisplayWizard<CameraSizeInPixelWizard>("Adjust Camera", "Adjust");
	}
 	void OnWizardCreate () {
       		float width = size.x / normalSize.x;
		float height = size.y / normalSize.y;
		float left = leftBottomCorner.x / normalSize.x;
		float top = leftBottomCorner.y / normalSize.y;
		camera.rect = new Rect(left,top, width,height);
    	}  
	
}

