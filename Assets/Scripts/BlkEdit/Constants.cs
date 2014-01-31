using UnityEngine;

namespace Blk
{
	/// <summary>
	/// Panel ids for the user interface.
	/// </summary>
	public static class PanelIds
	{
		public const string PANEL_MAIN = "Panel-Main";
	}
	
	public static class Constants
	{
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
	}

	public enum BlockType : byte
	{
		SOLID = Uzu.BlockType.SYSTEM_COUNT,
		MAX_COUNT,
	}
}