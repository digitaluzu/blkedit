using UnityEngine;
using System.Collections;

namespace BlkEdit
{
	public class GridLayer
	{
		public GridLayer (Uzu.VectorI2 dimensions)
		{
			_dimensions = dimensions;
			
			int totalCount = Uzu.VectorI2.ElementProduct (_dimensions);
			_states = new bool[totalCount];
			_colors = new Color32[totalCount];
		}

		public bool IsSet (Uzu.VectorI2 coord)
		{
			int idx = CoordToIndex (_dimensions, coord);
			return _states [idx];
		}

		public void Unset (Uzu.VectorI2 coord)
		{
			int idx = CoordToIndex (_dimensions, coord);
			_states [idx] = false;
		}

		public Color32 GetColor (Uzu.VectorI2 coord)
		{
			int idx = CoordToIndex (_dimensions, coord);
			return _colors [idx];
		}

		public void SetColor (Uzu.VectorI2 coord, Color32 color)
		{
			int idx = CoordToIndex (_dimensions, coord);
			_states [idx] = true;
			_colors [idx] = color;
		}

		public void Clear ()
		{
			for (int i = 0; i < _states.Length; i++) {
				_states [i] = false;
			}
		}

		#region Implementation.
		private Uzu.VectorI2 _dimensions;
		private bool[] _states;
		private Color32[] _colors;

		private static int CoordToIndex (Uzu.VectorI2 dimensions, Uzu.VectorI2 coord)
		{
			return coord.y * dimensions.x + coord.x;
		}
		#endregion
	};
}