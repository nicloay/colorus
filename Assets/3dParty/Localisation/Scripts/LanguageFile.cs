using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace localisation{
	[Serializable]
	public class LanguageFileEntry{
		public string key;
		public string value;
		public LanguageFileEntry (string key, string value)
		{
			this.key = key;
			this.value = value;
		}
		
	}
	
	public class LanguageFile : ScriptableObject {
		public SystemLanguage language;
		public List<LanguageFileEntry> entries;

		public bool containsKey(string key){
			for (int i = 0; i < entries.Count; i++) {
				if (entries[i].key.Equals(key))
					return true;
			}
			return false;
		}

		public void updateKey(string key, string value){
			for (int i = 0; i < entries.Count; i++) {
				if (entries[i].key.Equals(key)){
					entries[i].value = value;
					return;
				}					
			}
			entries.Add(new LanguageFileEntry(key,value));
		}
	}

}
