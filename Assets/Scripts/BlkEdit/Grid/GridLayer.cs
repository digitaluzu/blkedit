using UnityEngine;
using System.Collections;

namespace Blk
{
	public class GridLayer
	{
		public GridLayer (Uzu.VectorI2 dimensions)
		{
			_dimensions = dimensions;
			
			int totalCount = Uzu.VectorI2.ElementProduct (_dimensions);
			_cells = new GridCell[totalCount];
		}
		
		public bool IsCellSet (Uzu.VectorI2 cellCoord)
		{
			int idx = CoordToIndex (cellCoord);
			return _cells [idx] != null;
		}

		public GridCell GetCell (Uzu.VectorI2 cellCoord)
		{
			int idx = CoordToIndex (cellCoord);
			return _cells [idx];
		}
		
		public void SetCell (Uzu.VectorI2 cellCoord, GridCell cell)
		{
			int idx = CoordToIndex (cellCoord);
			_cells [idx] = cell;
		}

		public void ClearAll ()
		{
			for (int i = 0; i < _cells.Length; i++) {
				GridCell cell = _cells [i];
				if (cell != null) {
					cell.Unspawn ();
					_cells [i] = null;
				}
			}
		}

		#region Implementation.
		private Uzu.VectorI2 _dimensions;
		private GridCell[] _cells;
		
		private int CoordToIndex (Uzu.VectorI2 cellCoord)
		{
			return cellCoord.y * _dimensions.x + cellCoord.x;
		}
		#endregion
	};
}