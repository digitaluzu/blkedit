using UnityEngine;
using System.Collections;
using System.IO;

namespace Uzu
{
	public static class BlockWriter
	{
		public static byte[] Write (Chunk chunk)
		{
			using (MemoryStream stream = new MemoryStream ()) {
				using (BinaryWriter writer = new BinaryWriter (stream)) {
					WriteImpl (writer, chunk);
					return stream.ToArray ();
				}
			}
		}

		#region Implementation
		private static void WriteImpl (BinaryWriter writer, Chunk chunk)
		{
			BlockFormat.Header header = PrepareHeader (chunk);
			BlockFormat.Data data = PrepareData (chunk);

			{
				writer.Write (header.version);
				writer.Write (header.count.x);
				writer.Write (header.count.y);
				writer.Write (header.count.z);
			}

			{
				for (int i = 0; i < data._states.Length; i++) {
					writer.Write (data._states [i]);

					BlockFormat.RGB rgb = data._colors [i];
					writer.Write (rgb.r);
					writer.Write (rgb.g);
					writer.Write (rgb.b);
				}
			}
		}

		private static BlockFormat.Header PrepareHeader (Chunk chunk)
		{
			BlockFormat.Header header = new BlockFormat.Header ();

			{
				header.version = BlockFormat.CURRENT_VERSION;
				header.count = chunk.Blocks.CountXYZ;
			}

			return header;
		}

		private static BlockFormat.Data PrepareData (Chunk chunk)
		{
			BlockFormat.Data data = new BlockFormat.Data ();

			VectorI3 xyz = chunk.Blocks.CountXYZ;

			{
				int totalCount = VectorI3.ElementProduct (xyz);
				data._states = new bool[totalCount];
				data._colors = new BlockFormat.RGB[totalCount];
			}

			int cnt = 0;
			for (int x = 0; x < xyz.x; x++) {
				for (int y = 0; y < xyz.y; y++) {
					for (int z = 0; z < xyz.z; z++) {
						BlockType blockType = chunk.Blocks [cnt].Type;

						if (blockType != BlockType.EMPTY) {
							data._states [cnt] = true;
							data._colors [cnt] = new BlockFormat.RGB (chunk.Blocks [cnt].Color);
						}

						cnt++;
					}
				}
			}

			return data;
		}
		#endregion
	}
}
