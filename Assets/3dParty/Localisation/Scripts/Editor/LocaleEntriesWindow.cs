using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


namespace localisation{
	public class LocaleEntriesWindow : EditorWindow {
		Dictionary<SystemLanguage, LanguageFile> langFileCache;
		SystemLanguage[] languages;
		Dictionary<string, GUILocaleRow> entries;


		[MenuItem("Window/LocaleEntriesEditor")]
		static void createLocaleEntriesWindow(){
			LocaleEntriesWindow window = EditorWindow.GetWindow<LocaleEntriesWindow>();
			window.Show();
			window.initialize();
		}
		
		void initialize(){
			List<LanguageFile> currentLanguages = Locale.instance.languages;
			languages = new SystemLanguage[currentLanguages.Count];
			langFileCache = new Dictionary<SystemLanguage, LanguageFile>();
			for (int i = 0; i < currentLanguages.Count; i++) {
				languages[i] = currentLanguages[i].language;
				langFileCache.Add(languages[i], currentLanguages[i]);
			}
			entries = new Dictionary<string, GUILocaleRow>();

			string key,value;
			foreach(KeyValuePair<SystemLanguage, LanguageFile> langLangFileKVP in langFileCache){
				foreach(LanguageFileEntry langFileEntry in  langLangFileKVP.Value.entries){
				 	key = langFileEntry.key;
					value = langFileEntry.value;
					if (entries.ContainsKey(key)){
						if (entries[key].values.ContainsKey(langLangFileKVP.Key))
							Debug.LogError("something wrong, probably you have duplicates for " 
							               + langLangFileKVP.Key +" language");
						else 
							entries[key].values.Add(langLangFileKVP.Key, value);
					} else {
						GUILocaleRow localeRow = new GUILocaleRow();
						localeRow.key = key;
						localeRow.values = new Dictionary<SystemLanguage, string>();
						localeRow.values.Add(langLangFileKVP.Key, value);
						entries.Add(key,localeRow);
					}
				}	
			}

		}
		
		Vector2 scrollPosition;
		GUILayoutOption keyColumnWidth = GUILayout.Width(100);
		GUILayoutOption langColumnWidth = GUILayout.Width(200);
		string newStringValue;
		void OnGUI(){
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("key", keyColumnWidth);
			for (int i = 0; i < languages.Length; i++) {
				EditorGUILayout.LabelField(languages[i].ToString(), langColumnWidth);
			}
			EditorGUILayout.EndHorizontal();

			foreach(KeyValuePair<string, GUILocaleRow> kvp in entries){
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(kvp.Key, keyColumnWidth);
				for (int i = 0; i < languages.Length; i++) {
					EditorGUI.BeginChangeCheck();
					newStringValue = EditorGUILayout.TextField(kvp.Value.values[languages[i]],langColumnWidth);
					if (EditorGUI.EndChangeCheck()){
						kvp.Value.values[languages[i]]=newStringValue;
						langFileCache[languages[i]].updateKey(kvp.Key, newStringValue);
						EditorUtility.SetDirty(langFileCache[languages[i]]);
					}
				}
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.EndScrollView();
		}

		struct GUILocaleRow{
			public string                             key;
			public Dictionary<SystemLanguage, string> values;
		}
	}
}


