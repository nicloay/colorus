using UnityEngine;
using System.Collections;
using UnityEditor;

public class OptimizeSheetObjectBorderLayer : ScriptableWizard {
	
	public SheetObject sheetObject;
	
	
	[MenuItem ("Razukrashka/Optimize Sheet Object Border Layer")]
    static void CreateWizard () {
        ScriptableWizard.DisplayWizard<OptimizeSheetObjectBorderLayer>("OptimizeSheetObjectBorderLayer", "Create");
        //If you don't want to use the secondary button simply leave it out:
        //ScriptableWizard.DisplayWizard<WizardCreateLight>("Create Light", "Create");
    }
	
	 void OnWizardCreate () {
        Color32[] borderColors = sheetObject.persistentBorderLayer.GetPixels32();
		Color32[] frontColors  = sheetObject.persistentFrontOutline.GetPixels32();
		for (int i = 0; i < borderColors.Length; i++) {
			if (borderColors[i].a != 0 || borderColors[i].r != 0 || borderColors[i].g != 0 || borderColors[i].b != 0){
				if (frontColors[i].a == 255){
					borderColors[i].a = 255;	
				}else{
					borderColors[i] = Color.clear;
				} 
			}
		}
		sheetObject.persistentBorderLayer.SetPixels32(borderColors);
		sheetObject.persistentBorderLayer.Apply();
		
		TextureUtil.updateTextureAsset(sheetObject.persistentBorderLayer);
    }  
}
