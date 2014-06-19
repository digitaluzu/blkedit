using UnityEngine;
using System.Collections;

namespace Blk
{
	public static class FileUtil
	{
		public static string RootPath {
			get { return Application.persistentDataPath; }
		}

		public static string LocalModelPath {
			get { return System.IO.Path.Combine(RootPath, "local/"); }
		}

		public static string SavedModelPath {
			get { return System.IO.Path.Combine(RootPath, "saved/"); }
		}

		public static string GetNewFileName ()
		{
			System.Guid guid = System.Guid.NewGuid ();
			return string.Format (@"{0}", guid);
		}
	}
}
