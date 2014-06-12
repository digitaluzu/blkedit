using UnityEngine;

namespace Blk
{
	public static class PanelIds
	{
		public const string PANEL_CANVAS = "Panel-Canvas";
		public const string PANEL_OPTIONS = "Panel-Options";
	}
	
	public static class Constants
	{
		public static readonly System.Guid API_KEY = new System.Guid ("9C085DC5-EA3C-484B-A767-2F56019AB0F3");
		public const string SERVER_URL = "http://localhost:3000/";

		public static readonly Vector3 BLOCK_SIZE = Vector3.one;

		public const int CHUNK_SIZE_IN_BLOCKS_X = 16;
		public const int CHUNK_SIZE_IN_BLOCKS_Y = CHUNK_SIZE_IN_BLOCKS_X;
		public const int CHUNK_SIZE_IN_BLOCKS_Z = 1;
		/// <summary>
		/// The number of active (loaded) chunks in x/y/z.
		/// </summary>
		public static readonly Uzu.VectorI3 LOADED_CHUNK_COUNT = new Uzu.VectorI3 (1, 1, 1);
	
		public static Uzu.VectorI3 CHUNK_SIZE_IN_BLOCKS {
			get { return new Uzu.VectorI3 (CHUNK_SIZE_IN_BLOCKS_X, CHUNK_SIZE_IN_BLOCKS_Y, CHUNK_SIZE_IN_BLOCKS_Z); }
		}

		public static readonly Uzu.VectorI2 GRID_DIMENSIONS = new Uzu.VectorI2 (16, 16);
	}
}