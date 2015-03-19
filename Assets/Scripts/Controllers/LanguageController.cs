using UnityEngine;
using System.Collections;

public class LanguageController : ControllerInterface {
	#region ControllerInterface implementation

	public void init () {
#if !UNITY_IPHONE
		if (Application.systemLanguage == SystemLanguage.Russian){
			PropertiesSingleton.instance.language = SystemLanguage.Russian;
		} else {
			PropertiesSingleton.instance.language = SystemLanguage.English;
		}
#endif
	}


#endregion

}
