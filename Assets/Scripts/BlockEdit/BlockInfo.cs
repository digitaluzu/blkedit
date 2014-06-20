using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace BlkEdit
{
	public struct BlockInfo
	{
		public static string Extension {
			get { return "blki"; }
		}

		/// <summary>
		/// Id used to differentiate local files.
		/// Files that are uploaded to the server will be assigned
		/// their own unique id.
		/// </summary>
		public static string GetNewId ()
		{
			System.Guid guid = System.Guid.NewGuid ();
			return string.Format (@"{0}", guid);
		}

		public static bool Save (string path, BlockInfo info)
		{
			var dict = new Dictionary<string, object> ();

			{
				dict.Add ("id", info.Id);

				dict.Add ("name", info.Name);
				dict.Add ("blockDataPath", info.BlockDataPath);
				dict.Add ("imagePath", info.ImagePath);
//				dict.Add ("imageURL", info.ImageURL);
				dict.Add ("downloadCount", info.DownloadCount);
				dict.Add ("likeCount", info.LikeCount);
				dict.Add ("version", info.Version);
			}

			string jsonStr = Uzu.MiniJSON.Serialize (dict);

			using (StreamWriter writer = new StreamWriter (path)) {
				writer.Write (jsonStr);
			}

			return true;
		}

		public static bool Load (string path, out BlockInfo info)
		{
			string jsonStr = string.Empty;
			using (StreamReader reader = new StreamReader(path)) {            
				jsonStr = reader.ReadToEnd();
			}

			var dict = Uzu.MiniJSON.Deserialize (jsonStr) as Dictionary<string, object>;
			return Load (dict, out info);
		}

		public static bool Load (Dictionary <string, object> dict, out BlockInfo info)
		{
			info = new BlockInfo ();

			{
				if (!GetValue (dict, "id", out info.Id)) {
					return false;
				}

				GetValue (dict, "name", out info.Name);
				GetValue (dict, "blockDataPath", out info.BlockDataPath);
				GetValue (dict, "imagePath", out info.ImagePath);
				GetValue (dict, "imageURL", out info.ImageURL);
				GetValue (dict, "downloadCount", out info.DownloadCount);
				GetValue (dict, "likeCount", out info.LikeCount);
				GetValue (dict, "version", out info.Version);
			}

			return true;
		}

		public string Id;
		public string Name;
		public string BlockDataPath;
		public string ImagePath;
		public string ImageURL;

		public int DownloadCount;
		public int LikeCount;
		public int Version;

		// TODO: Can edit? Is uploaded? etc

		public override string ToString()
		{
			return string.Format("id={0},name={1},blockDataPath={2},imagePath={3},imageURL={4},downloadCount={5},likeCount={6},version={7}",
			                     Id, Name, BlockDataPath, ImagePath, ImageURL, DownloadCount, LikeCount, Version);
		}

		// TODO: move to utility in UzuCore?
		public static bool GetValue (Dictionary <string, object> dict, string keyName, out List<object> result)
		{
			object tmpObj;
			if (dict.TryGetValue (keyName, out tmpObj)) {
				result = tmpObj as List <object>;
				return result != null;
			}
			
			result = null;
			return false;
		}

		public static bool GetValue (Dictionary <string, object> dict, string keyName, out string result)
		{
			object tmpObj;
			if (dict.TryGetValue (keyName, out tmpObj)) {
				result = tmpObj as string;
				return result != null;
			}
			
			result = null;
			return false;
		}

		public static bool GetValue (Dictionary <string, object> dict, string keyName, out int result)
		{
			object tmpObj;
			if (dict.TryGetValue (keyName, out tmpObj)) {
				// JSON library only supports long.
				long? tmpResult = tmpObj as long?;
				if (tmpResult.HasValue) {
					result = (int)tmpResult.Value;
					return true;
				}
			}

			result = 0;
			return false;
		}
	}
}
