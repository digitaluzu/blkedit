using UnityEngine;
using System.Collections;

namespace BlkEdit
{
	public class GridConfig
	{
		public Uzu.VectorI3 Dimensions { get; set; }
	}

	public class Grid : Uzu.BaseBehaviour
	{
		public event OnGridCellSetDelegate OnGridCellSet;
		public event OnGridCellUnsetDelegate OnGridCellUnset;

		public delegate void OnGridCellSetDelegate (Uzu.VectorI2 coord, Color32 color);
		public delegate void OnGridCellUnsetDelegate (Uzu.VectorI2 coord);

		public void Initialize (GridConfig config)
		{
			_config = config;

			{
				Uzu.VectorI2 dimensions2D = new Uzu.VectorI2 (_config.Dimensions.x, _config.Dimensions.y);
				_currentLayer = new GridLayer (dimensions2D);
			}
		}
		
		public bool IsSet (Uzu.VectorI2 coord)
		{
			return _currentLayer.IsSet (coord);
		}
		
		public void Unset (Uzu.VectorI2 coord)
		{
			_currentLayer.Unset (coord);

			if (OnGridCellUnset != null) {
				OnGridCellUnset (coord);
			}
		}
		
		public Color32 GetColor (Uzu.VectorI2 coord)
		{
			return _currentLayer.GetColor (coord);
		}
		
		public void SetColor (Uzu.VectorI2 coord, Color32 color)
		{
			_currentLayer.SetColor (coord, color);

			if (OnGridCellSet != null) {
				OnGridCellSet (coord, color);
			}
		}
		
		#region Implementation.
		private GridConfig _config;
		private BlkEdit.GridLayer _currentLayer;
		#endregion
	}
}