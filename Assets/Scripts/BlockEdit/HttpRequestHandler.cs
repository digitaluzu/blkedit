using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BlkEdit
{
	// TODO: count + offset + paging
	public class HttpRequestHandler : Uzu.BaseBehaviour
	{
		public const string API_VERSION = "v0";
		public const string PORT = "3000";
		public const string SERVER_URL = "http://localhost:" + PORT + "/api/" + API_VERSION;

		public delegate void OnErrorDelegate (int httpErrorCode, string errorText);
		public event OnErrorDelegate OnError;

		public delegate void OnGetMostRecentEntriesDelegate (BlockInfo data);
		public event OnGetMostRecentEntriesDelegate OnGetMostRecentEntries;

//		public delegate void OnGetImageDelegate (string id, Texture2D texture);
//		public OnGetImageDelegate OnGetImage;

		public void StopAllRequests ()
		{
			StopAllCoroutines ();
		}
		
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

//		public void GetImage (string id, string imageURL)
//		{
//#if UNITY_EDITOR
//			if (OnGetImage == null) {
//				Debug.LogError ("Callback is null.");
//				return;
//			}
//#endif
//
//			StartCoroutine (DoGetImage(id, imageURL));
//		}

		#region Implementation.
		private IEnumerator DoGetMostRecentEntries ()
		{
			string url = SERVER_URL + "/models/most_recent";
			
			WWW www = new WWW (url);
			yield return www;
			
			if (!string.IsNullOrEmpty(www.error)) {
				DoErrorCallback (www.error);
				yield break;
			}

			if (!DoGetMostRecentEntriesReceiveResponse (www.text)) {
				Debug.LogWarning ("JSON parsing error.");
			}
		}

		private bool DoGetMostRecentEntriesReceiveResponse (string text)
		{
			var dict = Uzu.MiniJSON.Deserialize(text) as Dictionary<string,object>;
			if (dict == null) {
				return false;
			}

			List<object> dataObjects;
			if (!BlockInfo.GetValue (dict, "data", out dataObjects)) {
				return false;
			}

			for (int i = 0; i < dataObjects.Count; i++) {
				var dataObject = dataObjects[i] as Dictionary<string, object>;

				BlockInfo info;
				if (BlockInfo.Load (dataObject, out info)) {
					OnGetMostRecentEntries (info);
				}
			}

			return true;
		}

		private void DoErrorCallback (string errorText)
		{
			if (OnError != null) {
				int resultCode = 0;
				TryParseErrorCode (errorText, ref resultCode);
				
				OnError (resultCode, errorText);
			}
		}

		/// <summary>
		/// Attempt to parse the error code from the error string.
		/// </summary>
		private static bool TryParseErrorCode (string errorString, ref int resultCode)
		{
			if (errorString.Length >= 3) {
				string substr = errorString.Substring (0, 3);
				return System.Int32.TryParse (substr, out resultCode);
			}

			return false;
		}

//		private IEnumerator DoGetImage (string id, string imageURL)
//		{
//			WWW www = new WWW (imageURL);
//			yield return www;
//			
//			if (!string.IsNullOrEmpty(www.error)) {
//				Debug.LogError (www.error);
//				yield break;
//			}
//			
//			Debug.Log ("success: ");
//			//Debug.Log (www.text);
//
//			Texture2D texture = www.textureNonReadable;
//			Debug.Log ("Texture: " + texture);
//
//			OnGetImage(id, texture);
//		}
		#endregion
	}
}
