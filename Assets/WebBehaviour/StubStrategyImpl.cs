using UnityEngine;
using System.Collections;
using localisation;

public class StubStrategyImpl : WebStrategyInt {
	#region WebStrategyInt implementation
	public void onStart (string data){
		
	}

	public void onNewPictureOpen(SheetObject sheetObject){
		string text= "Colorus | "+sheetObject.nameKey.Localized();
		string eval="document.title=\"TEXT\";".Replace("TEXT",text);
		Debug2.LogDebug(" onNewPictureOpen evaluation eval=\n"+eval);
		Application.ExternalEval(eval);
	}

	public void onPictureSave (Texture2D texture, string pictureName){		
		string data = System.Convert.ToBase64String(texture.EncodeToPNG());
        	string filename="colorus.png";
		Application.ExternalCall("Download.save('"+data+"','"+filename+"')");	
	}
	
	public bool isReady ()
	{
		return true;
	}
	#endregion
}
