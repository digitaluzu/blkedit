using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Uzu
{
	public static class BlockImportExportUtil
	{
		public static byte[] SerializeToBytes<T>(T item)
		{
			var formatter = new BinaryFormatter();
			using (var stream = new MemoryStream())
			{
				formatter.Serialize(stream, item);
				stream.Seek(0, SeekOrigin.Begin);
				return stream.ToArray();
			}
		}

		public static object DeserializeFromBytes(byte[] bytes)
		{
			var formatter = new BinaryFormatter();
			using (var stream = new MemoryStream(bytes))
			{
				return formatter.Deserialize(stream);
			}
		}
	}

	// TODO: wraps multiple block worlds, which will allow us to deal with animations and things... provide a nice wrapper for it
	public class BlockPack
	{
		public BlockWorld _blockWorld;
	}
	
	[System.Serializable]
	public class BlockFormat
	{
		public int xCount;
		public int yCount;

		public bool[] _blocks;

		[System.Serializable]
		public struct RGB
		{
			byte r;
			byte g;
			byte b;

			public RGB (byte inR, byte inG, byte inB)
			{
				r = inR;
				g = inG;
				b = inB;
			}

			public Color32 ToColor32 ()
			{
				return new Color32 (r, g, b, 255);
			}
		}

		public RGB[] _colors;
	}

	public class BlockExporter
	{
		public bool Export (string filePath, BlockPack pack)
		{
			Debug.Log ("Exporting to: " + filePath);

			// Create directory if necessary.
			Directory.CreateDirectory (Path.GetDirectoryName (filePath));

			FileStream resourceFile = new FileStream (filePath, FileMode.Create, FileAccess.Write);
			BinaryWriter writer = new BinaryWriter (resourceFile);

			BlockFormat format = new BlockFormat ();
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

			byte[] bytes = BlockImportExportUtil.SerializeToBytes <BlockFormat> (format);
			writer.Write (bytes);


			Debug.Log(resourceFile.Length);

			writer.Close ();
			resourceFile.Close ();

			return true;
		}
	}

	public class BlockImporter
	{
		public BlockPack Import (string filePath)
		{
			Debug.Log ("Importing from: " + filePath);

			FileStream resourceFile = new FileStream (filePath, FileMode.Open);
			BinaryFormatter bf = new BinaryFormatter ();
			BlockFormat format = (BlockFormat)bf.Deserialize (resourceFile);

			resourceFile.Close ();

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

			return pack;
		}
	}
}