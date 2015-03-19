using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace localisation{
	public static class StringExtension {

		public static string Localized(this string str){
			return Locale.instance.getLocalizedString(str);
		}

		public static void UpdateLocalizedString(this string str, SystemLanguage lang, string value){
			List<LanguageFile> languageFiles = Locale.instance.languages;
			for (int i = 0; i < languageFiles.Count; i++) {
				if (languageFiles[i].language == lang){
					languageFiles[i].updateKey(str, value);
					return;
				}
			}
			Debug.LogError("cant find appropriate language file in Local(singleton)");			
		}
	}
}