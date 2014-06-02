using UnityEngine;
using System.Collections;

namespace Uzu
{
	// TODO:
	public class BlockPack
	{
		public BlockWorld _blockWorld;
	}

	public static class BlockFormat
	{
		public const int CURRENT_VERSION = 1;

		public class Header
		{
			public int version;
			public int xCount;
			public int yCount;
		}

		public class Data
		{
			public bool[] _states;
			public RGB[] _colors;
		}

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
	}
}