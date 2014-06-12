using UnityEngine;
using System.Collections.Generic;

namespace Blk
{
	// TODO: post requests should use API key
	// TODO: error handling... pass error / result via callback so caller can handle with dialogs and things.
	public class HttpRequestHandler : Uzu.BaseBehaviour
	{
		public delegate void OnGetIdsResponseDelegate (Uzu.SmartList <int> ids);
		
		public void GetIds (OnGetIdsResponseDelegate callback)
		{
			StartCoroutine (DoGetIds(callback));
		}

		#region Implementation.
		private Uzu.SmartList <int> _ids = new Uzu.SmartList<int> ();

		private System.Collections.IEnumerator DoGetIds (OnGetIdsResponseDelegate callback)
		{
			string url = Constants.SERVER_URL + "ids";
			
			WWW www = new WWW (url);
			yield return www;
			
			if (!string.IsNullOrEmpty(www.error)) {
				Debug.LogError (www.error);
				yield break;
			}
			
			Debug.Log ("success: ");
			Debug.Log (www.text);

			var dict = UzuMiniJSON.Json.Deserialize(www.text) as Dictionary<string,object>;
			if (dict != null) {
				object idsValue;
				if (dict.TryGetValue ("ids", out idsValue)) {
					List<object> ids = idsValue as List <object>;
					if (ids != null) {
						_ids.Clear ();
						for (int i = 0; i < ids.Count; i++) {
							long? val = ids [i] as long?;
							if (val.HasValue) {
								_ids.Add ((int)val.Value);
							}
						}
						
						if (callback != null) {
							callback (_ids);
						}
					}
				}
			}
			else {
				Debug.Log ("No dict!");
			}
		}
		#endregion
	}
}
