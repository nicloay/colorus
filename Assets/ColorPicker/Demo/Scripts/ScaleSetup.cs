using UnityEngine;
using System.Collections;
namespace colorpicker{
	public class ScaleSetup : MonoBehaviour {
		Rect useScaleRect = new Rect(10,10,140,20);
		Rect scaleRect = new Rect(150,15,50,20);
		Rect normalScaleRect = new Rect(210,10,40,20);
		
		float scale = 1;
		public bool useScaleMatrix=false;
		public Matrix4x4 scaleMatrix = Matrix4x4.identity;
		
		private ScaleSetup()
		{			
		}
		
		static ScaleSetup _instance;		
		public static ScaleSetup instance {
			get{
				if (_instance ==null){
					GameObject go = new GameObject("scaleSetup[autoInitialized]");
					_instance = go.AddComponent<ScaleSetup>();					
				}					
				return _instance;
			}
		}		
		
		void OnGUI () {
			float newScale;
			bool newUseScaleMatrix =GUI.Toggle(useScaleRect,useScaleMatrix, "use GUI.matrix scale");			
			if (newUseScaleMatrix){
				newScale = GUI.HorizontalSlider(scaleRect,scale,0.2f,5.0f);
				if (GUI.Button(normalScaleRect,"x1"))
					newScale=1.0f;
				
				if (newScale!=scale){
					scale = newScale;
					scaleMatrix = Matrix4x4.TRS(Vector3.zero,
						Quaternion.identity,
						new Vector3( scale,
						scale,
						1.0f));					
				}				
			} else {
				if (newUseScaleMatrix != useScaleMatrix && scale!=1){
					scaleMatrix = Matrix4x4.identity;
				}
			}
			useScaleMatrix = newUseScaleMatrix;
		}		
	}	
}
