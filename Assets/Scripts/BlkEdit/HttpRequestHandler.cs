using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Blk
{
	// TODO: post requests should use API key
	// TODO: error handling... pass error / result via callback so caller can handle with dialogs and things.
	// TODO: parse error code from error string (if possible)... WWW class doesn't give a nicely formatted error code, so we have to do it ourself.
	public class HttpRequestHandler : Uzu.BaseBehaviour
	{
		public delegate void OnGetMostRecentEntriesDelegate (DataInfo data);
		public OnGetMostRecentEntriesDelegate OnGetMostRecentEntries;

		public delegate void OnGetImageDelegate (string id, Texture2D texture);
		public OnGetImageDelegate OnGetImage;
		
		public void GetMostRecentEntries ()
		{
#if UNITY_EDITOR
			if (OnGetMostRecentEntries == null) {
				Debug.LogError ("Callback is null.");
				return;
			}
#endif

			StartCoroutine (DoGetMostRecentEntries());
		}

		public void GetImage (string id, string imageURL)
		{
#if UNITY_EDITOR
			if (OnGetImage == null) {
				Debug.LogError ("Callback is null.");
				return;
			}
#endif

			StartCoroutine (DoGetImage(id, imageURL));
		}

		#region Implementation.
		private IEnumerator DoGetMostRecentEntries ()
		{
			// TODO: count + offset + paging
			string url = Constants.SERVER_URL + "/models/most_recent";
			
			WWW www = new WWW (url);
			yield return www;
			
			if (!string.IsNullOrEmpty(www.error)) {
				Debug.LogError (www.error);
				yield break;
			}
			
			Debug.Log ("success: ");
//			Debug.Log (www.text);

			Debug.Log (Omg(www.text));
		}

		public struct DataInfo {
			public string id;
			public string name;
			public string imageURL;
			public int downloadCount;
			public int likeCount;
			public int version;
		}

		private bool Omg (string text)
		{
			var dict = UzuMiniJSON.Json.Deserialize(text) as Dictionary<string,object>;
			if (dict == null) {
				return false;
			}

			// TODO: better error handling
			object tmp;
			if (dict.TryGetValue ("data", out tmp)) {
				List<object> dataObjects;
				if (!AsList (tmp, out dataObjects)) {
					return false;
				}

				for (int i = 0; i < dataObjects.Count; i++) {
					var dataObject = dataObjects[i] as Dictionary<string, object>;

					DataInfo dataInfo = new DataInfo();
					dataInfo.id = (string)dataObject ["id"];
					dataInfo.imageURL = (string)dataObject ["imageURL"];
					dataInfo.downloadCount = AsInt(dataObject ["downloadCount"]);
					dataInfo.likeCount = AsInt(dataObject ["likeCount"]);
					dataInfo.version = AsInt(dataObject ["version"]);

					OnGetMostRecentEntries (dataInfo);
				}

				return true;
			}

			return false;
		}

		private bool AsList(object obj, out List<object> res)
		{
			res = obj as List<object>;
			return res != null;
		}

		private int AsInt(object obj)
		{
			// JSON library only supports long.
			return (int)(long)obj;
		}

		private IEnumerator DoGetImage (string id, string imageURL)
		{
			WWW www = new WWW (imageURL);
			yield return www;
			
			if (!string.IsNullOrEmpty(www.error)) {
				Debug.LogError (www.error);
				yield break;
			}
			
			Debug.Log ("success: ");
			//Debug.Log (www.text);

			Texture2D texture = www.textureNonReadable;
			Debug.Log ("Texture: " + texture);

			OnGetImage(id, texture);
		}
		#endregion
	}
}
