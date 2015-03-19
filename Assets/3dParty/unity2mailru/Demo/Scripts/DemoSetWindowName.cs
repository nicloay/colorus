using UnityEngine;
using System.Collections;

public class DemoSetWindowName : MonoBehaviour {
	Rect windowRect = new Rect(20, 20, 240, 100);
	string text="";
	
    void OnGUI() {
        windowRect = GUILayout.Window(10, windowRect, DoMyWindow, "browser window name");
    }
	
   	void DoMyWindow(int windowID) {		
		text= GUILayout.TextArea(text);
		if (GUILayout.Button("change window text")){
			MRUController.instance.callMailruByParams("mailru.app.utils.setTitle",text);
		}
		GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
    }
}
