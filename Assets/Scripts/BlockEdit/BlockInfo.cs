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

		public static bool Save (string path, BlockInfo info)
		{
			var dict = new Dictionary<string, object> ();

			{
				dict.Add ("id", info.Id);
				dict.Add ("name", info.Name);
				dict.Add ("blockDataPath", info.BlockDataPath);
				dict.Add ("imagePath", info.ImagePath);
			}

			string jsonStr = UzuMiniJSON.Json.Serialize (dict);

			using (StreamWriter writer = new StreamWriter (path)) {
				writer.Write (jsonStr);
			}

			return true;
		}

		public static bool Load (string path, out BlockInfo info)
		{
			info = new BlockInfo ();

			string jsonStr = string.Empty;
			using (StreamReader reader = new StreamReader(path)) {            
				jsonStr = reader.ReadToEnd();
			}

			var dict = UzuMiniJSON.Json.Deserialize (jsonStr) as Dictionary<string,object>;

			{
				info.Id = (string)dict ["id"];
				info.Name = (string)dict ["name"];
				info.BlockDataPath = (string)dict ["blockDataPath"];
				info.ImagePath = (string)dict ["imagePath"];
			}

			return true;
		}

		public string Id { get; set; }
		public string Name { get; set; }
		public string BlockDataPath { get; set; }
		public string ImagePath { get; set; }
	}
}
