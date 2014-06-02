using UnityEngine;
using System.Collections;
using System.IO;

namespace Uzu
{
	public static class BlockReader
	{
		public static BlockPack Read (byte[] data)
		{
			using (MemoryStream stream = new MemoryStream (data)) {
				using (BinaryReader reader = new BinaryReader (stream)) {
					return ReadImpl (reader);
				}
			}
		}

		#region Implementation.
		private static BlockPack ReadImpl (BinaryReader reader)
		{
			int version = reader.ReadInt32 ();

			// Handle support of multiple released data versions.
			// Deprecate support for versions as necessary.
			switch (version) {
			case 1:
				return ReadVersion_1 (version, reader);
			default:
				Debug.LogError ("Unsupported version: " + version);
				break;
			}

			return null;
		}

		private static BlockPack ReadVersion_1 (int version, BinaryReader reader)
		{
			BlockFormat.Header header = new BlockFormat.Header ();

			{
				header.version = version;
				header.xCount = reader.ReadInt32 ();
				header.yCount = reader.ReadInt32 ();
			}

			{
				/*
			BlockPack pack = new BlockPack ();
			
			{
				BlockWorld blockWorld;
				
				// Create block world.
				{
					Uzu.BlockWorldConfig config = new Uzu.BlockWorldConfig ();
					config.BlockSize = Blk.Constants.BLOCK_SIZE;
					config.ChunkSizeInBlocks = new VectorI3 (format.xCount, format.yCount, Blk.Constants.CHUNK_SIZE_IN_BLOCKS_Z);
					config.MaxBlockTypeCount = (int)Uzu.BlockType.SYSTEM_DEFAULT_COUNT;
					{
						Uzu.BlockDesc[] descs = new Uzu.BlockDesc[(int)Uzu.BlockType.SYSTEM_DEFAULT_COUNT];
						{
							Uzu.BlockDesc desc = new Uzu.BlockDesc ();
							descs [0] = desc;
						}
						{
							Uzu.BlockDesc desc = new Uzu.BlockDesc ();
							desc.Material = Blk.Main.Mat;
							descs [1] = desc;
						}
						config.BlockDescs = descs;
					}
					
					GameObject blockWorldGO = new GameObject ("UzuBlockWorld", typeof(Uzu.BlockWorld));
					blockWorld = blockWorldGO.GetComponent<Uzu.BlockWorld> ();
					blockWorld.Initialize (config);
				}
				
				// TODO: manually load here?
				blockWorld.LoadChunk (Uzu.VectorI3.zero);
				
				{
					int cnt = 0;
					for (int x = 0; x < format.xCount; x++) {
						for (int y = 0; y < format.yCount; y++) {
							bool isBlock = format._blocks [cnt];
							if (isBlock) {
								Uzu.VectorI3 idx = new VectorI3 (x, y, 0);
								blockWorld.SetBlockType (idx, Uzu.BlockType.SOLID);
								blockWorld.SetBlockColor (idx, format._colors [cnt].ToColor32 ());
							}
							cnt++;
						}
					}
				}
				
				pack._blockWorld = blockWorld;
			}
			*/
			}

			return null;
		}
		#endregion
	}
}
