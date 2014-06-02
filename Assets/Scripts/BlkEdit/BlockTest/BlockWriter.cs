using UnityEngine;
using System.Collections;
using System.IO;

namespace Uzu
{
	public static class BlockWriter
	{
		public static byte[] Write (BlockPack pack)
		{
			using (MemoryStream stream = new MemoryStream ()) {
				using (BinaryWriter writer = new BinaryWriter (stream)) {
					WriteImpl (writer, pack);
					return stream.ToArray ();
				}
			}
		}

		#region Implementation
		private static void WriteImpl (BinaryWriter writer, BlockPack pack)
		{
			BlockFormat.Header header = PrepareHeader (pack);
			BlockFormat.Data data = PrepareData (pack);

			{
				writer.Write (header.version);
				writer.Write (header.xCount);
				writer.Write (header.yCount);
			}

			{

			}
		}

		private static BlockFormat.Header PrepareHeader (BlockPack pack)
		{
			BlockFormat.Header header = new BlockFormat.Header ();

			{
				header.version = BlockFormat.CURRENT_VERSION;
				
				Uzu.VectorI3 chunkSizeInBlocks = pack._blockWorld.Config.ChunkSizeInBlocks;
				header.xCount = chunkSizeInBlocks.x;
				header.yCount = chunkSizeInBlocks.y;
			}

			return header;
		}

		private static BlockFormat.Data PrepareData (BlockPack pack)
		{
			BlockFormat.Data data = new BlockFormat.Data ();

			/*
			 * 			BlockFormat format = new BlockFormat ();
			{
				Uzu.VectorI3 chunkSizeInBlocks = pack._blockWorld.Config.ChunkSizeInBlocks;
				format.xCount = chunkSizeInBlocks.x;
				format.yCount = chunkSizeInBlocks.y;

				int totalCount = chunkSizeInBlocks.x * chunkSizeInBlocks.y;
				format._blocks = new bool[totalCount];
				format._colors = new BlockFormat.RGB[totalCount];

				int cnt = 0;
				for (int x = 0; x < chunkSizeInBlocks.x; x++) {
					for (int y = 0; y < chunkSizeInBlocks.y; y++) {
						Uzu.VectorI3 idx = new VectorI3 (x, y, 0);
						Uzu.BlockType blockType = pack._blockWorld.GetBlockType (idx);
						if (blockType != BlockType.EMPTY) {
							format._blocks [cnt] = true;
							Color32 c = pack._blockWorld.GetBlockColor (idx);
							format._colors [cnt] = new BlockFormat.RGB (c.r, c.g, c.b);
						}
						cnt++;
					}
				}
			}
			 */

			return data;
		}
		#endregion
	}
}
