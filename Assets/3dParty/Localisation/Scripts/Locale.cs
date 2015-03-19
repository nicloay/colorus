using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace localisation{
	public class Locale : MonoSingleton<Locale> {
		public List<LanguageFile> languages;
		public Dictionary<SystemLanguage,Dictionary<string,string>> allLanguagesCache;
		PropertiesSingleton props;

		public override void Init ()
		{
			props = PropertiesSingleton.instance;
			if (props.language == null)
				props.language = SystemLanguage.English;
			allLanguagesCache = new Dictionary<SystemLanguage, Dictionary<string, string>>();
			foreach (LanguageFile file  in languages) {
				Dictionary<string,string> langDict =  new Dictionary<string,string>();
				for (int i = 0; i < file.entries.Count; i++) {
					langDict.Add(file.entries[i].key, file.entries[i].value);
				}
				allLanguagesCache.Add(file.language,langDict);
			}
		}


		string result;
		public string getLocalizedString(string key){
			result = key;
			if (!allLanguagesCache.ContainsKey(props.language))
				Debug.LogError("Language not found "+props.language.ToString());
			else if (!allLanguagesCache[props.language].ContainsKey(key))
				Debug.LogError("key ["+key+"] not found in language " + props.language.ToString());
			else 
				result = allLanguagesCache[props.language][key];
			return result;
		}
	}
}